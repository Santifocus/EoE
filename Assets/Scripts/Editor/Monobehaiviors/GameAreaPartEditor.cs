using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;
using EoE.Information;

namespace EoE.Behaivior
{
	[CustomEditor(typeof(GameAreaPart)), CanEditMultipleObjects]
	public class GameAreaPartEditor : ObjectEditor
	{
		protected override void CustomInspector()
		{
			DrawDefaultInspector();
			DrawAreaSettings();
		}
		private void DrawAreaSettings()
		{
			Header("Area Settings");
			GameAreaPart settings = target as GameAreaPart;
			EnumField<GameAreaPart.AreaShape>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.areaShape))), ref settings.areaShape);

			if(settings.areaShape == GameAreaPart.AreaShape.Sphere)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.sphereRadius))), ref settings.sphereRadius, 1);
			}
			else
			{
				Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.boxSize))), ref settings.boxSize, 1);
			}
			GUILayout.Space(10);
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(GameAreaPart.DrawOpaqueArea))), ref GameAreaPart.DrawOpaqueArea);
		}
	}
}
