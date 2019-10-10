using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EoE
{
	public class DamageNumber : MonoBehaviour, IPoolableObject<DamageNumber>
	{
		public PoolableObject<DamageNumber> SelfPool { get; set; }
		[SerializeField] private TextMeshPro display = default;
		[SerializeField] private SpriteRenderer critIcon = default;
		private float lifeTime;

		private Gradient changeGradient;
		private Vector3 velocity;

		public void BeginDisplay(Vector3 velocity, Gradient changeGradient, float damageAmount, bool wasCritcal)
		{
			lifeTime = 0;
			this.changeGradient = changeGradient;

			Color curColor = GetCurrentColor(0);

			display.text = (Mathf.Round(damageAmount * 100) / 100).ToString();
			display.color = curColor;
			display.ForceMeshUpdate();

			this.velocity = velocity;

			if (wasCritcal)
			{
				critIcon.color = curColor;
				critIcon.gameObject.SetActive(true);
				critIcon.transform.localPosition = new Vector3(display.textBounds.extents.x + 0.1f, 0, 0);
			}
			else
			{
				critIcon.gameObject.SetActive(false);
			}
		}

		private void Update()
		{
			lifeTime += Time.deltaTime;
			float lifePoint = lifeTime / GameController.CurrentGameSettings.DamageNumberLifeTime;

			Color curColor = GetCurrentColor(lifePoint);
			display.color = curColor;
			critIcon.color = curColor;

			transform.position += velocity * Time.deltaTime;

			if (lifePoint > 1)
				ReturnToPool();
		}
		private Color GetCurrentColor(float lifePoint)
		{
			return changeGradient.Evaluate(lifePoint);
		}
		public void ReturnToPool()
		{
			SelfPool.ReturnPoolObject(this);
		}
	}
}