using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EnemyHealthBar : MonoBehaviour
	{
		[SerializeField] private Image sliderImage = default;
		[SerializeField] private RectTransform rTransform;
		public float Value { get; set; }
		private Vector3 pos;
		public Vector3 Position { get => pos; set => SetPosition(value); }
		private float displayedValue;

		private void Start()
		{
			displayedValue = 1;
			Value = 1;
		}
		private void SetPosition(Vector3 pos)
		{
			this.pos = pos;
			rTransform.position = pos;
		}
		private void Update()
		{
			displayedValue = Mathf.Lerp(displayedValue, Value, Time.deltaTime * GameController.CurrentGameSettings.EnemeyHealthBarLerpSpeed);
			sliderImage.fillAmount = displayedValue;
		}
	}
}