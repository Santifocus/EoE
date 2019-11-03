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

		[Space(15)]
		[Header("Values")]
		[SerializeField] private float changeStateTime = 0.5f;
		[SerializeField] private float spinSpeed = 1;
		[SerializeField] private float inOutSpeed = 0.55f;
		[SerializeField] private float inOutDistance = 5;
		[SerializeField] private float toAnchorDistance = 30;
		[SerializeField] private float startDistance = 90;
		[SerializeField] private float whenFadeOutAnimationSpeed = 0.35f;
		[SerializeField] private float indicatorBaseRotaion = 0;

		[Space(15)]
		[Header("Target Offscreen")]
		[SerializeField] private Image offScreenIndicator = default;
		[SerializeField] private float offScreenToAnchorDist = 30;
		[SerializeField] private float offScreenIndicatorBaseRotation = 0;

		private float showState;
		private bool isShown;
		private bool onScreenState;

		private float indicatorAnimationTime;
		private void Start()
		{
			ChangeOnScreenState(true);
			ChangeOnScreenState(false);
		}
		private void Update()
		{
			isShown = Player.TargetedEntitie != null;
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
			if (changedColor)
				UpdateIndicatorColors();
		}
		private void RepositionIndicators()
		{
			if (isShown)
			{
				Vector3 unclampedPos = PlayerCameraController.PlayerCamera.WorldToScreenPoint(Player.TargetedEntitie.actuallWorldPosition);
				Vector3 clampedPos = new Vector3(Mathf.Clamp(unclampedPos.x, 0, Screen.width), Mathf.Clamp(unclampedPos.y, 0, Screen.height), unclampedPos.z);
				if(unclampedPos == clampedPos && !(unclampedPos.z < 0))
				{
					ChangeOnScreenState(true);
					AnimateIndicators();
				}
				else
				{
					if (clampedPos.z < 0)
						unclampedPos *= -1;

					clampedPos = new Vector3(Mathf.Clamp(unclampedPos.x, offScreenToAnchorDist, Screen.width - offScreenToAnchorDist), Mathf.Clamp(unclampedPos.y, offScreenToAnchorDist, Screen.height - offScreenToAnchorDist), unclampedPos.z);
					Vector2 direction = ((Vector2)clampedPos - (Vector2)unclampedPos).normalized;
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + offScreenIndicatorBaseRotation;

					indicatorAnchor.eulerAngles = new Vector3(0, 0, angle);
					offScreenIndicator.rectTransform.anchoredPosition = new Vector2(offScreenToAnchorDist, 0);
					ChangeOnScreenState(false);
				}

				indicatorAnchor.position = clampedPos;
			}
			else
			{
				//The spin out animation is unbound to enemys
				if(onScreenState)
					AnimateIndicators();
			}
		}
		private void ChangeOnScreenState(bool state)
		{
			if (state != onScreenState)
			{
				onScreenState = state;
				offScreenIndicator.gameObject.SetActive(!state);
				
				for(int i = 0; i < indicatorParts.Length; i++)
				{
					indicatorParts[i].gameObject.SetActive(state);
				}
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
			offScreenIndicator.color = col;
			for(int i = 0; i < indicatorParts.Length; i++)
			{
				indicatorParts[i].color = col;
			}
		}
	}
}