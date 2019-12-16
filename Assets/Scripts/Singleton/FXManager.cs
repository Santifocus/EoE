using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using EoE.Combatery;

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
			EventManager.PlayerCausedDamageEvent += PlayerCausedDamage;
			EventManager.PlayerLevelupEvent += PlayerLevelUp;
			EventManager.EntitieDiedEvent += EnemyKilled;
			EventManager.PlayerDiedEvent += PlayerDied;
		}
		private void OnDestroy()
		{
			EventManager.PlayerTookDamageEvent -= PlayerTookDamage;
			EventManager.PlayerLandedEvent -= PlayerLanded;
			EventManager.PlayerCausedDamageEvent -= PlayerCausedDamage;
			EventManager.PlayerLevelupEvent -= PlayerLevelUp;
			EventManager.EntitieDiedEvent -= EnemyKilled;
			EventManager.PlayerDiedEvent -= PlayerDied;
		}
		private void PlayerTookDamage(float causedDamage, float? knockBack)
		{
			if (causedDamage > 0)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveDamage.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnReceiveDamage[i], Player.Instance.transform, true);
				}

				if ((Player.Instance.curHealth - causedDamage) / Player.Instance.curMaxHealth < playerSettings.EffectsHealthThreshold)
				{
					for (int i = 0; i < playerSettings.EffectsOnDamageWhenBelowThreshold.Length; i++)
					{
						PlayFX(playerSettings.EffectsOnDamageWhenBelowThreshold[i], Player.Instance.transform, true);
					}
				}
			}

			if (knockBack.HasValue)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveKnockback.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnReceiveKnockback[i], Player.Instance.transform, true);
				}
			}

		}
		private void PlayerLanded(float velocity)
		{
			if (velocity > playerSettings.PlayerLandingVelocityThreshold)
			{
				for (int i = 0; i < playerSettings.EffectsOnPlayerLanding.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnPlayerLanding[i], Player.Instance.transform, true);
				}
			}
		}
		private void PlayerLevelUp()
		{
			for (int i = 0; i < playerSettings.EffectsOnLevelup.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnLevelup[i], Player.Instance.transform, true);
			}
		}
		private void EnemyKilled(Entitie killed, Entitie killer)
		{
			if (killer is Player && Player.Instance.Alive)
			{
				for (int i = 0; i < playerSettings.EffectsOnEnemyKilled.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnEnemyKilled[i], Player.Instance.transform, true);
				}
			}
		}
		private void PlayerDied(Entitie killer)
		{
			for (int i = 0; i < playerSettings.EffectsOnPlayerDeath.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnPlayerDeath[i], Player.Instance.transform, true);
			}
		}
		private void PlayerCausedDamage(Entitie receiver, bool wasCrit)
		{
			for (int i = 0; i < playerSettings.EffectsOnCauseDamage.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnCauseDamage[i], Player.Instance.transform, true);
			}

			if (wasCrit)
			{
				for (int i = 0; i < playerSettings.EffectsOnCauseCrit.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnCauseCrit[i], Player.Instance.transform, true);
				}
			}
		}
		public static FXInstance PlayFX(CustomFXObject customFX, Transform target, bool allowScreenEffects, float multiplier = 1)
		{
			return PlayFX(	customFX.FX,
							target,
							allowScreenEffects,
							multiplier,
							customFX.HasPositionOffset ? ((Vector3?)customFX.PositionOffset) : null,
							customFX.HasRotationOffset ? ((Vector3?)customFX.RotationOffset) : null,
							customFX.HasCustomScale ? ((Vector3?)customFX.CustomScale) : null
							);
		}
		public static FXInstance PlayFX(FXObject effect, Transform target, bool allowScreenEffects, float multiplier = 1, Vector3? customOffset = null, Vector3? customRotationOffset = null, Vector3? customScale = null)
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
				return EffectUtils.PlaySound(effect as SoundEffect, target, customOffset, multiplier);
			}
			else if (effect is ParticleEffect)
			{
				return EffectUtils.PlayParticleEffect(effect as ParticleEffect, target, customOffset, customRotationOffset, customScale, multiplier);
			}
			return null;
		}
	}
}