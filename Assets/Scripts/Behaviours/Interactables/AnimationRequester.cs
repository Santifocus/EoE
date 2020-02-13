using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using EoE.UI;

namespace EoE.Behaviour
{
	public class AnimationRequester : SceneChangeInteractable
	{
		[SerializeField] private bool doLoadingScreenOut = false;
		[SerializeField] private bool allowSkip = true;
		[SerializeField] private VideoClip targetAnimation = default;
		protected override void Interact()
		{
			AnimationSceneController.RequestAnimation(targetAnimation, targetSceneIndex, allowSkip, doLoadingScreen, doLoadingScreenOut);
		}
	}
}