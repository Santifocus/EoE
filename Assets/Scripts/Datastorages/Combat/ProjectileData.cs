﻿using UnityEngine;

namespace EoE.Combatery
{
	public class ProjectileData : ScriptableObject
	{
		//Direction
		public InherritDirection DirectionStyle = InherritDirection.Target;
		public InherritDirection FallbackDirectionStyle = InherritDirection.Local;
		public DirectionBase Direction = DirectionBase.Forward;

		//Flight
		public float Duration = 15;
		public float FlightSpeed = 25;
		public Vector3 CreateOffsetToCaster = Vector3.forward;
		public CustomFXObject[] VisualStartEffects = new CustomFXObject[0];
		public EffectAOE[] StartEffects = new EffectAOE[0];
		public EffectAOE[] WhileEffects = new EffectAOE[0];

		//Collision
		public float TerrainHitboxSize = 0.5f;
		public float EntitieHitboxSize = 1;
		public ColliderMask CollideMask = (ColliderMask)(-1);
		public int Bounces = 0;
		public bool DestroyOnEntiteBounce = true;
		public bool CollisionEffectsOnBounce = true;

		public CustomFXObject[] VisualCollisionEffects = new CustomFXObject[0];

		public EffectAOE[] CollisionEffectsAOE = new EffectAOE[0];
		public EffectSingle DirectHit;

		//Remenants
		public bool CreatesRemenants = false;
		public ProjectileRemenants Remenants = new ProjectileRemenants();
	}
	[System.Serializable]
	public class ProjectileRemenants
	{
		public float Duration = 5;
		public CustomFXObject[] VisualEffects = new CustomFXObject[0];
		public EffectAOE[] StartEffects = new EffectAOE[0];
		public EffectAOE[] WhileEffects = new EffectAOE[0];
		public bool TryGroundRemenants = true;
	}
}