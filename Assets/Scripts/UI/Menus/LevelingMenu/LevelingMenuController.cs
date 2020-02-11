using EoE.Controlls;
using EoE.Behaviour.Entities;
using EoE.Information;
using EoE.Sounds;
using TMPro;
using UnityEngine;

namespace EoE.UI
{
	public class LevelingMenuController : CharacterMenuPage
	{
		public static LevelingMenuController Instance { get; private set; }
		//Inspector Variables
		[Header("Skill Point Apply Settings")]
		[SerializeField] private Color remainingSkillpoints = Color.cyan;
		[SerializeField] private Color noSkillPointsLeft = Color.gray;
		[SerializeField] private TextMeshProUGUI availableAtributePoints = default;
		[SerializeField] private TextMeshProUGUI availableSkillPoints = default;

		[Space(5)]
		[SerializeField] private SkillPointStat[] skillPointApplier = default;

		public Color standardColor = Color.white;
		public Color notConfirmedChangeColor = Color.green;

		public Color recentlyChangedFlashColor = Color.red;
		public float recentlyChangedFlashTime = 0.2f;
		public int recentlyChangedFlashCount = 3;


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
		private int skillPoints
		{ get => Player.Instance.AvailableSkillPoints; set => Player.Instance.AvailableSkillPoints = value; }
		private int attributePoints
		{ get => Player.Instance.AvailableAtributePoints; set => Player.Instance.AvailableAtributePoints = value; }

		//Assigned Skillpoints
		private int[] assignedSkillPoints;
		private int assignedSkillPointCount;

		private int[] assignedAttributePoints;
		private int assignedAttributePointCount;
		protected override void Start()
		{
			Instance = this;
			assignedSkillPoints = new int[3]; //ATK, MGA, DEF
			assignedAttributePoints = new int[3]; //Health, Mana, Stamina
			base.Start();
		}
		protected override void ResetPage()
		{
			skillPointApplier[0].Select();
			UpdateDisplay();
			ResetAssignedSkillPoints(false);
		}
		public void ModifyAssignedSkillPoint(bool add)
		{
			TargetBaseStat stat = GetSelectedStat();
			bool toAttributePoints = (int)stat < 3;

			//First check if we have enought Points for the stat we want to increment
			if (add && (toAttributePoints ? assignedAttributePointCount == attributePoints : assignedSkillPointCount == skillPoints))
			{
				PlayFeedback(false);
				return;
			}

			//Then check if we try to remove a point but dont actually have one assigned
			if (!add && this[stat] == 0)
			{
				PlayFeedback(false);
				return;
			}

			int change = add ? 1 : -1;
			this[stat] += change;

			if (toAttributePoints)
				assignedAttributePointCount += change;
			else
				assignedSkillPointCount += change;

			UpdateDisplay();
			PlayFeedback(true);
		}
		private TargetBaseStat GetSelectedStat()
		{
			for(int i = 0; i < skillPointApplier.Length; i++)
			{
				if(skillPointApplier[i].isSelected)
				{
					return skillPointApplier[i].targetStat;
				}
			}

			return TargetBaseStat.Health;
		}
		public void ApplyAssignedPoints()
		{
			if (assignedAttributePointCount + assignedSkillPointCount == 0)
			{
				PlayFeedback(false);
				return;
			}
			attributePoints -= assignedAttributePointCount;
			for (int i = 0; i < 3; i++)
			{
				Player.Instance.LevelingPointsBuff.Effects[i].Amount += Player.PlayerSettings.LevelSettings[(TargetBaseStat)i] * assignedAttributePoints[i];
				assignedAttributePoints[i] = 0;
			}
			assignedAttributePointCount = 0;

			skillPoints -= assignedSkillPointCount;
			for (int i = 0; i < 3; i++)
			{
				Player.Instance.LevelingPointsBuff.Effects[i + 3].Amount += Player.PlayerSettings.LevelSettings[(TargetBaseStat)(i + 3)] * assignedSkillPoints[i];
				assignedSkillPoints[i] = 0;
			}
			assignedSkillPointCount = 0;

			Player.Instance.RecalculateBuffs();
			UpdateDisplay();

			PlayFeedback(true);
		}
		public void ResetAssignedSkillPoints(bool feedback = true)
		{
			if (assignedAttributePointCount + assignedSkillPointCount == 0)
			{
				if (feedback)
					PlayFeedback(false);
				return;
			}

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

			if (feedback)
				PlayFeedback(true);
		}
		private void UpdateDisplay()
		{
			availableAtributePoints.text = (attributePoints - assignedAttributePointCount).ToString();
			availableAtributePoints.color = (attributePoints - assignedAttributePointCount) > 0 ? remainingSkillpoints : noSkillPointsLeft;
			availableSkillPoints.text = (skillPoints - assignedSkillPointCount).ToString();
			availableSkillPoints.color = (skillPoints - assignedSkillPointCount) > 0 ? remainingSkillpoints : noSkillPointsLeft;

			for (int i = 0; i < skillPointApplier.Length; i++)
			{
				SkillPointStat target = skillPointApplier[i] as SkillPointStat;
				if (target == null)
					continue;

				target.UpdateNumbers();
			}
		}
		public void PlayFeedback(bool succesSound)
		{
			SoundManager.SetSoundState(succesSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}

		protected override void DeactivatePage()
		{
			gameObject.SetActive(false);
		}
	}
}