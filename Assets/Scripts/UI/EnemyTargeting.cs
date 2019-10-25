using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EnemyTargeting : MonoBehaviour
	{
		[SerializeField] private RectTransform indicatorAnchor = default;
		[SerializeField] private Image[] indicatorParts = default;
		[SerializeField] private Gradient changeStateColors = default;
		[SerializeField] private float changeStateTime = 0.5f;
		[SerializeField] private float spinSpeed = 1;
		[SerializeField] private float inOutSpeed = 0.55f;
		[SerializeField] private float inOutDistance = 5;
		[SerializeField] private float toAnchorDistance = 30;
		[SerializeField] private float startDistance = 90;
		[SerializeField] private float whenFadeOutAnimationSpeed = 0.35f;
		[SerializeField] private float indicatorBaseRotaion = 0;
		private float showState;
		private bool isShown;

		private float indicatorAnimationTime;
		private void Update()
		{
			isShown = Player.targetedEntitie != null;
			bool changedColor = false;

			if(isShown && !indicatorAnchor.gameObject.activeInHierarchy)
				indicatorAnchor.gameObject.SetActive(true);

			if (isShown && showState < 1)
			{
				changedColor = true;
				showState += Time.deltaTime / changeStateTime;
				if (showState > 1)
				{
					showState = 1;
				}
			}
			else if(!isShown && showState > 0)
			{
				changedColor = true;
				showState -= Time.deltaTime / changeStateTime;
				if(showState < 0)
				{
					showState = 0;
					indicatorAnchor.gameObject.SetActive(false);
				}
			}
			UpdateIndicators(changedColor);
		}
		private void UpdateIndicators(bool changedColor)
		{
			RepositionIndicators();
			AnimateIndicators();
			if (changedColor)
				UpdateIndicatorColors();
		}
		private void RepositionIndicators()
		{
			if (isShown)
			{
				indicatorAnchor.position = PlayerCameraController.PlayerCamera.WorldToScreenPoint(Player.targetedEntitie.actuallWorldPosition);
				indicatorAnchor.gameObject.SetActive(indicatorAnchor.position.z > 0);
			}
		}
		private void AnimateIndicators()
		{
			indicatorAnimationTime = indicatorAnimationTime + Time.deltaTime * (isShown ? 1 : whenFadeOutAnimationSpeed) * Mathf.PI;
			float curDist = toAnchorDistance + Mathf.Sin(indicatorAnimationTime * inOutSpeed) * inOutDistance + (1 - showState) * startDistance;

			float baseRotation = spinSpeed * indicatorAnimationTime;
			indicatorAnchor.eulerAngles = new Vector3(0, 0, baseRotation * Mathf.Rad2Deg);

			float rotationPerPart = (Mathf.PI * 2) / indicatorParts.Length;
			for (int i = 0; i < indicatorParts.Length; i++)
			{
				float selfRotation = baseRotation + rotationPerPart * i;
				indicatorParts[i].rectTransform.localPosition = new Vector3(Mathf.Cos(selfRotation), Mathf.Sin(selfRotation), 0) * curDist;
				indicatorParts[i].rectTransform.localEulerAngles = new Vector3(0, 0, selfRotation * Mathf.Rad2Deg + indicatorBaseRotaion);
			}
		}
		private void UpdateIndicatorColors()
		{
			Color col = changeStateColors.Evaluate(showState);
			for(int i = 0; i < indicatorParts.Length; i++)
			{
				indicatorParts[i].color = col;
			}
		}
	}
}