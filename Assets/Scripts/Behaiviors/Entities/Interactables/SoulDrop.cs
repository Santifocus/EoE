using EoE.Information;
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
		[SerializeField] private GameObject mainParticles = default;
		[SerializeField] private ParticleSystem soulParticles = default;

		[Space(10)]
		[Header("Animation")]
		[SerializeField] private float textColorCycleTime = 3;
		[SerializeField] private float gravityMultiplier = 0.4f;
		[SerializeField] private float fadeOutTime = 1.5f;

		[Space(5)]
		[SerializeField] private Color amountColor = Color.blue;

		private static float colorTime;
		private int soulCount;

		public void Setup(int soulCount)
		{
			canBeInteracted = false;
			this.soulCount = soulCount;

			string amountColHex = ColorUtility.ToHtmlStringRGBA(amountColor);
			infoSign.text = "Free <color=#" + amountColHex + ">" + soulCount + "</color> Souls. [A]";

			infoSign.gameObject.SetActive(isTarget);
			canBeInteracted = true;
		}
		protected override void Interact()
		{
			Player.Instance.AddExperience(soulCount);

			int totalCurrency = GameController.CurrentGameSettings.BaseCurrencyPerSoul * soulCount + Random.Range(0, GameController.CurrentGameSettings.ExtraRandomCurrencyPerSoul + 1) * soulCount;
			Player.Instance.ChangeCurrency(totalCurrency);

			infoSign.gameObject.SetActive(false);
			for (int i = 0; i < EffectsOnInteract.Length; i++)
			{
				if (EffectsOnInteract[i] is TextBasedFX)
				{
					(string, string)[] replaceInstructions = new (string, string)[2]
					{
						("{Experience}", soulCount.ToString()),
						("{Currency}", totalCurrency.ToString())
					};
					FXManager.PlayFX((EffectsOnInteract[i] as TextBasedFX).CreateInstructedNotification(replaceInstructions), transform, true);
					continue;
				}
				FXManager.PlayFX(EffectsOnInteract[i], transform, true);
			}

			StartCoroutine(FadeAway());
		}
		private IEnumerator FadeAway()
		{
			infoSign.gameObject.SetActive(false);

			canBeInteracted = false;
			float timer = 0;
			ParticleSystem.EmissionModule em = soulParticles.emission;
			em.rateOverTime = soulCount / fadeOutTime;
			soulParticles.Play();
			EffectUtils.FadeAndDestroyParticles(mainParticles, 0);
			bool emissionDisabled = false;

			while (timer < fadeOutTime || mainParticles)
			{
				if(timer >= fadeOutTime && !emissionDisabled)
				{
					emissionDisabled = true;
					em.rateOverTime = 0;
				}

				yield return new WaitForEndOfFrame();
				timer += Time.deltaTime;
			}
			if (!emissionDisabled)
				em.rateOverTime = 0;

			ParticleSystem.MainModule mm = soulParticles.main;
			Destroy(gameObject, mm.startLifetime.constantMax * 1.1f);
		}
		private void FixedUpdate()
		{
			body.velocity = new Vector3(0, Mathf.Min(0, body.velocity.y - Time.fixedDeltaTime * Physics.gravity.y * (1 - gravityMultiplier)), 0);

			if (isTarget && Player.Targetable)
			{
				colorTime += Time.fixedDeltaTime / textColorCycleTime;
				float point = (Mathf.Sin(colorTime * Mathf.PI) + 1) / 2;
				infoSign.color = colorOverTime.Evaluate(point);

				Vector3 facingDir = Vector3.Lerp(Player.Instance.actuallWorldPosition, PlayerCameraController.PlayerCamera.transform.position, 0.35f);
				Vector3 signDir = (infoSign.transform.position - facingDir).normalized;
				infoSign.transform.forward = signDir;
			}
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