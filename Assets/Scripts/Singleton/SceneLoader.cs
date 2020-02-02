using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EoE
{
	public class SceneLoader : MonoBehaviour
	{
		private const float LOAD_FINISH_PROGRESS = 0.9f;
		private const float MAX_DELTA_STEP = 0.025f;
		public static SceneLoader Instance { get; private set; }
		public static bool Transitioning => Instance.transitioning;

		[SerializeField] private Graphic[] sceneTransitionGraphics = default;
		[SerializeField] private float fadeTime = 0.5f;

		[Header("Loading Screen")]
		[SerializeField] private Graphic[] loadingSceneGraphics = default;
		[SerializeField] private float loadScreenMinTime = 2;
		[SerializeField] private float hintFadeTime = 0.5f;
		[SerializeField] private GameObject[] possibleHints = default;

		private bool transitioning;
		private int targetSceneID;

		private float[] sceneTransitionGraphicsAlphaValues;
		private float[] loadingSceneGraphicAlphaValues;

		private void Start()
		{
			Instance = this;

			for(int i = 0; i < possibleHints.Length; i++)
			{
				possibleHints[i].SetActive(false);
			}

			sceneTransitionGraphicsAlphaValues = FetchAlphaValues(sceneTransitionGraphics, false);
			loadingSceneGraphicAlphaValues = FetchAlphaValues(loadingSceneGraphics, false);
		}
		private float[] FetchAlphaValues(Graphic[] graphics, bool setState)
		{
			float[] alphaValues = new float[graphics.Length];
			for (int i = 0; i < graphics.Length; i++)
			{
				alphaValues[i] = graphics[i].color.a;
				graphics[i].color = new Color(graphics[i].color.r, graphics[i].color.g, graphics[i].color.b, 0);
				graphics[i].gameObject.SetActive(setState);
			}
			return alphaValues;
		}
		public static void TransitionToScene(int sceneID, bool loadScreen)
		{
			Instance.targetSceneID = sceneID;

			if (Transitioning)
				return;

			Instance.transitioning = true;
			if (loadScreen)
				Instance.StartCoroutine(Instance.TransitionWithLoadScreen());
			else
				Instance.StartCoroutine(Instance.TransitionScenes());
		}
		private IEnumerator TransitionScenes()
		{
			for (int i = 0; i < sceneTransitionGraphics.Length; i++)
			{
				sceneTransitionGraphics[i].gameObject.SetActive(true);
			}

			//Fade in
			float timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);

				float alphaMul = timer / fadeTime;
				for (int i = 0; i < sceneTransitionGraphics.Length; i++)
				{
					sceneTransitionGraphics[i].color = new Color(	sceneTransitionGraphics[i].color.r, 
																	sceneTransitionGraphics[i].color.g, 
																	sceneTransitionGraphics[i].color.b, 
																	sceneTransitionGraphicsAlphaValues[i] * alphaMul);
				}
			}

			//AsyncLoad target scene
			EffectManager.ResetFX();
			yield return SceneManager.LoadSceneAsync(targetSceneID);

			//Fade out
			timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);
				float alphaMul = 1 - (timer / fadeTime);

				//BlackPlane graphics
				for (int i = 0; i < sceneTransitionGraphics.Length; i++)
				{
					sceneTransitionGraphics[i].color = new Color(	sceneTransitionGraphics[i].color.r, 
																	sceneTransitionGraphics[i].color.g, 
																	sceneTransitionGraphics[i].color.b, 
																	sceneTransitionGraphicsAlphaValues[i] * alphaMul);
				}
			}

			//Reset transition graphics
			for (int i = 0; i < sceneTransitionGraphics.Length; i++)
			{
				sceneTransitionGraphics[i].color = new Color(sceneTransitionGraphics[i].color.r, sceneTransitionGraphics[i].color.g, sceneTransitionGraphics[i].color.b, 0);
				sceneTransitionGraphics[i].gameObject.SetActive(false);
			}

			transitioning = false;
		}

		private IEnumerator TransitionWithLoadScreen()
		{
			AsyncOperation loadingSceneLoadOperation = SceneManager.LoadSceneAsync(ConstantCollector.LOAD_SCENE_INDEX);
			loadingSceneLoadOperation.allowSceneActivation = false;

			for (int i = 0; i < loadingSceneGraphics.Length; i++)
			{
				loadingSceneGraphics[i].gameObject.SetActive(true);
			}

			//Fade in
			float timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);

				float alphaMul = timer / fadeTime;
				for (int i = 0; i < loadingSceneGraphics.Length; i++)
				{
					loadingSceneGraphics[i].color = new Color(	loadingSceneGraphics[i].color.r, 
																loadingSceneGraphics[i].color.g, 
																loadingSceneGraphics[i].color.b, 
																loadingSceneGraphicAlphaValues[i] * alphaMul);
				}
			}

			//Switch to loading scene
			while (loadingSceneLoadOperation.progress < LOAD_FINISH_PROGRESS)
			{
				yield return new WaitForEndOfFrame();
			}
			loadingSceneLoadOperation.allowSceneActivation = true;

			//AsyncLoad target scene
			EffectManager.ResetFX();
			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneID);
			loadOperation.allowSceneActivation = false;

			timer = 0;
			float finishTime = loadScreenMinTime * (1 + Random.value * 0.2f);

			//Prepare tipp
			GameObject targetTipp = possibleHints[Random.Range(0, possibleHints.Length)];

			Graphic[] hintGraphics = targetTipp.GetComponentsInChildren<Graphic>();
			float[] hintGraphicsAlphaValues = FetchAlphaValues(hintGraphics, true);
			targetTipp.SetActive(true);

			while ((timer < finishTime) || (loadOperation.progress < LOAD_FINISH_PROGRESS))
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);

				//Update alpha of the target hint
				float alphaMul = Mathf.Clamp01(timer / hintFadeTime);
				for (int i = 0; i < hintGraphicsAlphaValues.Length; i++)
				{
					hintGraphics[i].color = new Color(	hintGraphics[i].color.r, 
														hintGraphics[i].color.g, 
														hintGraphics[i].color.b, 
														hintGraphicsAlphaValues[i] * alphaMul);
				}
			}

			loadOperation.allowSceneActivation = true;

			//Fade out
			timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);
				float alphaMul = 1 - (timer / fadeTime);

				//BlackPlane graphics
				for (int i = 0; i < loadingSceneGraphics.Length; i++)
				{
					loadingSceneGraphics[i].color = new Color(	loadingSceneGraphics[i].color.r, 
																loadingSceneGraphics[i].color.g, 
																loadingSceneGraphics[i].color.b, 
																loadingSceneGraphicAlphaValues[i] * alphaMul);
				}
				//Hint graphics
				for (int i = 0; i < hintGraphicsAlphaValues.Length; i++)
				{
					hintGraphics[i].color = new Color(	hintGraphics[i].color.r, 
														hintGraphics[i].color.g, 
														hintGraphics[i].color.b,
														hintGraphicsAlphaValues[i] * alphaMul);
				}
			}

			//Reset all alpha values
			//Loadscene graphics
			for (int i = 0; i < loadingSceneGraphics.Length; i++)
			{
				loadingSceneGraphics[i].color = new Color(loadingSceneGraphics[i].color.r, loadingSceneGraphics[i].color.g, loadingSceneGraphics[i].color.b, 0);
				loadingSceneGraphics[i].gameObject.SetActive(false);
			}
			//Hint graphics
			for (int i = 0; i < hintGraphicsAlphaValues.Length; i++)
			{
				hintGraphics[i].color = new Color(	hintGraphics[i].color.r, 
													hintGraphics[i].color.g, 
													hintGraphics[i].color.b, 
													hintGraphicsAlphaValues[i]);
			}
			targetTipp.SetActive(false);
			transitioning = false;
		}
	}
}