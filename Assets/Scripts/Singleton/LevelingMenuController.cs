using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EoE.Controlls;
using EoE.Information;
using EoE.Entities;

namespace EoE.UI
{
	public class LevelingMenuController : MonoBehaviour
	{
		//Health
		//Mana
		//AttackDamage
		//MagicPower
		//Endurance

		private PlayerSettings baseData => Player.PlayerSettings;
		private Buff levelingBuff => Player.LevelingBaseBuff;
		private Buff skillBuff => Player.LevelingBaseBuff;
	}
}