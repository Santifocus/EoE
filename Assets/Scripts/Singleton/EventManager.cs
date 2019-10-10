﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Entities;
using EoE.Information;

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

		//Entitie Death
		public delegate void EntitieDied(Entitie killed, Entitie killer);
		public static EntitieDied EntitieDiedEvent;
		public static void EntitieDiedInvoke(Entitie killed, Entitie killer)
		{
			EntitieDiedEvent?.Invoke(killed, killer);
		}
	}
}