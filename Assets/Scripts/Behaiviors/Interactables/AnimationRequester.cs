using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.UI;
using UnityEngine.Video;

namespace EoE.Entities
{
	public class AnimationRequester : SceneChangeInteractable
	{
		[SerializeField] private bool doLoadingScreenOut = false;
		[SerializeField] private VideoClip targetAnimation = default;
		protected override void Interact()
		{
			AnimationSceneController.RequestAnimation(targetAnimation, targetSceneIndex, doLoadingScreen, doLoadingScreenOut);
		}
	}
}