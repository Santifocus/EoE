using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EoE.UI
{
	public class QuitGameButton : CMenuItem
	{
		protected override void OnPress()
		{
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.MAIN_MENU_SCENE_INDEX);
		}
	}
}