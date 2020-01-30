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

		[SerializeField] private Graphic[] blackPlaneGraphics;
		[SerializeField] private float fadeTime = 0.5f;
		[SerializeField] private float minLoadSceneTime = 2;

		[Header("Loading Screen Hints")]
		[SerializeField] private float hintFadeTime = 0.5f;
		[SerializeField] private GameObject[] possibleHints = default;

		private bool transitioning;
		private int targetSceneID;
		private float[] blackPlaneAlphaValues;

		private void Start()
		{
			Instance = this;

			for(int i = 0; i < possibleHints.Length; i++)
			{
				possibleHints[i].SetActive(false);
			}

			//Fetch alpha values of the blackplane and its children
			blackPlaneAlphaValues = new float[blackPlaneGraphics.Length];
			for (int i = 0; i < blackPlaneGraphics.Length; i++)
			{
				blackPlaneAlphaValues[i] = blackPlaneGraphics[i].color.a;
				blackPlaneGraphics[i].color = new Color(blackPlaneGraphics[i].color.r, blackPlaneGraphics[i].color.g, blackPlaneGraphics[i].color.b, 0);
				blackPlaneGraphics[i].gameObject.SetActive(false);
			}
		}
		public static void TransitionToScene(int sceneID)
		{
			Instance.targetSceneID = sceneID;

			if (Transitioning)
				return;
			Instance.StartCoroutine(Instance.TransitionControl());
		}
		private IEnumerator TransitionControl()
		{
			transitioning = true;
			AsyncOperation loadingSceneLoadOperation = SceneManager.LoadSceneAsync(ConstantCollector.LOAD_SCENE_INDEX);
			loadingSceneLoadOperation.allowSceneActivation = false;

			for(int i = 0; i < blackPlaneGraphics.Length; i++)
			{
				blackPlaneGraphics[i].gameObject.SetActive(true);
			}
			//Fade in blackout
			float timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);

				float alphaMul = timer / fadeTime;
				for (int i = 0; i < blackPlaneGraphics.Length; i++)
				{
					blackPlaneGraphics[i].color = new Color(blackPlaneGraphics[i].color.r, blackPlaneGraphics[i].color.g, blackPlaneGraphics[i].color.b, blackPlaneAlphaValues[i] * alphaMul);
				}
			}

			//Switch to loading scene
			while (loadingSceneLoadOperation.progress < LOAD_FINISH_PROGRESS)
			{
				yield return new WaitForEndOfFrame();
			}
			loadingSceneLoadOperation.allowSceneActivation = true;

			//(Un-)Load scenes
			AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneID);
			loadOperation.allowSceneActivation = false;

			timer = 0;
			float finishTime = minLoadSceneTime * (1 + Random.value * 0.5f);

			//Prepate tipp
			GameObject targetTipp = possibleHints[Random.Range(0, possibleHints.Length)];

			Graphic[] hintGraphics = targetTipp.GetComponentsInChildren<Graphic>();
			float[] hintAlphaValues = new float[hintGraphics.Length];
			for(int i = 0; i < hintAlphaValues.Length; i++)
			{
				hintAlphaValues[i] = hintGraphics[i].color.a;
				hintGraphics[i].color = new Color(hintGraphics[i].color.r, hintGraphics[i].color.g, hintGraphics[i].color.b, 0);
			}
			targetTipp.SetActive(true);

			while ((timer < finishTime) || (loadOperation.progress < LOAD_FINISH_PROGRESS))
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);

				//Update alpha of the target hint
				float alphaMul = Mathf.Clamp01(timer / hintFadeTime);
				for (int i = 0; i < hintAlphaValues.Length; i++)
				{
					hintGraphics[i].color = new Color(hintGraphics[i].color.r, hintGraphics[i].color.g, hintGraphics[i].color.b, hintAlphaValues[i] * alphaMul);
				}
			}

			loadOperation.allowSceneActivation = true;

			//Fade out blackout
			timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, MAX_DELTA_STEP);
				float alphaMul = 1 - (timer / fadeTime);

				//BlackPlane graphics
				for (int i = 0; i < blackPlaneGraphics.Length; i++)
				{
					blackPlaneGraphics[i].color = new Color(blackPlaneGraphics[i].color.r, blackPlaneGraphics[i].color.g, blackPlaneGraphics[i].color.b, blackPlaneAlphaValues[i] * alphaMul);
				}
				//Hint graphics
				for (int i = 0; i < hintAlphaValues.Length; i++)
				{
					hintGraphics[i].color = new Color(hintGraphics[i].color.r, hintGraphics[i].color.g, hintGraphics[i].color.b, alphaMul * hintAlphaValues[i]);
				}
			}

			//Reset all alpha values
			//BlackPlane graphics
			for (int i = 0; i < blackPlaneGraphics.Length; i++)
			{
				blackPlaneGraphics[i].color = new Color(blackPlaneGraphics[i].color.r, blackPlaneGraphics[i].color.g, blackPlaneGraphics[i].color.b, 0);
			}
			//Hint graphics
			for (int i = 0; i < hintAlphaValues.Length; i++)
			{
				hintGraphics[i].color = new Color(hintGraphics[i].color.r, hintGraphics[i].color.g, hintGraphics[i].color.b, hintAlphaValues[i]);
			}

			transitioning = false;
			for (int i = 0; i < blackPlaneGraphics.Length; i++)
			{
				blackPlaneGraphics[i].gameObject.SetActive(false);
			}
			targetTipp.SetActive(false);
		}
	}
}