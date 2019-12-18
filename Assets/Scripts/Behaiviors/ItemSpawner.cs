using UnityEngine;

namespace EoE.Information
{
	public class ItemSpawner : MonoBehaviour
	{
		[SerializeField] private DropData[] itemsToSpawn = new DropData[0];
		private void Start()
		{
			for (int i = 0; i < itemsToSpawn.Length; i++)
			{
				itemsToSpawn[i].targetItem.CreateItemDrop(transform.position, itemsToSpawn[i].spawnAmount, itemsToSpawn[i].stopVelocity);
			}
			Destroy(gameObject);
		}
		[System.Serializable]
		public struct DropData
		{
			public Item targetItem;
			public int spawnAmount;
			public bool stopVelocity;
		}
	}
}