using EoE.Behaviour.Entities;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Other
{
	public class CheatController : MonoBehaviour
	{
		private static readonly KeyCode TeleportKey = KeyCode.T;
		private static readonly KeyCode InvincibleToggleKey = KeyCode.I;
		private static readonly KeyCode LevelUpKey = KeyCode.L;
		private static readonly KeyCode MoneyIncreaseKey = KeyCode.M;
		private static readonly KeyCode PeacefullModeKey = KeyCode.P;

		private static readonly int MoneyIncreaseAmount = 100;

		private static bool invincibleToggled = false;
		private static bool peacefullModeActive = false;

		[SerializeField] private Notification teleportNotificication = default;
		[SerializeField] private Notification invincibleNotificication = default;
		[SerializeField] private Notification levelUpNotificication = default;
		[SerializeField] private Notification moneyIncreaseNotificication = default;
		[SerializeField] private Notification peacefullModeNotification = default;
		[SerializeField] private GameObject entitieStorage = default;

		private void Update()
		{
			if (Input.GetKeyDown(TeleportKey))
			{
				Player.Instance.charController.enabled = false;
				Player.Instance.transform.position = transform.position;
				Player.Instance.charController.enabled = true;

				FXManager.ExecuteFX(teleportNotificication, Player.Instance.transform, true);
			}

			if (Input.GetKeyDown(InvincibleToggleKey))
			{
				if (invincibleToggled)
					Player.Instance.Invincibilities--;
				else
					Player.Instance.Invincibilities++;

				invincibleToggled = !invincibleToggled;
				FXManager.ExecuteFX(invincibleNotificication, Player.Instance.transform, true);
			}

			if (Input.GetKeyDown(LevelUpKey))
			{
				Player.Instance.AddExperience(Player.Instance.RequiredExperienceForLevel - Player.Instance.TotalExperience);
				FXManager.ExecuteFX(levelUpNotificication, Player.Instance.transform, true);
			}

			if (Input.GetKeyDown(MoneyIncreaseKey))
			{
				Player.Instance.ChangeCurrency(MoneyIncreaseAmount);
				FXManager.ExecuteFX(moneyIncreaseNotificication, Player.Instance.transform, true);
			}
			if (Input.GetKeyDown(PeacefullModeKey))
			{
				peacefullModeActive = !peacefullModeActive;
				entitieStorage.SetActive(!peacefullModeActive);
				for(int i = 0; i < Entity.AllEntities.Count; i++)
				{
					if (Entity.AllEntities[i] is Enemy)
						Entity.AllEntities[i].StatDisplay.gameObject.SetActive(!peacefullModeActive);
				}
				FXManager.ExecuteFX(peacefullModeNotification, Player.Instance.transform, true);
			}
		}
	}
}