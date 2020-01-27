using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EoE.UI
{
	public class LogoSceneController : MonoBehaviour
	{
		[SerializeField] private VideoPlayer logoAnimationPlayer = default;
		[SerializeField] private float fadeTime = 0.4f;
		[SerializeField] private Camera rendererCamera = default;
		[SerializeField] private AudioListener audioListener = default;
		[SerializeField] private RawImage renderImage = default;

		private const int WAIT_FRAMES = 5;
		private void Start()
		{
			SetupCamera();
			logoAnimationPlayer.Play();
			StartCoroutine(DelayedEndTest());
		}
		private void SetupCamera()
		{
			RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 1);
			rendererCamera.targetTexture = renderTexture;
			renderImage.texture = renderTexture;
		}
		private IEnumerator DelayedEndTest()
		{
			for(int i = 0; i < WAIT_FRAMES; i++)
			{
				yield return new WaitForEndOfFrame();
			}
			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (!logoAnimationPlayer.isPlaying)
				{
					audioListener.enabled = false;
					SceneManager.LoadScene(ConstantCollector.MAIN_MENU_SCENE_INDEX, LoadSceneMode.Additive);
					break;
				}
			}
			float timer = 0;

			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
				renderImage.color = new Color(1, 1, 1, 1 - (timer / fadeTime));
			}

			SceneManager.UnloadSceneAsync(0);
		}
	}
}