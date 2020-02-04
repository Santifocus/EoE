using EoE.Entities;
using EoE.Information;
using EoE.Sounds;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Behaivior
{
	public class GameArea : MonoBehaviour
	{
		private const float TRIGGER_EFFECT_COOLDOWN = 10;
		private static GameArea PlayerCurrentArea;

		[Header("Area Settings")]
		[SerializeField] private bool isStartArea = false;
		[SerializeField] private int areaPriority = 1;
		[SerializeField] private GameAreaPart[] parts = default;

		[Space(6)]
		[Header("Trigger Effects")]
		[SerializeField] private FXObject[] firstTimeEnterEffects = default;
		[SerializeField] private FXObject[] enterEffects = default;
		[SerializeField] private FXObject[] firstTimeExitEffects = default;
		[SerializeField] private FXObject[] exitEffects = default;

		[Space(6)]
		[Header("Music")]
		[Range(0, 1)] [SerializeField] private float startVolume = 0;
		[SerializeField] private int musicPriority = 1;
		public int targetMusicIndex = 0;

		private bool areaActive;
		private bool areaActiveLastCheck;
		private float triggerEffectCooldown;
		private bool wasEntered;
		private bool wasExited;

		private MusicInstance musicInstance;

		private void Start()
		{
			musicInstance = new MusicInstance(startVolume, musicPriority, targetMusicIndex);
			if (isStartArea)
			{
				FXManager.ExecuteFX(firstTimeEnterEffects, Player.Instance.transform, true);
				wasEntered = true;
				areaActiveLastCheck = true;
				SetAreaActiveState(true);
				CheckArea();
			}
		}
		private void Update()
		{
			CheckArea();
		}
		private void CheckArea()
		{
			if (Player.Existant)
			{
				bool playerInArea = PlayerInArea();
				if(playerInArea)
				{
					if((PlayerCurrentArea != this) && !PlayerCurrentArea || (PlayerCurrentArea.areaPriority < areaPriority))
					{
						if(PlayerCurrentArea)
							PlayerCurrentArea.SetAreaActiveState(false);

						PlayerCurrentArea = this;
						SetAreaActiveState(true);
					}
				}
				else if (PlayerCurrentArea == this)
				{
					PlayerCurrentArea = null;
					SetAreaActiveState(false);
				}
			}
		}
		private void SetAreaActiveState(bool state)
		{
			areaActive = state;
			if (areaActiveLastCheck != state)
			{
				//Area now active
				if (state)
				{
					if (!wasEntered)
					{
						FXManager.ExecuteFX(firstTimeEnterEffects, Player.Instance.transform, true);
						wasEntered = true;
					}

					if (triggerEffectCooldown <= 0)
					{
						FXManager.ExecuteFX(enterEffects, Player.Instance.transform, true);
						triggerEffectCooldown = TRIGGER_EFFECT_COOLDOWN;
					}
				}
				else //Area was active and is not anymore
				{
					if (!wasExited)
					{
						FXManager.ExecuteFX(firstTimeExitEffects, Player.Instance.transform, true);
						wasExited = true;
					}

					if (triggerEffectCooldown <= 0)
					{
						FXManager.ExecuteFX(exitEffects, Player.Instance.transform, true);
						triggerEffectCooldown = TRIGGER_EFFECT_COOLDOWN;
					}
				}
			}

			//Music
			musicInstance.WantsToPlay = areaActive;
			if (!musicInstance.IsAdded && areaActive)
			{
				MusicController.Instance.AddMusicInstance(musicInstance);
			}
			areaActiveLastCheck = state;
		}
		private bool PlayerInArea()
		{
			for(int i = 0; i < parts.Length; i++)
			{
				if (parts[i].PlayerInPart())
					return true;
			}
			return false;
		}
		private void OnDrawGizmosSelected()
		{
			if (parts == null)
				return;

			for (int i = 0; i < parts.Length; i++)
			{
				if (!parts[i].transform.IsChildOf(transform))
				{
					parts[i].OnDrawGizmosSelected();
				}
			}
		}
}
}