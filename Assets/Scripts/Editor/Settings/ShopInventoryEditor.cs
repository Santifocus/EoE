using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(ShopInventory)), CanEditMultipleObjects]
	public class ShopInventoryEditor : ScriptableObjectEditor
	{
		private static bool OptionAreaOpen;
		private Queue<ShopItem> queuedRemovals = new Queue<ShopItem>();
		protected override void CustomInspector()
		{
			base.CustomInspector();
			ShopInventory settings = target as ShopInventory;

			DrawArray<ShopItem>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShopItems))), ref settings.ShopItems, serializedObject.FindProperty(nameof(settings.ShopItems)), DrawShopItem, ShopItemContentGetter, 0, true);
			DrawInFoldoutHeader(new GUIContent("Options"), ref OptionAreaOpen, DrawOptionsArea);
			RemoveQueuedRemovals();
		}
		private void DrawOptionsArea()
		{
			if (GUILayout.Button("Sort By ID"))
				SortByID();
			if (GUILayout.Button("Sort By Name"))
				SortByName();
			if (GUILayout.Button("Sort By Price"))
				SortByPrice();

			if (GUILayout.Button("Gather All Items"))
				GatherItems();
		}
		private void GatherItems()
		{
			string[] foundAssetsGUIDs = AssetDatabase.FindAssets("t:" + typeof(Item).Name.ToLower());
			List<Item> allFoundItems = new List<Item>(foundAssetsGUIDs.Length);
			for (int i = 0; i < foundAssetsGUIDs.Length; i++)
			{
				allFoundItems.Add((Item)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsGUIDs[i]), typeof(Item)));
			}

			ShopInventory settings = target as ShopInventory;

			settings.ShopItems = new ShopItem[allFoundItems.Count];

			for(int i = 0; i < settings.ShopItems.Length; i++)
			{
				settings.ShopItems[i] = new ShopItem()
				{
					InfinitePurchases = true,
					Item = allFoundItems[i],
					MaxPurchases = 0
				};
			}
			serializedObject.FindProperty(nameof(settings.ShopItems)).arraySize = settings.ShopItems.Length;
			isDirty = true;
		}
		private void SortByID()
		{
			ShopInventory settings = target as ShopInventory;
			List<ShopItem> shopItems = new List<ShopItem>(settings.ShopItems);
			RemoveNulls(ref shopItems);

			shopItems.Sort((x, y) => x.Item.SortingID.CompareTo(y.Item.SortingID));
			settings.ShopItems = shopItems.ToArray();
			isDirty = true;
		}
		private void SortByName()
		{
			ShopInventory settings = target as ShopInventory;
			List<ShopItem> shopItems = new List<ShopItem>(settings.ShopItems);
			RemoveNulls(ref shopItems);

			shopItems.Sort((x, y) => string.Compare(x.Item.ItemName.text, y.Item.ItemName.text));
			settings.ShopItems = shopItems.ToArray();
			isDirty = true;
		}
		private void SortByPrice()
		{
			ShopInventory settings = target as ShopInventory;
			List<ShopItem> shopItems = new List<ShopItem>(settings.ShopItems);
			RemoveNulls(ref shopItems);

			shopItems.Sort((x, y) => x.Item.ItemWorth.CompareTo(y.Item.ItemWorth));
			settings.ShopItems = shopItems.ToArray();
			isDirty = true;
		}
		private void RemoveNulls(ref List<ShopItem> list)
		{
			if(list == null)
			{
				list = new List<ShopItem>();
			}
			if (list.Count == 0)
				return;

			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == null || !list[i].Item)
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		private void DrawShopItem(GUIContent content, ShopItem settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
				return;

			bool changed = false;
			EditorGUILayout.BeginHorizontal();
			Foldout(content, property, offSet);

			//Draw the Item icon if it is available
			Rect rect = EditorGUILayout.GetControlRect(false, 24);
			rect.width = 24;
			rect.x *= 2;
			Texture itemTexture = settings.Item ? (settings.Item.ItemIcon ? settings.Item.ItemIcon.texture : null) : null;
			Texture icon = itemTexture ?? EditorGUIUtility.IconContent("CollabConflict").image;
			EditorGUI.DrawPreviewTexture(rect, icon, null, ScaleMode.ScaleToFit);

			if (GUILayout.Button("Up"))
			{
				changed = true;
				MoveUpItem(settings);
				return;
			}
			else if (GUILayout.Button("Down"))
			{
				changed = true;
				MoveDownItem(settings);
				return;
			}
			else if (GUILayout.Button("X"))
			{
				queuedRemovals.Enqueue(settings);
			}
			EditorGUILayout.EndHorizontal();

			if (changed)
				return;

			if (property.isExpanded)
			{
				GUILayout.Space(3);
				ObjectField<Item>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Item))), ref settings.Item, offSet + 1);
				if (settings.Item)
				{
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.InfinitePurchases))), ref settings.InfinitePurchases, offSet + 1);
					if (!settings.InfinitePurchases)
					{
						IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaxPurchases))), ref settings.MaxPurchases, offSet + 1);
					}
				}
			}
		}
		private GUIContent ShopItemContentGetter(int index)
		{
			ShopInventory settings = target as ShopInventory;
			ShopItem targetItem = settings.ShopItems[index];

			if(targetItem != null && targetItem.Item)
			{
				return new GUIContent(targetItem.Item.ItemName.text);
			}

			return new GUIContent("(Empty Slot)");
		}
		private void MoveUpItem(ShopItem item)
		{
			ShopInventory settings = target as ShopInventory;
			for (int i = 0; i < settings.ShopItems.Length; i++)
			{
				if (settings.ShopItems[i] == item)
				{
					if (i == 0)
						return;

					ShopItem other = settings.ShopItems[i - 1];
					settings.ShopItems[i - 1] = item;
					settings.ShopItems[i] = other;
				}
			}
			isDirty = true;
		}
		private void MoveDownItem(ShopItem item)
		{
			ShopInventory settings = target as ShopInventory;
			for (int i = 0; i < settings.ShopItems.Length; i++)
			{
				if (settings.ShopItems[i] == item)
				{
					if (i == (settings.ShopItems.Length - 1))
						return;

					ShopItem other = settings.ShopItems[i + 1];
					settings.ShopItems[i + 1] = item;
					settings.ShopItems[i] = other;
				}
			}
			isDirty = true;
		}
		private void RemoveQueuedRemovals()
		{
			ShopInventory settings = target as ShopInventory;

			List<ShopItem> shopItems = new List<ShopItem>(settings.ShopItems);
			while(queuedRemovals.Count > 0)
			{
				shopItems.Remove(queuedRemovals.Dequeue());
			}

			settings.ShopItems = shopItems.ToArray();
			isDirty = true;

			serializedObject.FindProperty(nameof(settings.ShopItems)).arraySize = settings.ShopItems.Length;
		}
	}
}
