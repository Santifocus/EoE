using EoE.Behaviour.Entities;
using UnityEngine;

namespace EoE.Combatery
{
	public class WeaponHitbox : MonoBehaviour
	{
		[SerializeField] private Collider coll = default;
		private WeaponController controller;
		private Collider TerrainCollider;
		private Collider EntitieCollider;
		public void Setup(WeaponController controller)
		{
			this.controller = controller;
			SetupColliders();
		}
		private void SetupColliders()
		{
			coll.enabled = false;

			TerrainCollider = Instantiate(coll, transform);
			EntitieCollider = Instantiate(coll, transform);

			CleanCollider(TerrainCollider.gameObject);
			CleanCollider(EntitieCollider.gameObject);

			TerrainCollider.gameObject.layer = ConstantCollector.TERRAIN_COLLIDING_LAYER;
			EntitieCollider.gameObject.layer = ConstantCollector.ENTITIE_COLLIDING_LAYER;

			Physics.IgnoreCollision(TerrainCollider, Player.Instance.coll);
			Physics.IgnoreCollision(EntitieCollider, Player.Instance.coll);

			TerrainCollider.isTrigger = EntitieCollider.isTrigger = false;
		}
		private void CleanCollider(GameObject target)
		{
			//Remove all non collider components
			Component[] allComponents = target.GetComponents<Component>();
			for (int i = 0; i < allComponents.Length; i++)
			{
				if (!(allComponents[i] is Collider || allComponents[i] is Transform))
					Destroy(allComponents[i]);
			}
			//Remove all children, destroy is called at the end of the frame so we can iterate throught with no worries
			for (int i = 0; i < target.transform.childCount; i++)
			{
				Destroy(target.transform.GetChild(i).gameObject);
			}
		}
		public void SetColliderStyle(AttackStyle style)
		{
			TerrainCollider.enabled = style != null ? AttackStyle.HasCollisionMask(style.CollisionMask, ColliderMask.Terrain) : false;
			EntitieCollider.enabled = style != null ? AttackStyle.HasCollisionMask(style.CollisionMask, ColliderMask.Entities) : false;
		}
		public void IgnoreCollision(Collider other, bool state)
		{
			if (!other)
				return;
			Physics.IgnoreCollision(TerrainCollider, other, state);
			Physics.IgnoreCollision(EntitieCollider, other, state);
		}
		private void OnCollisionEnter(Collision collision)
		{
			controller.HitObject(collision.GetContact(0).point, collision.collider, collision.GetContact(0).normal * -1);
		}
	}
}