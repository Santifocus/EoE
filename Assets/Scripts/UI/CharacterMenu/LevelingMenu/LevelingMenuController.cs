using EoE.Controlls;
using EoE.Entities;
using EoE.Information;
using UnityEngine;

namespace EoE.UI
{
	public class LevelingMenuController : CharacterMenuPage
	{
		//Inspector Variables
		[SerializeField] private float navigationCooldown = 0.2f;
		[SerializeField] private CMenuItem[] menuItems = default;

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
			if (!(menuItems[navigationIndex] is SkillPointStat))
				return;

			AcceptResetButton.PointTarget pointTarget = (int)(menuItems[navigationIndex] as SkillPointStat).targetStat < 3 ? AcceptResetButton.PointTarget.AttributePoints : AcceptResetButton.PointTarget.SkillPoints;

			for (int i = 0; i < menuItems.Length; i++)
			{
				AcceptResetButton confirmButton = menuItems[i] as AcceptResetButton;
				if (confirmButton && confirmButton.targetPoints == pointTarget)
				{
					navigationIndex = i;

					//By disabling and then enabling we can stop the component to call Update this tick
					//we dont want it to do a Update because otherwise it would isnatntly request a ApplyPoints
					menuItems[i].enabled = false;
					menuItems[i].enabled = true;

					menuItems[i].SelectMenuItem();
					break;
				}
			}
		}
		public void ResetAssignedSkillPoints(bool onAttributes)
		{
			if (onAttributes)
			{
				for (int i = 0; i < 3; i++)
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