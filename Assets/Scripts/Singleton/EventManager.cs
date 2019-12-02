using EoE.Entities;

namespace EoE.Events
{
	public static class EventManager
	{
		//Entitie Death
		public delegate void EntitieDied(Entitie killed, Entitie killer);
		public static EntitieDied EntitieDiedEvent;
		public static void EntitieDiedInvoke(Entitie killed, Entitie killer)
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

		//Player Landed on ground
		public delegate void PlayerLanded(float speed);
		public static PlayerLanded PlayerLandedEvent;
		public static void PlayerLandedInvoke(float speed)
		{
			PlayerLandedEvent?.Invoke(speed);
		}

		//Player Dashed
		public delegate void PlayerDodge();
		public static PlayerDodge PlayerDodgeEvent;
		public static void PlayerDodgeInvoke()
		{
			PlayerDodgeEvent?.Invoke();
		}

		//Player Caused Damage
		public delegate void PlayerCausedDamage(Entitie receiver, bool wasCrit);
		public static PlayerCausedDamage PlayerCausedDamageEvent;
		public static void PlayerCausedDamageInvoke(Entitie receiver, bool wasCrit)
		{
			PlayerCausedDamageEvent?.Invoke(receiver, wasCrit);
		}

		//Player Soulcount changed
		public delegate void PlayerSoulCountChanged();
		public static PlayerSoulCountChanged PlayerSoulCountChangedEvent;
		public static void PlayerSoulCountChangedInvoke()
		{
			PlayerSoulCountChangedEvent?.Invoke();
		}

		//Player Levelup
		public delegate void PlayerLevelup();
		public static PlayerLevelup PlayerLevelupEvent;
		public static void PlayerLevelupInvoke()
		{
			PlayerLevelupEvent?.Invoke();
		}

		//Player Death
		public delegate void PlayerDied(Entitie killer);
		public static PlayerDied PlayerDiedEvent;
		public static void PlayerDiedInvoke(Entitie killer)
		{
			PlayerDiedEvent?.Invoke(killer);
		}
	}
}