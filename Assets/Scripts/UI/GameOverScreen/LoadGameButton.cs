﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EoE.UI
{
	public class LoadGameButton : CMenuItem
	{
		protected override void OnPress()
		{
			SceneManager.LoadScene(ConstantCollector.GAME_SCENE_INDEX);
		}
	}
}