﻿using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using UnityEngine;
using TMPro;

namespace EoE.UI
{
	public class LevelingMenuController : CharacterMenuPage
	{
		//Inspector Variables
		[SerializeField] private float navigationCooldown = 0.2f;
		[SerializeField] private CMenuItem[] menuItems = default;

		[SerializeField] private Color remainingSkillpoints = Color.cyan;
		[SerializeField] private Color noSkillPointsLeft = Color.gray;
		[SerializeField] private TextMeshProUGUI availableAtributePoints = default;
		[SerializeField] private TextMeshProUGUI availableSkillPoints = default;

		//Getter Helpers
		public int this[TargetBaseStat stat]
		{
			get
			{
				int index = (int)stat;
				if (index >= 3)
					return assignedSkillPoints[index - 3];
				else
					return assignedAttributePoints[index];
			}
			set
			{
				int index = (int)stat;
				if (index >= 3)
					assignedSkillPoints[index - 3] = value;
				else
					assignedAttributePoints[index] = value;
			}
		}
		public static LevelingMenuController Instance { get; private set; }
		private PlayerSettings baseData => Player.PlayerSettings;
		private Buff skillBuff => Player.LevelingPointsBuff;
		private int skillPoints
		{ get => Player.AvailableSkillPoints; set => Player.AvailableSkillPoints = value; }
		private int attributePoints
		{ get => Player.AvailableAtributePoints; set => Player.AvailableAtributePoints = value; }

		//Assigned Skillpoints
		private int[] assignedSkillPoints;
		private int assignedSkillPointCount;

		private int[] assignedAttributePoints;
		private int assignedAttributePointCount;

		private int navigationIndex;
		private float curNavigationCooldown;
		protected override void Start()
		{
			base.Start();
			Instance = this;
			assignedSkillPoints = new int[3]; //ATK, MGA, DEF
			assignedAttributePoints = new int[3]; //Health, Mana, Endurance
		}
		protected override void ResetPage()
		{
			curNavigationCooldown = 0;
			navigationIndex = 0;
			if(menuItems.Length > 0)
				menuItems[navigationIndex].SelectMenuItem();
			UpdateDisplay();
		}
		private void Update()
		{
			if (curNavigationCooldown > 0)
				curNavigationCooldown -= Time.unscaledDeltaTime;

			if (InputController.MenuDown.Down || (InputController.MenuDown.Active && curNavigationCooldown <= 0))
			{
				curNavigationCooldown = navigationCooldown;

				navigationIndex++;
				if (navigationIndex == menuItems.Length)
					navigationIndex -= menuItems.Length;

				menuItems[navigationIndex].SelectMenuItem();
			}
			else if (InputController.MenuUp.Down || (InputController.MenuUp.Active && curNavigationCooldown <= 0))
			{
				curNavigationCooldown = navigationCooldown;

				navigationIndex--;
				if (navigationIndex == -1)
					navigationIndex += menuItems.Length;

				menuItems[navigationIndex].SelectMenuItem();
			}
		}
		public bool ModifyAssignedSkillPoint(bool add, TargetBaseStat stat)
		{
			bool toAttributePoints = (int)stat < 3;

			//First check if we have enought Points for the stat we want to increment
			if (add && (toAttributePoints ? assignedAttributePointCount == attributePoints : assignedSkillPointCount == skillPoints))
			{
				InputFeedback();
				return false;
			}

			//Then check if we try to remove a point but dont actually have one assigned
			if (!add && this[stat] == 0)
			{
				InputFeedback();
				return false;
			}

			int change = add ? 1 : -1;
			this[stat] += change;

			if (toAttributePoints)
				assignedAttributePointCount += change;
			else
				assignedSkillPointCount += change;

			UpdateDisplay();
			InputFeedback();
			return true;
		}
		public void ApplyAssignedPoints()
		{
			attributePoints -= assignedAttributePointCount;
			for (int i = 0; i < 3; i++)
			{
				skillBuff.Effects[i].Amount += baseData.LevelSettings[(TargetBaseStat)i] * assignedAttributePoints[i];
				assignedAttributePoints[i] = 0;
			}
			assignedAttributePointCount = 0;

			skillPoints -= assignedSkillPointCount;
			for (int i = 0; i < 3; i++)
			{
				skillBuff.Effects[i + 3].Amount += baseData.LevelSettings[(TargetBaseStat)(i + 3)] * assignedSkillPoints[i];
				assignedSkillPoints[i] = 0;
			}
			assignedSkillPointCount = 0;

			Player.Instance.RecalculateBuffs();
			UpdateDisplay();
		}
		public void GotoConfirmation()
		{
			for(int i = 0; i < menuItems.Length; i++)
			{
				if(menuItems[i] is AcceptResetButton)
				{
					menuItems[i].SelectMenuItem();
					break;
				}
			}
		}
		public void ResetAssignedSkillPoints()
		{
			for (int i = 0; i < 3; i++)
			{
				assignedAttributePoints[i] = 0;
			}
			assignedAttributePointCount = 0;
			for (int i = 0; i < 3; i++)
			{
				assignedSkillPoints[i] = 0;
			}
			assignedSkillPointCount = 0;

			UpdateDisplay();
		}
		private void InputFeedback()
		{

		}
		private void UpdateDisplay()
		{
			availableAtributePoints.text = (attributePoints - assignedAttributePointCount).ToString();
			availableAtributePoints.color = (attributePoints - assignedAttributePointCount) > 0 ? remainingSkillpoints : noSkillPointsLeft;
			availableSkillPoints.text = (skillPoints - assignedSkillPointCount).ToString();
			availableSkillPoints.color = (skillPoints - assignedSkillPointCount) > 0 ? remainingSkillpoints : noSkillPointsLeft;

			for (int i = 0; i < menuItems.Length; i++)
			{
				SkillPointStat target = menuItems[i] as SkillPointStat;
				if (target == null)
					continue;

				target.UpdateNumbers();
			}
		}

		protected override void DeactivatePage()
		{
			gameObject.SetActive(false);
		}
	}
}