using EoE.Combatery;
using EoE.Entities;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using UnityEngine;

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
			EventManager.PlayerCausedDamageEvent += PlayerCausedDamage;
			EventManager.PlayerLevelupEvent += PlayerLevelUp;
			EventManager.EntitieDiedEvent += EnemyKilled;
			EventManager.PlayerDiedEvent += PlayerDied;
		}
		private void OnDestroy()
		{
			Unsubscribe();
		}
		private void Unsubscribe()
		{
			EventManager.PlayerTookDamageEvent -= PlayerTookDamage;
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
			}

			if (knockBack.HasValue)
			{
				for (int i = 0; i < playerSettings.EffectsOnReceiveKnockback.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnReceiveKnockback[i], Player.Instance.transform, true);
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
		private void EnemyKilled(Entity killed, Entity killer)
		{
			if (killer is Player && Player.Instance)
			{
				for (int i = 0; i < playerSettings.EffectsOnEnemyKilled.Length; i++)
				{
					PlayFX(playerSettings.EffectsOnEnemyKilled[i], Player.Instance.transform, true);
				}
			}
		}
		private void PlayerDied(Entity killer)
		{
			for (int i = 0; i < playerSettings.EffectsOnPlayerDeath.Length; i++)
			{
				PlayFX(playerSettings.EffectsOnPlayerDeath[i], Player.Instance.transform, true);
			}
			Unsubscribe();
		}
		private void PlayerCausedDamage(Entity receiver, bool wasCrit)
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
		public static FXInstance PlayFX(CustomFXObject customFX, Transform target, bool allowPlayerOnlyEffects, float multiplier = 1)
		{
			return PlayFX(customFX.FX,
							target,
							allowPlayerOnlyEffects,
							multiplier,
							customFX.HasPositionOffset ? ((Vector3?)customFX.PositionOffset) : null,
							customFX.HasRotationOffset ? ((Vector3?)customFX.RotationOffset) : null,
							customFX.HasCustomScale ? ((Vector3?)customFX.CustomScale) : null
							);
		}
		public static FXInstance PlayFX(FXObject effect, Transform target, bool allowPlayerOnlyEffects, float multiplier = 1, Vector3? customOffset = null, Vector3? customRotationOffset = null, Vector3? customScale = null)
		{
			if (allowPlayerOnlyEffects)
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
				else if (effect is CustomUI)
				{
					return EffectUtils.PlayCustomUI(effect as CustomUI, multiplier);
				}
				else if(effect is Notification)
				{
					return EffectUtils.ShowNotification(effect as Notification, multiplier);
				}
				else if (effect is Dialogue)
				{
					return EffectUtils.ShowDialogue(effect as Dialogue);
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