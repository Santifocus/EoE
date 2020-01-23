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
		private void Start()
		{
			StartCoroutine(TutorialControl());
		}
		private IEnumerator TutorialControl()
		{
			for(int i = 0; i < settings.Parts.Length; i++)
			{
				float curWaitTime = settings.Parts[i].DelayTime;

				while(curWaitTime > 0)
				{
					yield return new WaitForEndOfFrame();
					curWaitTime -= Time.unscaledDeltaTime;
				}
				FXInstance[] activeFX = Player.Instance.ActivateActivationEffects(settings.Parts[i].Effects);

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
			}
			Debug.Log("Tutorial Finished");
		}
	}
}