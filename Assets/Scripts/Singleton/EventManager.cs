using EoE.Behaviour.Entities;
using EoE.Information;

namespace EoE.Events
{
	public static class EventManager
	{
		//Entitie Death
		public delegate void EntitieDied(Entity killed, Entity killer);
		public static EntitieDied EntitieDiedEvent;
		public static void EntitieDiedInvoke(Entity killed, Entity killer)
		{
			EntitieDiedEvent?.Invoke(killed, killer);
		}

		//Player Received Damage
		public delegate void PlayerTookDamage(float causedDamage, float? knockBack);
		public static PlayerTookDamage PlayerTookDamageEvent;
		public static void PlayerTookDamageInvoke(float causedDamage, float? knockBack)
		{
			PlayerTookDamageEvent?.Invoke(causedDamage, knockBack);
		}

		//Player Dashed
		public delegate void PlayerDodge();
		public static PlayerDodge PlayerDodgeEvent;
		public static void PlayerDashInvoke()
		{
			PlayerDodgeEvent?.Invoke();
		}

		//Player Caused Damage
		public delegate void PlayerCausedDamage(Entity receiver, ChangeInfo changeInfo, bool wasCrit);
		public static PlayerCausedDamage PlayerCausedDamageEvent;
		public static void PlayerCausedDamageInvoke(Entity receiver, ChangeInfo changeInfo, bool wasCrit)
		{
			PlayerCausedDamageEvent?.Invoke(receiver, changeInfo, wasCrit);
		}

		//Player First Strike
		public delegate void PlayerFirstStrike(Entity receiver);
		public static PlayerFirstStrike PlayerFirstStrikeEvent;
		public static void PlayerFirstStrikeInvoke(Entity receiver)
		{
			PlayerFirstStrikeEvent?.Invoke(receiver);
		}

		//Player Experience changed
		public delegate void PlayerExperienceChanged();
		public static PlayerExperienceChanged PlayerExperienceChangedEvent;
		public static void PlayerExperienceChangedInvoke()
		{
			PlayerExperienceChangedEvent?.Invoke();
		}

		//Player Currency changed
		public delegate void PlayerCurrencyChanged();
		public static PlayerCurrencyChanged PlayerCurrencyChangedEvent;
		public static void PlayerCurrencyChangedInvoke()
		{
			PlayerCurrencyChangedEvent?.Invoke();
		}

		//Player Levelup
		public delegate void PlayerLevelup();
		public static PlayerLevelup PlayerLevelupEvent;
		public static void PlayerLevelupInvoke()
		{
			PlayerLevelupEvent?.Invoke();
		}

		//Player Death
		public delegate void PlayerDied(Entity killer);
		public static PlayerDied PlayerDiedEvent;
		public static void PlayerDiedInvoke(Entity killer)
		{
			PlayerDiedEvent?.Invoke(killer);
		}
	}
}