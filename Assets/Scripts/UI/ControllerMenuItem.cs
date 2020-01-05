using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using EoE.Controlls;

namespace EoE.UI
{
	public class ControllerMenuItem : MonoBehaviour
	{
		private const float NAVIGATION_COOLDOWN = 0.2f;
		private static Dictionary<int, ControllerMenuItem> SelectedItemInLayer = new Dictionary<int, ControllerMenuItem>();
		private static int ActiveMenuLayer;
		private static float MenuNavigationCooldown;

		[SerializeField] private int menuLayer = 0;
		[SerializeField] private bool startSelected = false;
		[SerializeField] private UnityEvent upEvent = default;
		[SerializeField] private UnityEvent downEvent = default;
		[SerializeField] private UnityEvent rightEvent = default;
		[SerializeField] private UnityEvent leftEvent = default;
		public UnityEvent onSelectedEvent = default;
		public UnityEvent onDeSelectedEvent = default;
		[SerializeField] private UnityEvent onEnterEvent = default;
		[SerializeField] private UnityEvent onBackEvent = default;

		private bool isSelected;

		private void Start()
		{
			if (startSelected)
				Select();
		}

		private void Update()
		{
			if (!isSelected || (ActiveMenuLayer != menuLayer))
			{
				return;
			}
			else if (MenuNavigationCooldown > 0)
			{
				MenuNavigationCooldown -= Time.unscaledDeltaTime;
				return;
			}

			if (InputController.MenuUp.Pressed)
			{
				SetNavigationCooldown(NAVIGATION_COOLDOWN);
				upEvent.Invoke();
			}
			else if (InputController.MenuDown.Pressed)
			{
				SetNavigationCooldown(NAVIGATION_COOLDOWN);
				downEvent.Invoke();
			}
			else if (InputController.MenuRight.Pressed)
			{
				SetNavigationCooldown(NAVIGATION_COOLDOWN);
				rightEvent.Invoke();
			}
			else if (InputController.MenuLeft.Pressed)
			{
				SetNavigationCooldown(NAVIGATION_COOLDOWN);
				leftEvent.Invoke();
			}
			else if (InputController.MenuEnter.Down)
			{
				SetNavigationCooldown(NAVIGATION_COOLDOWN);
				onEnterEvent.Invoke();
			}
			else if (InputController.MenuBack.Down)
			{
				SetNavigationCooldown(NAVIGATION_COOLDOWN);
				onBackEvent.Invoke();
			}
		}
		public void Select()
		{
			if (SelectedItemInLayer.ContainsKey(menuLayer))
			{
				ControllerMenuItem curSelected = SelectedItemInLayer[menuLayer];
				if (curSelected == this)
				{
					if(menuLayer == ActiveMenuLayer)
						return;
				}
				else
				{
					if (curSelected != null)
					{
						curSelected.onDeSelectedEvent.Invoke();
						curSelected.isSelected = false;
					}

					SelectedItemInLayer[menuLayer] = this;
				}
			}
			else
			{
				SelectedItemInLayer.Add(menuLayer, this);
			}

			ActiveMenuLayer = menuLayer;
			isSelected = true;
			onSelectedEvent.Invoke();
		}
		public void SetNavigationCooldown(float amount)
		{
			MenuNavigationCooldown = amount;
		}
		private void OnDrawGizmosSelected()
		{
			RectTransform r = transform as RectTransform;
			if (!r)
				return;
			Vector2 pos = r.position;
			DrawForEvent(upEvent,		pos + new Vector2(r.rect.width / 3, r.rect.height / 2)		* r.lossyScale,	3, -2, Color.blue);
			DrawForEvent(downEvent,		pos + new Vector2(-r.rect.width / 3, -r.rect.height / 2)	* r.lossyScale,	-3, 2, Color.red);
			DrawForEvent(rightEvent,	pos + new Vector2(r.rect.width / 2, -r.rect.height / 3)		* r.lossyScale,	-2, -3, Color.green);
			DrawForEvent(leftEvent,		pos + new Vector2(-r.rect.width / 2, r.rect.height / 3)		* r.lossyScale,	2, 3, Color.yellow);
		}
		private void DrawForEvent(UnityEvent targetEvent, Vector2 lineStart, float xDiv, float yDiv, Color targetColor)
		{
			int count = targetEvent.GetPersistentEventCount();
			Gizmos.color = targetColor;

			for (int i = 0; i < count; i++)
			{
				Object target = targetEvent.GetPersistentTarget(i);
				if (!target)
					continue;

				GameObject objectTarget = (target as GameObject) ?? (target as MonoBehaviour).gameObject;
				if (!objectTarget)
					continue;

				RectTransform otherR = objectTarget.transform as RectTransform;
				if (otherR)
				{
					Vector2 lineEnd = (Vector2)otherR.position + new Vector2(otherR.rect.width / xDiv, otherR.rect.height / yDiv) * otherR.lossyScale;
					Gizmos.DrawLine(lineStart, lineEnd);
				}
			}
		}
	}
}