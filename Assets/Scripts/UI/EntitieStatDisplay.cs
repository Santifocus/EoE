using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EntitieStatDisplay : MonoBehaviour
	{
		[SerializeField] private Image healthBarSlider = default;
		[SerializeField] private RectTransform selfTransform = default;
		[SerializeField] private GridLayoutGroup buffGrid = default;
		[SerializeField] private Transform buffIconPrefab = default;
		public float HealthValue { get; set; }
		private Vector3 pos;
		public Vector3 Position { get => pos; set => SetPosition(value); }
		public float Height => healthBarSlider.rectTransform.rect.height;
		private float displayedValue;
		private Dictionary<BuffInstance, Transform> buffIconLookup;

		private void Start()
		{
			displayedValue = 1;
			HealthValue = 1;
			buffIconLookup = new Dictionary<BuffInstance, Transform>();
		}
		private void SetPosition(Vector3 pos)
		{
			this.pos = pos;
			selfTransform.position = pos;
		}
		private void Update()
		{
			displayedValue = Mathf.Lerp(displayedValue, HealthValue, Time.deltaTime * GameController.CurrentGameSettings.EnemeyHealthBarLerpSpeed);
			healthBarSlider.fillAmount = displayedValue;
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