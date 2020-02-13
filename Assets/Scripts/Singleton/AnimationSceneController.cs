using EoE.Controlls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EoE.UI
{
	public class AnimationSceneController : MonoBehaviour
	{
		private static bool IsCustomRequest = false;
		private static VideoClip RequestedAnimation = null;
		private static int RequestedSceneIndex = ConstantCollector.MAIN_MENU_SCENE_INDEX;
		private static bool AllowSkip = false;
		private static bool ShouldDoLoadingScreen = false;

		[SerializeField] private VideoPlayer animationPlayer = default;
		[SerializeField] private VideoClip defaultAnimation = default;

		[SerializeField] private float fadeTime = 0.4f;
		[SerializeField] private Camera rendererCamera = default;
		[SerializeField] private AudioListener audioListener = default;
		[SerializeField] private RawImage renderImage = default;
		[SerializeField] private GameObject skipDisplay = default;

		private const int WAIT_FRAMES = 5;
		private void Start()
		{
			animationPlayer.clip = RequestedAnimation != null ? RequestedAnimation : defaultAnimation;
			skipDisplay.SetActive(AllowSkip);
			animationPlayer.Play();
			SetupCamera();

			StartCoroutine(DelayedEndTest());
		}
		public static void RequestAnimation(VideoClip requestedAnimation, int requestedSceneIndex, bool allowSkip, bool shouldDoLoadingScreenIn, bool shouldDoLoadingScreenOut)
		{
			IsCustomRequest = true;
			RequestedAnimation = requestedAnimation;
			AllowSkip = allowSkip;
			RequestedSceneIndex = requestedSceneIndex;
			ShouldDoLoadingScreen = shouldDoLoadingScreenOut;
			SceneLoader.TransitionToScene(ConstantCollector.ANIMATION_SCENE_INDEX, shouldDoLoadingScreenIn);
		}
		private void SetupCamera()
		{
			RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 1);
			rendererCamera.targetTexture = renderTexture;
			renderImage.texture = renderTexture;
			rendererCamera.Render();
			renderImage.gameObject.SetActive(true);
		}
		private IEnumerator DelayedEndTest()
		{
			for(int i = 0; i < WAIT_FRAMES; i++)
			{
				yield return new WaitForEndOfFrame();
			}
			bool isFinishing = false;
			while (true)
			{
				yield return new WaitForEndOfFrame();
				if (!isFinishing && (!animationPlayer.isPlaying || (InputController.Pause.Down && AllowSkip)))
				{
					isFinishing = true;
				}

				if(isFinishing)
				{
					if (!SceneLoader.Transitioning)
					{
						audioListener.enabled = false;
						if (IsCustomRequest)
							SceneLoader.TransitionToScene(RequestedSceneIndex, ShouldDoLoadingScreen);
						else
							SceneManager.LoadScene(RequestedSceneIndex, LoadSceneMode.Additive);

						break;
					}
				}
			}
			float timer = 0;

			while (timer < fadeTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Mathf.Min(Time.unscaledDeltaTime, 0.05f);
				renderImage.color = new Color(1, 1, 1, 1 - (timer / fadeTime));
			}

			RequestedAnimation = null;
			RequestedSceneIndex = ConstantCollector.MAIN_MENU_SCENE_INDEX;
			AllowSkip = false;
			if(!IsCustomRequest)
				SceneManager.UnloadSceneAsync(ConstantCollector.ANIMATION_SCENE_INDEX);
			IsCustomRequest = false;
		}
	}
}