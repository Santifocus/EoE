using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace EoE.Entities
{
	public class SoulDrop : Interactable
	{
		[SerializeField] private TextMeshPro infoSign = default;
		[SerializeField] private Gradient colorOverTime = default;
		[SerializeField] private float colorCyleTime = 3;
		[SerializeField] private Rigidbody body = default;
		[SerializeField] private float gravityMultiplier = 0.4f;

		private static float colorTime;
		private bool isTarget;
		private int soulCount;
		private bool collected;
		private float delayTillInteractable;

		public void Setup(int soulCount)
		{
			delayTillInteractable = 0.75f;
			this.soulCount = soulCount;
			infoSign.text = "Free " + soulCount + " Souls. [X]";
		}
		public override void Interact()
		{
			if (collected || delayTillInteractable > 0)
				return;

			Player.Instance.AddSouls(soulCount);
			infoSign.gameObject.SetActive(false);

			StartCoroutine(FadeAway());
		}

		private IEnumerator FadeAway()
		{
			yield return null;
			Destroy(gameObject);
		}

		private void Update()
		{
			if (delayTillInteractable > 0)
				delayTillInteractable -= Time.deltaTime;

			if (isTarget)
			{
				colorTime += Time.deltaTime / colorCyleTime;
				float point = Mathf.Abs(Mathf.Sin(colorTime * Mathf.PI));
				infoSign.color = colorOverTime.Evaluate(point);

				Vector3 signDir = new Vector3(infoSign.transform.position.x - Player.Instance.transform.position.x, 0, infoSign.transform.position.z - Player.Instance.transform.position.z).normalized;
				infoSign.transform.forward = signDir;
			}
		}

		private void FixedUpdate()
		{
			body.velocity = new Vector3(0, Mathf.Min(0, body.velocity.y - Time.fixedDeltaTime * Physics.gravity.y * (1 - gravityMultiplier)), 0);
		}

		protected override void MarkAsInteractTarget()
		{
			isTarget = true;
			infoSign.gameObject.SetActive(true);
		}

		protected override void StopMarkAsInteractable()
		{
			isTarget = false;
			infoSign.gameObject.SetActive(false);
		}
	}
}