using EoE.Information;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EntitieStatDisplay : MonoBehaviour
	{
		private const float DONE_LERP_THRESHOLD = 0.01f;
		[SerializeField] private Image healthBarSlider = default;
		[SerializeField] private Image remenantHealthBarSlider = default;
		[SerializeField] private RectTransform selfTransform = default;
		[SerializeField] private GridLayoutGroup buffGrid = default;
		[SerializeField] private Transform buffIconPrefab = default;
		private float healthValue;
		public float HealthValue
		{
			get => healthValue;
			set
			{
				if (curLerpState == LerpState.Idle)
				{
					lerpWaitDelay = GameController.CurrentGameSettings.EntitieHealthBarLerpDelay;
					curLerpState = LerpState.Waiting;
				}

				healthBarSlider.fillAmount = healthValue = value;
			}
		}
		private Vector3 pos;
		public Vector3 Position { get => pos; set => SetPosition(value); }
		public float Height => remenantHealthBarSlider.rectTransform.rect.height;
		private Dictionary<BuffInstance, Transform> buffIconLookup;

		private enum LerpState { Idle = 1, Waiting = 2, Lerping = 3 }
		private LerpState curLerpState = LerpState.Idle;

		private float lerpWaitDelay;
		private float remenantValue;

		public void Setup()
		{
			healthBarSlider.fillAmount = remenantHealthBarSlider.fillAmount = healthValue = remenantValue = 1;
			buffIconLookup = new Dictionary<BuffInstance, Transform>();
		}
		private void Update()
		{
			if (curLerpState == LerpState.Waiting)
			{
				lerpWaitDelay -= Time.deltaTime;
				if(lerpWaitDelay <= 0)
				{
					curLerpState = LerpState.Lerping;
					lerpWaitDelay = 0;
				}
			}
			else if(curLerpState == LerpState.Lerping)
			{
				remenantValue = Mathf.MoveTowards(remenantValue, healthValue, Time.deltaTime * GameController.CurrentGameSettings.EntitieHealthBarLerpSpeed);
				if(Mathf.Abs(remenantValue - healthValue) <= DONE_LERP_THRESHOLD)
				{
					remenantValue = healthValue;
					curLerpState = LerpState.Idle;
				}
			}
			remenantHealthBarSlider.fillAmount = remenantValue;
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
		private void SetPosition(Vector3 pos)
		{
			this.pos = pos;
			selfTransform.position = pos;
		}
	}
}