using EoE.Entities;
using EoE.Events;
using EoE.Information;
using EoE.UI;
using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public class PlayerEventFXManager : MonoBehaviour
	{
		private const float KNOCKBACK_VALUE_DIVIDER = 16;
		public static PlayerEventFXManager Instance { get; private set; }
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
		private void Unsubscribe()
		{
			EventManager.PlayerTookDamageEvent -= PlayerTookDamage;
			EventManager.PlayerCausedDamageEvent -= PlayerCausedDamage;
			EventManager.PlayerLevelupEvent -= PlayerLevelUp;
			EventManager.EntitieDiedEvent -= EnemyKilled;
			EventManager.PlayerDiedEvent -= PlayerDied;
		}
		private void PlayerDied(Entity killer) => Unsubscribe();
		private void OnDestroy()
		{
			Unsubscribe();
		}
		private void PlayerTookDamage(float causedDamage, float? knockBack)
		{
			if (causedDamage > 0)
			{
				FXManager.ExecuteFX(playerSettings.EffectsOnReceiveDamage, Player.Instance.transform, true);
			}

			if (knockBack.HasValue)
			{
				FXManager.ExecuteFX(playerSettings.EffectsOnReceiveKnockback, Player.Instance.transform, true, knockBack.Value / KNOCKBACK_VALUE_DIVIDER);
			}
		}
		private void PlayerLevelUp()
		{
			FXManager.ExecuteFX(playerSettings.EffectsOnLevelup, Player.Instance.transform, true);
		}
		private void EnemyKilled(Entity killed, Entity killer)
		{
			if (killer is Player && Player.Existant)
			{
				FXManager.ExecuteFX(playerSettings.EffectsOnEnemyKilled, Player.Instance.transform, true);
			}
		}
		private void PlayerCausedDamage(Entity receiver, ChangeInfo changeInfo, bool wasCrit)
		{
			if (changeInfo.cause != CauseType.Physical)
				return;

			FXManager.ExecuteFX(playerSettings.EffectsOnCauseDamage, Player.Instance.transform, true);

			if (wasCrit)
			{
				FXManager.ExecuteFX(playerSettings.EffectsOnCauseCrit, Player.Instance.transform, true);
			}
		}
	}
}