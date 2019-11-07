using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EoE.Controlls;
using EoE.Information;
using EoE.Entities;

namespace EoE.UI
{
	public class LevelingMenuController : CharacterMenuPage
	{
		//Inspector Variables
		[SerializeField] private float navigationCooldown = 0.2f;
		[SerializeField] private LevelingMenuComponent[] components = default;

		//Getter Helpers
		public int this[TargetStat stat]
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
			components[navigationIndex].SelectComponent();
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
				if (navigationIndex == components.Length)
					navigationIndex -= components.Length;

				components[navigationIndex].SelectComponent();
			}
			else if (InputController.MenuUp.Down || (InputController.MenuUp.Active && curNavigationCooldown <= 0))
			{
				curNavigationCooldown = navigationCooldown;

				navigationIndex--;
				if (navigationIndex == -1)
					navigationIndex += components.Length;

				components[navigationIndex].SelectComponent();
			}
		}
		public bool ModifyAssignedSkillPoint(bool add, TargetStat stat)
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
		public void ApplyAssignedPoints(bool onAttributes)
		{
			if (onAttributes)
			{
				attributePoints -= assignedAttributePointCount;
				for (int i = 0; i < 3; i++)
				{
					skillBuff.Effects[i].Amount += baseData.LevelSettings[(TargetStat)i] * assignedAttributePoints[i];
					assignedAttributePoints[i] = 0;
				}
				assignedAttributePointCount = 0;
			}
			else
			{
				skillPoints -= assignedSkillPointCount;
				for (int i = 0; i < 3; i++)
				{
					skillBuff.Effects[i + 3].Amount += baseData.LevelSettings[(TargetStat)(i + 3)] * assignedSkillPoints[i];
					assignedSkillPoints[i] = 0;
				}
				assignedSkillPointCount = 0;
			}

			Player.Instance.RecalculateBuffs();
			UpdateDisplay();
		}
		public void GotoConfirmation()
		{
			if (!(components[navigationIndex] is SkillPointStat))
				return;

			AcceptResetButton.PointTarget pointTarget = (int)(components[navigationIndex] as SkillPointStat).targetStat < 3 ? AcceptResetButton.PointTarget.AttributePoints : AcceptResetButton.PointTarget.SkillPoints;

			for(int i = 0; i < components.Length; i++)
			{
				AcceptResetButton confirmButton = components[i] as AcceptResetButton;
				if (confirmButton && confirmButton.targetPoints == pointTarget)
				{
					navigationIndex = i;

					//By disabling and then enabling we can stop the component to call Update this tick
					//we dont want it to do a Update because otherwise it would isnatntly request a ApplyPoints
					components[i].enabled = false;
					components[i].enabled = true;

					components[i].SelectComponent();
					break;
				}
			}
		}
		public void ResetAssignedSkillPoints(bool onAttributes)
		{
			if (onAttributes)
			{
				for(int i = 0; i < 3; i++)
				{
					assignedAttributePoints[i] = 0;
				}
				assignedAttributePointCount = 0;
			}
			else
			{
				for (int i = 0; i < 3; i++)
				{
					assignedSkillPoints[i] = 0;
				}
				assignedSkillPointCount = 0;
			}
			UpdateDisplay();
		}
		private void InputFeedback()
		{

		}
		private void UpdateDisplay()
		{
			for(int i = 0; i < components.Length; i++)
			{
				SkillPointStat target = components[i] as SkillPointStat;
				if (target == null)
					continue;

				target.UpdateNumbers();
			}
		}
	}
}