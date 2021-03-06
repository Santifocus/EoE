﻿using EoE.Combatery;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class ShieldData : ScriptableObject
	{
		public CombatObject BaseData = default;
		public ObjectCost FirstActivationCost = default;
		public EffectSingle EffectOnUserOnShieldBreak = default;

		public float ShieldResistance = 20;
		public float ShieldDrain = 1;
		public float ShieldRegeneration = 3;
		public float ShieldSize = 2;
		public bool ReflectProjectiles = false;

		public bool FollowOwner = true;
		public bool InheritOwnerRotation = true;
		public Vector3 OffsetToOwner = Vector3.zero;
		public Vector3 RotationOffsetToOwner = Vector3.zero;

		public ShieldLevelEffects[] ShieldLevelBasedEffects = new ShieldLevelEffects[0];

		public ActivationEffect[] ShieldFailedStartEffects = new ActivationEffect[0];
		public ActivationEffect[] ShieldStartEffects = new ActivationEffect[0];
		public ActivationEffect[] ShieldHitEffects = new ActivationEffect[0];
		public ActivationEffect[] ShieldDisableEffects = new ActivationEffect[0];
		public ActivationEffect[] ShieldBreakEffects = new ActivationEffect[0];
	}
	[System.Serializable]
	public class ShieldLevelEffects
	{
		public float MinShieldLevel;
		public float MaxShieldLevel;
		public ActivationEffect[] Effects = new ActivationEffect[0];
	}
}