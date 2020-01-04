using EoE.Combatery;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class UltimateBarController : MonoBehaviour
	{
		public static UltimateBarController Instance { get; private set; }
		[SerializeField] [Range(0, 1)] private float minFill = 0.1f;
		[SerializeField] [Range(0, 1)] private float maxFill = 0.685f;
		[SerializeField] private float barLerpSpeed = 10;
		[SerializeField] private float fadeCycleSpeed = 3;

		[Space(5)]
		[Header("Bars")]
		[SerializeField] private Image bar = default;
		[SerializeField] private Image barGlow = default;

		[Space(8)]
		[SerializeField] private Image buttonIcon = default;
		[SerializeField] private Image ultimateIcon = default;

		private WeaponUltimate settings;
		private float curfill;
		private bool barFilled;
		private float fadeTimer;

		private void Start()
		{
			Instance = this;
		}
		public void Setup(WeaponUltimate settings)
		{
			gameObject.SetActive(settings != null);
			if (settings == null)
				return;

			this.settings = settings;
			SetFill(0);
			ultimateIcon.sprite = settings.Ultimate.UltimateIcon;

			barFilled = false;
			barGlow.gameObject.SetActive(false);
			buttonIcon.gameObject.SetActive(false);
		}
		private void LateUpdate()
		{
			if (settings == null)
				return;

			float targetFill = (settings.TotalRequiredCharge <= 0) ? (1) : (WeaponController.Instance.ultimateCharge / settings.TotalRequiredCharge);
			SetFill(Mathf.Lerp(curfill, targetFill, barLerpSpeed * Time.deltaTime));

			if (barFilled)
			{
				fadeTimer += Time.deltaTime * fadeCycleSpeed;
				float glowAlpha = Mathf.Max(0, Mathf.Sin(fadeTimer * 2));
				float buttonAlpha = Mathf.Max(0, Mathf.Sin(fadeTimer));
				float iconAlpha = Mathf.Max(0, Mathf.Sin(fadeTimer + Mathf.PI));

				barGlow.color = new Color(barGlow.color.r, barGlow.color.g, barGlow.color.b, glowAlpha);
				buttonIcon.color = new Color(buttonIcon.color.r, buttonIcon.color.g, buttonIcon.color.b, buttonAlpha);
				ultimateIcon.color = new Color(ultimateIcon.color.r, ultimateIcon.color.g, ultimateIcon.color.b, iconAlpha);
			}

			bool nowFilled = targetFill >= 1;
			if (barFilled != nowFilled)
			{
				barFilled = nowFilled;
				if (barFilled)
				{
					fadeTimer = 0;
				}
				else
				{
					ultimateIcon.color = new Color(ultimateIcon.color.r, ultimateIcon.color.g, ultimateIcon.color.b, 1);
				}
				barGlow.gameObject.SetActive(barFilled);
				buttonIcon.gameObject.SetActive(barFilled);
			}
		}
		private void SetFill(float fill)
		{
			float normalizedFill = fill * (maxFill - minFill);
			bar.fillAmount = normalizedFill + minFill;
			curfill = fill;
		}
	}
}