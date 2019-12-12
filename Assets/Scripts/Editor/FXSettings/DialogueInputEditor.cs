using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EoE.UI;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(DialogueInput), true), CanEditMultipleObjects]
	public class DialogueInputEditor : ObjectSettingEditor
	{
		private static bool PartsSettingsOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Parts Settings"), ref PartsSettingsOpen, PartsSettingsArea);
		}
		private void PartsSettingsArea()
		{
			DialogueInput settings = target as DialogueInput;

			if (settings.parts == null)
				settings.parts = new DialogueInput.DialoguePart[0];

			for (int i = 0; i < settings.parts.Length; i++)
			{
				EditorGUILayout.BeginHorizontal();
				ColorField(new GUIContent("Part " + i), ref settings.parts[i].textColor);
				StringField("", ref settings.parts[i].text);
				EditorGUILayout.EndHorizontal();
			}

			int newSize = settings.parts.Length;
			if (IntField("Size", ref newSize))
			{
				DialogueInput.DialoguePart[] newArray = new DialogueInput.DialoguePart[newSize];

				for (int i = 0; i < newSize; i++)
				{
					if (i < settings.parts.Length)
						newArray[i] = settings.parts[i];
					else
						break;
				}
				settings.parts = newArray;
			}
		}
	}
}