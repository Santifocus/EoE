using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(DropTable), true), CanEditMultipleObjects]
	public class DropTableEditor : Editor
	{
		[MenuItem("EoE/DropTable")]
		public static void CreateDropTable()
		{
			DropTable asset = CreateInstance<DropTable>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/DropTables/New Drop Tables.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Drop Table' at: Assets/Settings/DropTables/...");
		}

		private static bool tableOpen;
		private bool[] innerFoldouts;
		public override void OnInspectorGUI()
		{
			CustomInspector();
			if (isDirty)
			{
				isDirty = false;
				EditorUtility.SetDirty(target);
			}
		}
		protected virtual void CustomInspector() 
		{
			DropTable table = target as DropTable;

			if(table.PossibleDrops == null)
			{
				table.PossibleDrops = new DropTable.ItemDrop[0];
			}

			if(innerFoldouts == null)
			{
				innerFoldouts = new bool[table.PossibleDrops.Length];
			}

			Foldout(new GUIContent("Possible Drops"), ref tableOpen);

			if (!tableOpen)
				return;

			int curArraySize = table.PossibleDrops.Length;
			if(innerFoldouts.Length != curArraySize)
			{
				int oldSize = innerFoldouts.Length;
				bool[] newArray = new bool[curArraySize];
				for (int i = 0; i < curArraySize; i++)
				{
					if (i < oldSize)
						newArray[i] = innerFoldouts[i];
					else
						break;
				}

				innerFoldouts = newArray;
			}

			for (int i = 0; i < curArraySize; i++)
			{
				Foldout(new GUIContent("Drop Nr. " + (i + 1)), ref innerFoldouts[i], 1);

				if (innerFoldouts[i])
				{
					ObjectField(new GUIContent("Drop", "What should be dropped?"), ref table.PossibleDrops[i].Drop, 2);
					if (IntField(new GUIContent("Min Drop Amount", "How many drops should be spawned at least?"), ref table.PossibleDrops[i].MinDropAmount, 2))
					{
						if (table.PossibleDrops[i].MinDropAmount < 0)
							table.PossibleDrops[i].MinDropAmount = 0;
						
						if (table.PossibleDrops[i].MinDropAmount > table.PossibleDrops[i].MaxDropAmount)
							table.PossibleDrops[i].MaxDropAmount = table.PossibleDrops[i].MinDropAmount;
					}

					if (IntField(new GUIContent("Max Drop Amount", "How many drops should be spawned at max?"), ref table.PossibleDrops[i].MaxDropAmount, 2))
					{
						if (table.PossibleDrops[i].MaxDropAmount < 0)
							table.PossibleDrops[i].MaxDropAmount = 0;

						if (table.PossibleDrops[i].MaxDropAmount < table.PossibleDrops[i].MinDropAmount)
							table.PossibleDrops[i].MinDropAmount = table.PossibleDrops[i].MaxDropAmount;
					}
					if(FloatField(new GUIContent("Drop Chance", "What is the chance to drop this Drop? (0 = 0%; 1 = 100%)"), ref table.PossibleDrops[i].DropChance, 2))
					{
						if (table.PossibleDrops[i].DropChance < 0)
							table.PossibleDrops[i].DropChance = 0;
						if (table.PossibleDrops[i].DropChance > 1)
							table.PossibleDrops[i].DropChance = 1;
					}
				}

			}

			GUILayout.Space(10);
			int newArraySize = curArraySize;
			if(IntField("Size", ref newArraySize, 1) && newArraySize != curArraySize)
			{
				DropTable.ItemDrop[] newArray = new DropTable.ItemDrop[newArraySize];
				for (int i = 0; i < newArraySize; i++)
				{
					if (i < curArraySize)
						newArray[i] = table.PossibleDrops[i];
					else
						newArray[i] = new DropTable.ItemDrop();
				}

				table.PossibleDrops = newArray;
			}
		}
	}
}