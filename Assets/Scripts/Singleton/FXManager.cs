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
	public class FXManager : MonoBehaviour
	{
		public static FXManager Instance { get; private set; }
		private PlayerSettings playerSettings => Player.PlayerSettings;
		private void Start()
		{
			Instance = this;
			EventManager.PlayerTookDamageEvent += PlayerTookDamage;
			EventManager.PlayerLandedEvent += PlayerLanded;
			EventManager.PlayerDodgeEvent += PlayerDodged;
			EventManager.PlayerCausedDamageEvent += PlayerCausedDamage;
			EventManager.PlayerLevelupEvent += PlayerLevelUp;
			EventManager.EntitieDiedEvent += EnemyKilled;
		}
		private void OnDestroy()
		{
			EventManager.PlayerTookDamageEvent -= PlayerTookDamage;
			EventManager.PlayerLandedEvent -= PlayerLanded;
			EventManager.PlayerDodgeEvent -= PlayerDodged;
			EventManager.PlayerCausedDamageEvent -= PlayerCausedDamage;
			EventManager.PlayerLevelupEvent -= PlayerLevelUp;
			EventManager.EntitieDiedEvent -= EnemyKilled;
		}
		private void PlayerTookDamage(float causedDamage, float? knockBack)
		{
			if (causedDamage > 0)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveDamage.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnReceiveDamage[i], Player.Instance.transform, true, 1);
				}

				if ((Player.Instance.curHealth - causedDamage) / Player.Instance.curMaxHealth < playerSettings.EffectsHealthThreshold)
				{
					for (int i = 0; i < playerSettings.EffectsOnDamageWhenBelowThreshold.Length; i++)
					{
						PlayFX(playerSettings.EffectsOnDamageWhenBelowThreshold[i], Player.Instance.transform, true, 1);
					}
				}
			}

			if (knockBack.HasValue)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveKnockback.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnReceiveKnockback[i], Player.Instance.transform, true, 1);
				}
			}

		}
		private void PlayerLanded(float velocity)
		{
			if (velocity > playerSettings.PlayerLandingVelocityThreshold)
			{
				for (int i = 0; i < playerSettings.EffectsOnPlayerLanding.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnPlayerLanding[i], Player.Instance.transform, true, 1);
				}
			}
		}
		private void PlayerDodged()
		{
			for (int i = 0; i < playerSettings.EffectsOnPlayerDodge.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnPlayerDodge[i], Player.Instance.transform, true, 1);
			}
		}
		private void EnemyKilled(Entitie killed, Entitie killer)
		{
			if (killer is Player)
			{
				for (int i = 0; i < playerSettings.EffectsOnEnemyKilled.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnEnemyKilled[i], Player.Instance.transform, true, 1);
				}
			}
		}
		private void PlayerCausedDamage(Entitie receiver, bool wasCrit)
		{
			for (int i = 0; i < playerSettings.EffectsOnCauseDamage.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnCauseDamage[i], Player.Instance.transform, true, 1);
			}

			if (wasCrit)
			{
				for (int i = 0; i < playerSettings.EffectsOnCauseCrit.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnCauseCrit[i], Player.Instance.transform, true, 1);
				}
			}
		}
		private void PlayerLevelUp()
		{
			for (int i = 0; i < playerSettings.EffectsOnLevelup.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnLevelup[i], Player.Instance.transform, true, 1);
			}
		}
		public static FXInstance PlayFX(FXObject effect, Transform target, bool allowScreenEffects, float multiplier = 1)
		{
			if (allowScreenEffects)
			{
				if (effect is ScreenShake)
				{
					return EffectUtils.ShakeScreen(effect as ScreenShake, multiplier);
				}
				else if (effect is ControllerRumble)
				{
					return EffectUtils.RumbleController(effect as ControllerRumble, multiplier);
				}
				else if (effect is ScreenBlur)
				{
					return EffectUtils.BlurScreen(effect as ScreenBlur, multiplier);
				}
				else if (effect is ScreenBorderColor)
				{
					return EffectUtils.ColorScreenBorder(effect as ScreenBorderColor, multiplier);
				}
				else if (effect is ScreenTint)
				{
					return EffectUtils.TintScreen(effect as ScreenTint, multiplier);
				}
				else if (effect is CameraFOVWarp)
				{
					return EffectUtils.WarpCameraFOV(effect as CameraFOVWarp, multiplier);
				}
				else if (effect is DialogueInput)
				{
					DialogueController.CreateAndShowDialogue(effect as DialogueInput);
					return null;
				}
			}

			if (effect is TimeDilation)
			{
				return EffectUtils.DilateTime(effect as TimeDilation, multiplier);
			}
			else if (effect is SoundEffect)
			{
				return EffectUtils.PlaySound(effect as SoundEffect, target, multiplier);
			}
			else if (effect is ParticleEffect)
			{
				return EffectUtils.PlayParticleEffect(effect as ParticleEffect, target);
			}
			return null;
		}
	}
}