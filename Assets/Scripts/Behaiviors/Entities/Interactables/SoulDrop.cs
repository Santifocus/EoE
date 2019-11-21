using System.Collections;
using TMPro;
using UnityEngine;

namespace EoE.Entities
{
	public class SoulDrop : Interactable
	{
		[Header("References")]
		[SerializeField] private TextMeshPro infoSign = default;
		[SerializeField] private Gradient colorOverTime = default;
		[SerializeField] private Rigidbody body = default;
		[SerializeField] private MeshRenderer sphereRenderer = default;
		[SerializeField] private ParticleSystem soulParticles = default;

		[Space(10)]
		[Header("Animation")]
		[SerializeField] private float textColorCycleTime = 3;
		[SerializeField] private float gravityMultiplier = 0.4f;
		[SerializeField] private float fadeInTime = 1.5f;
		[SerializeField] private float fadeOutTime = 1.5f;

		[Space(5)]
		[SerializeField] private Color amountColor = Color.blue;
		[SerializeField] private float finallSphereSize = 1;
		[SerializeField] private int pulsateCount = 5;
		[SerializeField] private float pulsateStrenght = 0.5f;
		[SerializeField] private Gradient pulsateColorGradient = default;

		private static float colorTime;
		private int soulCount;

		public void Setup(int soulCount)
		{
			canBeInteracted = false;
			this.soulCount = soulCount;

			string amountColHex = ColorUtility.ToHtmlStringRGBA(amountColor);
			infoSign.text = "Free <color=#" + amountColHex + ">" + soulCount + "</color> Souls. [A]";
			StartCoroutine(FadeIn());
		}
		protected override void Interact()
		{
			Player.Instance.AddSouls(soulCount);
			infoSign.gameObject.SetActive(false);

			StartCoroutine(FadeAway());
		}
		private IEnumerator FadeIn()
		{
			float timer = 0;
			float timePerPuls = fadeInTime / pulsateCount;

			while (timer < fadeInTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;

				float point = Mathf.Sin(timer / timePerPuls * Mathf.PI);

				float scale = timer / fadeInTime * finallSphereSize + point * pulsateStrenght;
				sphereRenderer.transform.localScale = scale * Vector3.one;
				sphereRenderer.material.color = pulsateColorGradient.Evaluate((point + 1) / 2);
			}
			sphereRenderer.transform.localScale = finallSphereSize * Vector3.one;
			infoSign.gameObject.SetActive(isTarget);

			canBeInteracted = true;
		}
		private IEnumerator FadeAway()
		{
			infoSign.gameObject.SetActive(false);

			canBeInteracted = false;
			float timer = 0;
			ParticleSystem.EmissionModule em = soulParticles.emission;
			em.rateOverTime = soulCount / fadeOutTime;
			soulParticles.Play();

			while (timer < fadeOutTime)
			{
				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;

				float scale = (1 - timer / fadeOutTime) * finallSphereSize;
				sphereRenderer.transform.localScale = scale * Vector3.one;
			}
			em.rateOverTime = 0;
			ParticleSystem.MainModule mm = soulParticles.main;

			sphereRenderer.transform.localScale = Vector3.zero;

			Destroy(gameObject, mm.startLifetime.constantMax * 1.1f);
		}

		private void Update()
		{
			if (isTarget)
			{
				colorTime += Time.deltaTime / textColorCycleTime;
				float point = (Mathf.Sin(colorTime * Mathf.PI) + 1) / 2;
				infoSign.color = colorOverTime.Evaluate(point);

				Vector3 facingDir = Vector3.Lerp(Player.Instance.actuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.35f);
				Vector3 signDir = (infoSign.transform.position - facingDir).normalized;
				infoSign.transform.forward = signDir;
			}
		}

		private void FixedUpdate()
		{
			body.velocity = new Vector3(0, Mathf.Min(0, body.velocity.y - Time.fixedDeltaTime * Physics.gravity.y * (1 - gravityMultiplier)), 0);
		}

		protected override void MarkAsInteractTarget()
		{
			infoSign.gameObject.SetActive(true && canBeInteracted);
		}

		protected override void StopMarkAsInteractable()
		{
			infoSign.gameObject.SetActive(false);
		}
	}
}