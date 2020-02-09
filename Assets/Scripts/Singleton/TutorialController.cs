using EoE.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.UI
{
	public class TutorialController : MonoBehaviour
	{
		[SerializeField] private TutorialSettings settings = default;
		[SerializeField] private Vector3 dummySpawnPos = default;
		[SerializeField] private Dummy dummyPrefab = default;
		[SerializeField] private UnityEngine.Video.VideoClip introAnimation = default;

		private Dummy spawnedDummy;

		private void Start()
		{
			StartCoroutine(TutorialControl());
		}
		private IEnumerator TutorialControl()
		{
			for(int i = 0; i < settings.Parts.Length; i++)
			{
				//Wait for a set delay
				float curWaitTime = settings.Parts[i].DelayTime;
				while(curWaitTime > 0)
				{
					yield return new WaitForEndOfFrame();
					curWaitTime -= Time.unscaledDeltaTime;
				}

				//Activate Effects and store fxinstances
				FXInstance[] activeFX = Player.Instance.ActivateActivationEffects(settings.Parts[i].Effects);

				//Create a dummy
				if (settings.Parts[i].SpawnDummy && !spawnedDummy)
				{
					spawnedDummy = Instantiate(dummyPrefab);
					spawnedDummy.transform.position = dummySpawnPos;
				}

				//Wait untill all FX Instances are timed out
				bool atLeastOneActive = true;
				while (atLeastOneActive)
				{
					yield return new WaitForEndOfFrame();
					atLeastOneActive = false;
					for (int j = 0; j < activeFX.Length; j++)
					{
						if (!activeFX[j].IsRemoved)
						{
							atLeastOneActive = true;
							break;
						}
					}
				}

				//Delete the dummy
				if(settings.Parts[i].DeleteDummyOnFinish && spawnedDummy)
				{
					spawnedDummy.BaseDeath(null);
				}
			}

			AnimationSceneController.RequestAnimation(introAnimation, ConstantCollector.GAME_SCENE_INDEX, false, true);
		}
	}
}