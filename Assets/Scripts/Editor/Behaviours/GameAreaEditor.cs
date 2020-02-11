using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Information;
using EoE.Sounds;

namespace EoE.Behaviour
{
	[CustomEditor(typeof(GameArea)), CanEditMultipleObjects]
	public class GameAreaEditor : ObjectEditor
	{
		private static MusicController musicController;
		protected override void CustomInspector()
		{
			DrawDefaultInspector();
			DrawAreaMusicInfo();
		}
		private void DrawAreaMusicInfo()
		{
			GameArea settings = target as GameArea;
			if (!musicController)
			{
				musicController = FindObjectOfType<MusicController>();
			}

			if (musicController)
			{
				if(settings.targetMusicIndex < 0 || settings.targetMusicIndex >= musicController.MusicList.Length)
				{
					EditorGUILayout.HelpBox("Invalid Music Index", MessageType.Error);
				}
				else if (!musicController.MusicList[settings.targetMusicIndex])
				{
					EditorGUILayout.HelpBox("Target Music is Null", MessageType.Error);
				}
				else
				{
					Header("Target Music: " + musicController.MusicList[settings.targetMusicIndex].soundName, 0, false);
				}
			}
		}
	}
}
