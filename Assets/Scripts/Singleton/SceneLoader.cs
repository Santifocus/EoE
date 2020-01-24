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
		public static SceneLoader Instance { get; private set; }
		public static bool Transitioning => Instance.transitioning;

		[SerializeField] private Image blackPlane = default;
		[SerializeField] private float fadeTime = 0.5f;
		[SerializeField] private float minLoadSceneTime = 2;

		[Header("Loading Screen Hints")]
		[SerializeField] private float hintFadeTime = 0.5f;
		[SerializeField] private GameObject[] possibleHints = default;

		private bool transitioning;
		private int targetSceneID;

		private void Start()
		{
			Instance = this;
			blackPlane.gameObject.SetActive(false);

			for(int i = 0; i < possibleHints.Length; i++)
			{
				possibleHints[i].SetActive(false);
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
			blackPlane.gameObject.SetActive(true);

			AsyncOperation loadingSceneLoadOperation = SceneManager.LoadSceneAsync(ConstantCollector.LOAD_SCENE_INDEX);
			loadingSceneLoadOperation.allowSceneActivation = false;

			//Fade in blackout
			float timer = 0;
			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.unscaledDeltaTime;
				blackPlane.color = new Color(0, 0, 0, timer / fadeTime);
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
				timer += Time.unscaledDeltaTime;

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
				timer += Time.unscaledDeltaTime;
				float alphaMul = 1 - (timer / fadeTime);

				blackPlane.color = new Color(0, 0, 0, alphaMul);
				for (int i = 0; i < hintAlphaValues.Length; i++)
				{
					hintGraphics[i].color = new Color(hintGraphics[i].color.r, hintGraphics[i].color.g, hintGraphics[i].color.b, alphaMul * hintAlphaValues[i]);
				}
			}

			//Disable the hint
			targetTipp.SetActive(false);
			//Reset all alpha values
			for (int i = 0; i < hintAlphaValues.Length; i++)
			{
				hintGraphics[i].color = new Color(hintGraphics[i].color.r, hintGraphics[i].color.g, hintGraphics[i].color.b, hintAlphaValues[i]);
			}

			transitioning = false;
			blackPlane.gameObject.SetActive(false);
		}
	}
}