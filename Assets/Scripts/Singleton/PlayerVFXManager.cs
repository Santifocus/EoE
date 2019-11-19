using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Events;
using EoE.Information;
using EoE.Utils;
using EoE.UI;

namespace EoE
{
	public class PlayerVFXManager : MonoBehaviour
	{
		public static PlayerVFXManager Instance { get; private set; }
		private PlayerSettings playerSettings => Player.PlayerSettings;
		private void Start()
		{
			Instance = this;
			EventManager.PlayerTookDamageEvent += PlayerTookDamage;
			EventManager.PlayerLandedEvent += PlayerLanded;
			EventManager.PlayerCausedDamageEvent += PlayerCausedDamage;
			EventManager.PlayerLevelupEvent += PlayerLevelUp;
		}
		private void OnDestroy()
		{
			EventManager.PlayerTookDamageEvent -= PlayerTookDamage;
			EventManager.PlayerLandedEvent -= PlayerLanded;
			EventManager.PlayerCausedDamageEvent -= PlayerCausedDamage;
			EventManager.PlayerLevelupEvent -= PlayerLevelUp;
		}
		private void PlayerTookDamage(float causedDamage, float? knockBack)
		{
			if (causedDamage > 0)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveDamage.Length; i++)
				{
					PlayVFX(playerSettings.EffectsOnReceiveDamage[i]);
				}
			}

			if (knockBack.HasValue)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveKnockback.Length; i++)
				{
					PlayVFX(playerSettings.EffectsOnReceiveKnockback[i]);
				}
			}

			if ((Player.Instance.curHealth - causedDamage) / Player.Instance.curMaxHealth < playerSettings.EffectsHealthThreshold)
			{
				for (int i = 0; i < playerSettings.EffectsOnDamageWhenBelowThreshold.Length; i++)
				{
					PlayVFX(playerSettings.EffectsOnDamageWhenBelowThreshold[i]);
				}
			}
		}
		private void PlayerLanded(float velocity)
		{
			if (velocity > playerSettings.PlayerLandingVelocityThreshold)
			{
				for (int i = 0; i < playerSettings.EffectsOnPlayerLanding.Length; i++)
				{
					PlayVFX(playerSettings.EffectsOnPlayerLanding[i]);
				}
			}
		}
		private void PlayerCausedDamage(Entitie receiver, bool wasCrit)
		{
			for (int i = 0; i < playerSettings.EffectsOnCauseDamage.Length; i++)
			{
				PlayVFX(playerSettings.EffectsOnCauseDamage[i]);
			}

			if (wasCrit)
			{
				for (int i = 0; i < playerSettings.EffectsOnCauseCrit.Length; i++)
				{
					PlayVFX(playerSettings.EffectsOnCauseCrit[i]);
				}
			}
		}
		private void PlayerLevelUp()
		{
			for (int i = 0; i < playerSettings.EffectsOnLevelup.Length; i++)
			{
				PlayVFX(playerSettings.EffectsOnLevelup[i]);
			}
		}
		public static void PlayVFX(VFXEffect effect)
		{
			if (GameController.GameIsPaused)
				return;

			if (effect is ScreenShake)
			{
				EffectUtils.ShakeScreen(effect as ScreenShake);
			}
			else if (effect is ControllerRumble)
			{
				EffectUtils.RumbleController(effect as ControllerRumble);
			}
			else if (effect is ScreenBlur)
			{
				EffectUtils.BlurScreen(effect as ScreenBlur);
			}
			else if (effect is ScreenBorderColor)
			{
				EffectUtils.ColorScreenBorder(effect as ScreenBorderColor);
			}
			else if (effect is ScreenTint)
			{
				EffectUtils.TintScreen(effect as ScreenTint);
			}
			else if (effect is TimeDilation)
			{
				EffectUtils.DilateTime(effect as TimeDilation);
			}
			else if (effect is ParticleEffect)
			{
				EffectUtils.PlayParticleEffect(effect as ParticleEffect, Player.Instance.transform);
			}
			else if (effect is DialogueInput)
			{
				DialogueController.CreateAndShowDialogue(effect as DialogueInput);
			}
		}
	}
}