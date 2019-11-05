using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EoE.Controlls;
using EoE.Information;
using EoE.Entities;

namespace EoE.UI
{
	public class LevelingMenuController : CharacterMenuPage
	{
		//Base
		//Health
		//Mana
		//Endurance

		//Special
		//AttackDamage
		//MagicPower
		//Defense

		private PlayerSettings baseData => Player.PlayerSettings;
		private Buff skillBuff => Player.LevelingSkillPointsBuff;
		private int bPoints => Player.AvailableBaseSkillPoints;
		private int sPoints => Player.AvailableSpecialSkillPoints;

		protected override void ResetPage()
		{

		}
	}
}