using UnityEngine;

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
		public ActivationEffect[] StartEffects = new ActivationEffect[0];
		public ActivationEffect[] WhileEffects = new ActivationEffect[0];

		//Collision
		public float TerrainHitboxSize = 0.5f;
		public float EntitieHitboxSize = 1;
		public ColliderMask CollideMask = (ColliderMask)(-1);
		public EffectSingle DirectHit;
		public ActivationEffect[] CollisionEffects = new ActivationEffect[0];

		public int Bounces = 0;
		public bool DestroyOnEntiteBounce = true;
		public bool CollisionEffectsOnBounce = true;
	}
}