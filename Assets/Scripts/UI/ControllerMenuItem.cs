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

			OpenSelfLayer();
			isSelected = true;
			onSelectedEvent.Invoke();
		}
		public void SetNavigationCooldown(float amount)
		{
			MenuNavigationCooldown = amount;
		}
		public void OpenSelfLayer()
		{
			ActiveMenuLayer = menuLayer;
		}
		public void PlayFeedback(bool succesSound)
		{
			Sounds.SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}
	}
}