using EoE.Information;
using EoE.Information.Logic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Entities
{
	public class ContainerInteractable : Interactable
	{
		[SerializeField] private LogicComponent openCondition = default;
		[SerializeField] private ItemData[] itemsToSpawn = default;
		[SerializeField] private float itemStartSpawnDelay = 1;
		[SerializeField] private float individualItemSpawnDelay = 0.35f;
		[SerializeField] private Vector3 minSpawnVelocity = new Vector3(-1, 1, 0);
		[SerializeField] private Vector3 maxSpawnVelocity = new Vector3(1, 2, 1);

		[Space(3)]
		[SerializeField] private Collider[] containerCollider = default;

		[Space(5)]
		[Header("Feedback")]
		[SerializeField] private ActivationEffect[] effectsOnPlayerOnFailOpen = default;
		[SerializeField] private ActivationEffect[] effectsOnPlayerOnOpen = default;

		[Space(3)]
		[SerializeField] private Animator animator = default;
		[SerializeField] private string openAnimationTrigger = "Open";

		private List<ItemDrop> createdItemDrops;
		private bool creatingDrops;

		private void Start()
		{
			canBeInteracted = true;
		}
		protected override void Interact()
		{
			if (!openCondition || openCondition.True)
			{
				StartCoroutine(OpenContainer());
			}
			else
			{
				Player.Instance.ActivateActivationEffects(effectsOnPlayerOnFailOpen);
			}
		}

		private IEnumerator OpenContainer()
		{
			canBeInteracted = false;
			Player.Instance.ActivateActivationEffects(effectsOnPlayerOnOpen);
			animator.SetTrigger(openAnimationTrigger);

			float timer = 0;
			while(timer < itemStartSpawnDelay)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
			}

			createdItemDrops = new List<ItemDrop>(itemsToSpawn.Length);
			creatingDrops = true;
			StartCoroutine(ReenableItemCollisionOnFall());
			//Spawn the items
			for (int i = 0; i < itemsToSpawn.Length; i++)
			{
				ItemDrop[] droppedItems = itemsToSpawn[i].TargetItem.CreateItemDrop(transform.position, itemsToSpawn[i].StackSize, true);
				createdItemDrops.AddRange(droppedItems);

				//First disable all so we re-enable them when it is their turn to spawn
				for (int j = 0; j < droppedItems.Length; j++)
				{
					droppedItems[j].gameObject.SetActive(false);
				}

				for (int j = 0; j < droppedItems.Length; j++)
				{
					droppedItems[j].gameObject.SetActive(true);
					Vector3 velocityMul = new Vector3(	Random.Range(minSpawnVelocity.x, maxSpawnVelocity.x),
														Random.Range(minSpawnVelocity.y, maxSpawnVelocity.y),
														Random.Range(minSpawnVelocity.z, maxSpawnVelocity.z));

					SetCollisionIgnoreState(droppedItems[j].colls, true);
					droppedItems[j].body.velocity = velocityMul.x * transform.right + 
													velocityMul.y * transform.up + 
													velocityMul.z * transform.forward;

					yield return new WaitForSeconds(individualItemSpawnDelay);
				}
			}

			creatingDrops = false;
		}
		private IEnumerator ReenableItemCollisionOnFall()
		{
			yield return new WaitForFixedUpdate();
			while (creatingDrops || createdItemDrops.Count > 0)
			{
				yield return new WaitForFixedUpdate();
				for (int i = 0; i < createdItemDrops.Count; i++)
				{
					if (!createdItemDrops[i].isActiveAndEnabled)
						continue;

					if (!createdItemDrops[i] || !createdItemDrops[i].body)
					{
						createdItemDrops.RemoveAt(i);
						i--;
						continue;
					}
					if (createdItemDrops[i].body.velocity.y < 0)
					{
						SetCollisionIgnoreState(createdItemDrops[i].colls, false);
						createdItemDrops.RemoveAt(i);
						i--;
					}
				}
			}
		}

		private void SetCollisionIgnoreState(Collider[] colls, bool state)
		{
			for (int i = 0; i < containerCollider.Length; i++)
			{
				for(int j = 0; j < colls.Length; j++)
				{
					Physics.IgnoreCollision(colls[j], containerCollider[i], state);
				}
			}
		}

		protected override void MarkAsInteractTarget()
		{

		}

		protected override void StopMarkAsInteractable()
		{

		}

		[System.Serializable]
		private class ItemData
		{
			public Item TargetItem;
			public int StackSize;
		}
	}
}