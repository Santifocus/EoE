using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class ContainerInteractable : Interactable
	{
		[Space(5)]
		[Header("Interact Display")]
		[SerializeField] private ColoredText[] infoText = default;
		[SerializeField] private TextMeshPro infoDisplay = default;
		[SerializeField] private Vector3 infoDisplayOffset = new Vector3(0, 2, 0);

		[Space(5)]
		[Header("Item Spawning")]
		[SerializeField] private ItemChoiceArray[] spawnArrays = default;
		[SerializeField] private float itemStartSpawnDelay = 1;
		[SerializeField] private float individualItemSpawnDelay = 0.35f;
		[SerializeField] private Vector3 minSpawnVelocity = new Vector3(-1, 1, 0);
		[SerializeField] private Vector3 maxSpawnVelocity = new Vector3(1, 2, 1);

		[Space(3)]
		[SerializeField] private Collider[] containerCollider = default;

		[Space(5)]
		[Header("Feedback")]
		[HideInInspector] public ActivationEffect[] effectsOnPlayerOnOpen = default;
		[SerializeField] private Animator animator = default;
		[SerializeField] private string openAnimationTrigger = "Open";

		private List<ItemDrop> createdItemDrops;
		private bool creatingDrops;

		private void Start()
		{
			canBeInteracted = true;
			infoDisplay.text = ColoredText.ToString(infoText);
			infoDisplay.gameObject.SetActive(false);
			infoDisplay.transform.localPosition = infoDisplayOffset;
		}
		protected override void Interact()
		{
			Player.Instance.ActivateActivationEffects(effectsOnPlayerOnOpen);
			animator.SetTrigger(openAnimationTrigger);
			StartCoroutine(OpenContainer());
		}

		private IEnumerator OpenContainer()
		{
			canBeInteracted = false;
			float timer = 0;
			while(timer < itemStartSpawnDelay)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
			}

			createdItemDrops = new List<ItemDrop>();
			creatingDrops = true;
			StartCoroutine(ReenableItemCollisionOnFall());
			//Spawn the items
			for (int i = 0; i < spawnArrays.Length; i++)
			{
				ItemData itemsToSpawn = null;
				float totalChanceValue = 0;
				for(int j = 0; j < spawnArrays[i].PossibleChoices.Length; j++)
				{
					totalChanceValue += spawnArrays[i].PossibleChoices[j].IndividualChance;
				}

				if(totalChanceValue > 0)
				{
					float choiceChanceNormalized = Random.value;
					for (int j = 0; j < spawnArrays[i].PossibleChoices.Length; j++)
					{
						choiceChanceNormalized -= spawnArrays[i].PossibleChoices[j].IndividualChance / totalChanceValue;
						if(choiceChanceNormalized <= 0)
						{
							itemsToSpawn = spawnArrays[i].PossibleChoices[j];
							break;
						}
					}
				}
				else
				{
					itemsToSpawn = spawnArrays[i].PossibleChoices[0];
				}

				ItemDrop[] droppedItems = itemsToSpawn.TargetItem.CreateItemDrop(transform.position, itemsToSpawn.StackSize, true);
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
					if (!createdItemDrops[i] || !createdItemDrops[i].isActiveAndEnabled)
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
		private void LateUpdate()
		{
			if (!isTarget || !Player.Existant)
				return;

			Vector3 facingDir = Vector3.Lerp(Player.Instance.actuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.45f);
			Vector3 signDir = (infoDisplay.transform.position - facingDir).normalized;
			infoDisplay.transform.forward = signDir;
		}
		protected override void MarkAsInteractTarget()
		{
			infoDisplay.gameObject.SetActive(true);
		}
		protected override void StopMarkAsInteractable()
		{
			infoDisplay.gameObject.SetActive(false);
		}
		[System.Serializable]
		private class ItemChoiceArray
		{
			public ItemData[] PossibleChoices = default;
		}
		[System.Serializable]
		private class ItemData
		{
			public Item TargetItem = default;
			public int StackSize = default;
			public float IndividualChance = default;
		}
	}
}