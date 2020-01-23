using EoE.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class PlayerSettings : EntitySettings
	{
		//Camera Settings
		public float CameraToPlayerDistance = 7;
		public float CameraYLerpSpeed = 5;
		public float CameraYLerpSringStiffness = 0.2f;
		public Vector3 CameraAnchorOffset = new Vector3(0, 1.6f, 0);
		public Vector3 CameraAnchorOffsetWhenTargeting = new Vector3(0, 1.6f, 0);
		public Vector2 CameraRotationPower = new Vector2(200, 75);
		public float CameraRotationSpeed = 10;
		public Vector2 CameraVerticalAngleClamps = new Vector2(-50, 50);
		public float CameraExtraZoomOnVertical = 0.5f;
		public float MaxEnemyTargetingDistance = 100;
		public Vector2 CameraClampsWhenTargeting = new Vector2(-30, 30);
		public float CameraBaseFOV = 60;

		//Endurance Settings
		public float Endurance = 100;
		public bool DoEnduranceRegen = true;
		public float EnduranceRegen = 6;
		public float EnduranceRegenInCombatMultiplier = 0.5f;
		public float EnduranceAfterUseCooldown = 3;
		public float EnduranceRegenAfterUseMultiplier = 0.5f;

		//Endurance Costs
		public float JumpEnduranceCost = 4;
		public float RunEnduranceCost = 3;

		//Jumping
		public Vector3 JumpPower = new Vector3(0, 10, 0);
		public float JumpImpulsePower = 2.5f;
		public float JumpBackwardMultiplier = 0.65f;

		//Dashing
		public float DashPower = 1;
		public float DashDuration = 0.2f;
		public float DashModelExistTime = 0.4f;
		public float DashCooldown = 0.5f;
		public float DashEnduranceCost = 20;
		public Material DashModelMaterial = null;

		//Shielding
		public ShieldData ShieldSettings = default;

		//IFrames
		public bool InvincibleAfterHit = true;
		public float InvincibleAfterHitTime = 0.3f;
		public Color InvincibleModelFlashColor = Color.white;
		public float InvincibleModelFlashDelay = 0.4f;
		public float InvincibleModelFlashTime = 0.4f;

		//Inventory
		public ItemGiveInfo[] StartItems = new ItemGiveInfo[0];
		public int InventorySize = 24;

		//Animation
		public float MaxModelTilt = 10;
		public float SideTurnSpringLerpSpeed = 13;
		public float SideTurnLerpSpringStiffness = 0.1f;
		public float WalkAnimationLerpSpeed = 4;
		public float AnimationWalkSpeedDivider = 10;

		public float BodyTurnHorizontalClamp = 30;
		public float BodyTurnWeight = 0.5f;
		public Vector4 HeadLookAngleClamps = new Vector4(60, 40, 60, 30);
		public float LookLerpSpeed = 4;
		public float LookLerpSpringStiffness = 0.05f;

		///FX
		public FXObject[] EffectsOnCombatStart = default;
		public FXObject[] EffectsOnUltimateCharged = default;
		public FXObject[] EffectsWhileUltimateCharged = default;
		public FXObject[] EffectsOnLevelup = default;

		//On Player do attack
		public FXObject[] EffectsOnCauseDamage = default;
		public FXObject[] EffectsOnCauseCrit = default;
		public FXObject[] EffectsOnEnemyKilled = default;

		//Player Movement
		public FXObject[] EffectsWhileWalk = default;
		public FXObject[] EffectsWhileRun = default;
		public FXObject[] EffectsOnJump = default;
		public float PlayerLandingVelocityThreshold = 5f;
		public FXObject[] EffectsOnPlayerLanding = default;
		public FXObject[] EffectsOnPlayerDash = default;

		//On Player receive attack
		public FXObject[] EffectsOnReceiveDamage = default;
		public FXObject[] EffectsOnReceiveKnockback = default;
		public float EffectsHealthThreshold = 0.3f;
		public FXObject[] EffectsWhileHealthBelowThreshold = default;

	}
	[System.Serializable]
	public class ItemGiveInfo
	{
		public Item Item;
		public int ItemCount = 1;
		public bool ForceEquip;

		public void GiveToPlayer()
		{
			if (!Item)
				return;

			//Add it to the inventory
			List<int> targetSlots = Player.Instance.Inventory.AddItem(new InventoryItem(Item, ItemCount));

			//Force equipp
			if (ForceEquip)
			{
				InventoryItem targetItem = Player.Instance.Inventory[targetSlots[targetSlots.Count - 1]];
				targetItem.isEquiped = true;
				//Find out what slot it belongs to, if it is a spell / normal item we try
				//to put it in a open slot, if all slots are filled we put it in the first
				if (Item is WeaponItem)
				{
					Player.Instance.EquipedWeapon = targetItem;
					targetItem.isEquiped = true;
					targetItem.data.Equip(targetItem, Player.Instance);
				}
				else if (Item is ArmorItem)
				{
					Player.Instance.EquipedArmor = targetItem;
					targetItem.isEquiped = true;
					targetItem.data.Equip(targetItem, Player.Instance);
				}
				else if (Item is ActivationCompoundItem)
				{
					bool added = false;

					//Try to find a slot that is null and put the item there
					for (int j = 0; j < Player.Instance.SelectableActivationCompoundItems.Length; j++)
					{
						if (Player.Instance.SelectableActivationCompoundItems[j] == null)
						{
							Player.Instance.SelectableActivationCompoundItems[j] = targetItem;
							Player.Instance.SelectableActivationCompoundItems[j].isEquiped = true;
							added = true;
							if (j == 0)
							{
								Player.Instance.SelectableActivationCompoundItems[j].data.Equip(targetItem, Player.Instance);
							}
							break;
						}
					}
					//couldnt find a null slot, put it in the first one, (just a fallback)
					if (!added && Player.Instance.SelectableActivationCompoundItems.Length > 0)
					{
						Player.Instance.SelectableActivationCompoundItems[0] = targetItem;
						Player.Instance.SelectableActivationCompoundItems[0].isEquiped = true;
						Player.Instance.SelectableActivationCompoundItems[0].data.Equip(targetItem, Player.Instance);
					}
				}
				else
				{
					bool added = false;

					//Try to find a slot that is null and put the item there
					for (int j = 0; j < Player.Instance.SelectableItems.Length; j++)
					{
						if (Player.Instance.SelectableItems[j] == null)
						{
							Player.Instance.SelectableItems[j] = targetItem;
							Player.Instance.SelectableItems[j].isEquiped = true;
							added = true;
							if (j == 0)
							{
								Player.Instance.SelectableItems[j].data.Equip(targetItem, Player.Instance);
							}
							break;
						}
					}
					//couldnt find a null slot, put it in the first one, (just a fallback)
					if (!added && Player.Instance.SelectableItems.Length > 0)
					{
						Player.Instance.SelectableItems[0] = targetItem;
						Player.Instance.SelectableItems[0].isEquiped = true;
						Player.Instance.SelectableItems[0].data.Equip(targetItem, Player.Instance);
					}
				}
			}
		}
	}
}