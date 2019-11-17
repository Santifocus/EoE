using EoE.Information;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE
{
	public class PlayerBuffDisplay : MonoBehaviour
	{
		[SerializeField] private GridLayoutGroup buffGrid = default;
		[SerializeField] private Transform buffIconPrefab = default;
		private Dictionary<BuffInstance, Transform> buffIconLookup;
		private void Start()
		{
			buffIconLookup = new Dictionary<BuffInstance, Transform>();
		}
		public void AddBuffIcon(BuffInstance targetBuff)
		{
			if (!targetBuff.Base.Icon)
				return;

			Transform newBuffIconParent = Instantiate(buffIconPrefab, buffGrid.transform);
			newBuffIconParent.GetChild(0).GetComponent<Image>().sprite = targetBuff.Base.Icon;

			buffIconLookup.Add(targetBuff, newBuffIconParent);
		}
		public void RemoveBuffIcon(BuffInstance targetBuff)
		{
			if (!targetBuff.Base.Icon)
				return;

			if (buffIconLookup.ContainsKey(targetBuff))
			{
				Destroy(buffIconLookup[targetBuff].gameObject);
				buffIconLookup.Remove(targetBuff);
			}
		}
	}
}