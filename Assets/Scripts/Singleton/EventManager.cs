using EoE.Entities;

namespace EoE.Events
{
	public static class EventManager
	{
		//Player Death
		public static PlayerDied PlayerDiedEvent;
		public delegate void PlayerDied(Entitie killer);
		public static void PlayerDiedInvoke(Entitie killer)
		{
			PlayerDiedEvent?.Invoke(killer);
		}

		//Player Levelup
		public delegate void PlayerLevelup();
		public static PlayerLevelup PlayerLevelupEvent;
		public static void PlayerLevelupInvoke()
		{
			PlayerLevelupEvent?.Invoke();
		}

		//Entitie Death
		public delegate void EntitieDied(Entitie killed, Entitie killer);
		public static EntitieDied EntitieDiedEvent;
		public static void EntitieDiedInvoke(Entitie killed, Entitie killer)
		{
			EntitieDiedEvent?.Invoke(killed, killer);
		}
	}
}