using EoE.Information;
using UnityEngine;

namespace EoE.Combatery
{
	public class ComboSet : ScriptableObject
	{
		//Color
		public Gradient StandardTextColor = new Gradient();
		public float ColorScrollSpeed = 1;

		//Default settings
		public float TextPunch = 20;
		public float PunchResetSpeed = 0.5f;

		//Combo data
		public ComboInfo[] ComboData = new ComboInfo[0];
	}
	[System.Serializable]
	public class ComboInfo
	{
		public int RequiredComboCount = 1;
		public ComboEffect Effect = new ComboEffect();
	}
	[System.Serializable]
	public class ComboEffect
	{
		//Text settings
		public bool OverrideTextColor = false;
		public Gradient TextColor = new Gradient();
		public float ColorScrollSpeed = 1;

		public bool OverrideTextPunch = false;
		public float TextPunch = 30;
		public float PunchResetSpeed = 0.5f;

		//Effect settings
		public EffectSingle EffectOnTarget;
		public EffectAOE EffectAOE;

		public HealTargetInfo[] HealEffects = new HealTargetInfo[0];
		public CustomFXObject[] EffectsTillComboEnds = new CustomFXObject[0];
		public Buff[] BuffsTillComboEnds = new Buff[0];
	}
}