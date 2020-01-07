using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(DropTable), true), CanEditMultipleObjects]
	public class DropTableEditor : ObjectSettingEditor
	{
		protected override void CustomInspector()
		{
			DropTable settings = target as DropTable;
			SerializedProperty dropTableArrayProperty = serializedObject.FindProperty(nameof(settings.PossibleDrops));
			DrawArray<DropTable.ItemDropData>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PossibleDrops))), ref settings.PossibleDrops, dropTableArrayProperty, DrawItemDropData, new GUIContent(". Drop"), 0, true);
		}
		private void DrawItemDropData(GUIContent content, DropTable.ItemDropData settings, SerializedProperty property, int offSet)
		{
			if(settings == null)
			{
				settings = new DropTable.ItemDropData();
				isDirty = true;
			}

			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				ObjectField(new GUIContent("Drop", "What should be dropped?"), ref settings.Drop, offSet + 1);
				if (IntField(new GUIContent("Min Drop Amount", "How many drops should be spawned at least?"), ref settings.MinDropAmount, offSet + 1))
				{
					if (settings.MinDropAmount < 0)
						settings.MinDropAmount = 0;

					if (settings.MinDropAmount > settings.MaxDropAmount)
						settings.MaxDropAmount = settings.MinDropAmount;
				}

				if (IntField(new GUIContent("Max Drop Amount", "How many drops should be spawned at max?"), ref settings.MaxDropAmount, offSet + 1))
				{
					if (settings.MaxDropAmount < 0)
						settings.MaxDropAmount = 0;

					if (settings.MaxDropAmount < settings.MinDropAmount)
						settings.MinDropAmount = settings.MaxDropAmount;
				}
				SliderField(new GUIContent("Drop Chance", "What is the chance to drop this Drop? (0 = 0%; 1 = 100%)"), ref settings.DropChance, 0, 1, offSet + 1);
			}
		}
	}
}