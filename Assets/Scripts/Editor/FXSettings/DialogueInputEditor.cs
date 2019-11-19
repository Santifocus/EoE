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
		private static bool PartsOpen;
		protected override void CustomInspector()
		{
			DialogueInput info = target as DialogueInput;
			if (info.parts == null)
				info.parts = new DialogueInput.DialoguePart[0];

			FoldoutHeader(new GUIContent("Parts"), ref PartsOpen);
			if (PartsOpen)
			{
				for (int i = 0; i < info.parts.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					ColorField(new GUIContent("Part " + i), ref info.parts[i].textColor);
					StringField("", ref info.parts[i].text);
					EditorGUILayout.EndHorizontal();
				}

				int newSize = info.parts.Length;
				if(IntField("Size", ref newSize))
				{
					DialogueInput.DialoguePart[] newArray = new DialogueInput.DialoguePart[newSize];

					for(int i = 0; i < newSize; i++)
					{
						if (i < info.parts.Length)
							newArray[i] = info.parts[i];
						else
							break;
					}
					info.parts = newArray;
				}
			}
			EndFoldoutHeader();
		}
	}
}