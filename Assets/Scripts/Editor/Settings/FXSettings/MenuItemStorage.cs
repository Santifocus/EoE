using EoE.Combatery;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace EoE.Information
{
	public static class MenuItemStorage
	{
		//Items
		[MenuItem("EoE/Item/ConsumableItem")] public static void CreateBuffItem() => AssetCreator<ConsumableItem>("Settings", "Items", "ConsumableItems");
		[MenuItem("EoE/Item/WeaponItem")] public static void CreateWeaponItem() => AssetCreator<WeaponItem>("Settings", "Items", "WeaponItems");
		[MenuItem("EoE/Item/ActivationCompoundItem")] public static void CreateActivationCompoundItem() => AssetCreator<ActivationCompoundItem>("Settings", "Items", "ActivationCompoundItems");
		[MenuItem("EoE/Item/ArmorItem")] public static void CreateArmorItem() => AssetCreator<ArmorItem>("Settings", "Items", "ArmorItems");

		//VFX
		[MenuItem("EoE/FX/ScreenShake")] public static void CreateScreenShake() => AssetCreator<ScreenShake>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenBlur")] public static void CreateScreenBlur() => AssetCreator<ScreenBlur>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenBorderColor")] public static void CreateScreenBorderColor() => AssetCreator<ScreenBorderColor>("Settings", "FX");
		[MenuItem("EoE/FX/ScreenTint")] public static void CreateScreenTint() => AssetCreator<ScreenTint>("Settings", "FX");
		[MenuItem("EoE/FX/ControllerRumble")] public static void CreateControllerRumble() => AssetCreator<ControllerRumble>("Settings", "FX");
		[MenuItem("EoE/FX/TimeDilation")] public static void CreateTimeDilation() => AssetCreator<TimeDilation>("Settings", "FX");
		[MenuItem("EoE/FX/ParticleEffect")] public static void CreateParticleEffect() => AssetCreator<ParticleEffect>("Settings", "FX");
		[MenuItem("EoE/FX/CameraFOVWarp")] public static void CreateCameraFOVWarp() => AssetCreator<CameraFOVWarp>("Settings", "FX");
		[MenuItem("EoE/FX/Dialogue")] public static void CreateDialogu() => AssetCreator<Dialogue>("Settings", "FX");
		[MenuItem("EoE/FX/Notification")] public static void CreateNotification() => AssetCreator<Notification>("Settings", "FX");
		[MenuItem("EoE/FX/CustomUI")] public static void CreateCustomUI() => AssetCreator<CustomUI>("Settings", "FX");

		//SFX
		[MenuItem("EoE/FX/Sound/Base")] public static void CreateSound() => AssetCreator<Sounds.Sound>("SFX", "SoundBases");
		[MenuItem("EoE/FX/Sound/Effect")] public static void CreateSoundEffect() => AssetCreator<SoundEffect>("Settings", "FX");

		//Combat
		[MenuItem("EoE/Combat/Object/BaseObject")] public static void CreateBaseCombatObject() => AssetCreator<CombatObject>("Settings", "Combat");
		[MenuItem("EoE/Combat/Object/Weapon")] public static void CreateWeapon() => AssetCreator<Weapon>("Settings", "Combat", "Weapons");
		[MenuItem("EoE/Combat/Object/ActivationCompound")] public static void CreateActivationCompound() => AssetCreator<ActivationCompound>("Settings", "Combat", "ActivationCompounds");
		[MenuItem("EoE/Combat/ProjectileData")] public static void CreateProjectileData() => AssetCreator<ProjectileData>("Settings", "Combat");
		[MenuItem("EoE/Combat/ShieldData")] public static void CreateShieldData() => AssetCreator<ShieldData>("Settings", "Combat");
		[MenuItem("EoE/Combat/Effect/EffectSingle")] public static void CreateEffectSingle() => AssetCreator<EffectSingle>("Settings", "Combat");
		[MenuItem("EoE/Combat/Effect/EffectAOE")] public static void CreateEffectAOE() => AssetCreator<EffectAOE>("Settings", "Combat");
		[MenuItem("EoE/Combat/Effect/RemenantsData")] public static void CreateRemenantsData() => AssetCreator<RemenantsData>("Settings", "Combat");
		[MenuItem("EoE/Combat/Physical/ComboSet")] public static void CreateComboSet() => AssetCreator<ComboSet>("Settings", "Combat", "Weapons", "ComboSets");
		[MenuItem("EoE/Combat/Physical/Ultimate/Basic")] public static void CreateBasicUltimate() => AssetCreator<BasicUltimate>("Settings", "Combat", "Weapons", "Ultimates");
		[MenuItem("EoE/Combat/Physical/Ultimate/Attack")] public static void CreateAttackUltimate() => AssetCreator<AttackUltimate>("Settings", "Combat", "Weapons", "Ultimates");

		//Other
		[MenuItem("EoE/Buff")] public static void CreateBuff() => AssetCreator<Buff>("Settings", "Buffs");
		[MenuItem("EoE/DropTable")] public static void CreateDropTable() => AssetCreator<DropTable>("Settings", "EntitieSettings", "DropTables");
		[MenuItem("EoE/LevelingSettings")] public static void CreateLevelingSettings() => AssetCreator<LevelingSettings>("Settings", "EntitieSettings", "LevelingSettings");
		[MenuItem("EoE/ShopInventory")] public static void CreateShopInventory() => AssetCreator<ShopInventory>("Settings", "InteractableSettings");
		[MenuItem("EoE/ConditionObject")] public static void CreateConditionObject() => AssetCreator<ConditionObject>("Settings", "ConditionObjects");

		//Data Managers
		[MenuItem("EoE/DataManagement/Collect Items")]
		public static void CollectItemData()
		{
			string[] itemCollectorGUID = AssetDatabase.FindAssets("t:ItemCollector");
			if (itemCollectorGUID.Length == 0)
			{
				ItemCollector newCollector = AssetCreator<ItemCollector>("Settings", "Data Collection");
				newCollector.CollectData();
			}
			else if (itemCollectorGUID.Length > 1)
			{
				Debug.Log("Found more then 1 Item collector... Deleting all and creating new one.");
				for (int i = 0; i < itemCollectorGUID.Length; i++)
				{
					AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(itemCollectorGUID[i]));
				}
				ItemCollector newCollector = AssetCreator<ItemCollector>("Settings", "Data Collection");
				newCollector.CollectData();
			}
			else
			{
				ItemCollector itemCollector = (ItemCollector)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(itemCollectorGUID[0]), typeof(ItemCollector));
				itemCollector.CollectData();
				EditorUtility.SetDirty(itemCollector);
			}
		}
		[MenuItem("EoE/DataManagement/Consistant Item Names")]
		public static void ConsistantItemNames()
		{
			string[] itemGUIDs = AssetDatabase.FindAssets("t:Item");
			for (int i = 0; i < itemGUIDs.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(itemGUIDs[i]);
				Item tItem = (Item)AssetDatabase.LoadAssetAtPath(path, typeof(Item));
				string newName = tItem.ItemName.text.Replace(" ", "").Replace("_", "") + "Item";
				AssetDatabase.RenameAsset(path, newName);
				EditorUtility.SetDirty(tItem);
			}
		}
		[MenuItem("EoE/DataManagement/Clean Object Names")]
		public static void CleanObjectNames()
		{
			bool onlyAllowLocalNamespace = true;
			string[] textToReplaceWithSpaces = new string[] { "_", ".", ",", "SFX", "FX", "Effect", "Particles", "Particle" };

			System.Type typeToClean = typeof(Sounds.Sound);
			string[] GUIDs = AssetDatabase.FindAssets("t:" + typeToClean.Name, new[] { "Assets/Settings/FX" });
			Debug.Log("Found: " + GUIDs.Length + " targeted objects");

			string addToEnd = "";
			for (int i = 0; i < GUIDs.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
				Object obj = (Object)AssetDatabase.LoadAssetAtPath(path, typeof(Object));
				System.Type objType = obj.GetType();

				if (onlyAllowLocalNamespace && !(objType).Namespace.StartsWith("EoE"))
					continue;

				string spacedName = obj.name;
				for(int j = 0; j < textToReplaceWithSpaces.Length; j++)
				{
					spacedName = spacedName.Replace(textToReplaceWithSpaces[j], " ");
				}
				string pascalCasedName = "";
				bool lastWasSpace = true;
				for (int j = 0; j < spacedName.Length; j++)
				{
					if (spacedName[j] == ' ')
					{
						lastWasSpace = true;
					}
					else if (lastWasSpace)
					{
						pascalCasedName += spacedName[j].ToString().ToUpper();
						lastWasSpace = false;
					}
					else
					{
						pascalCasedName += spacedName[j];
					}
				}

				string newName = pascalCasedName.Replace(" ", "");

				string toAdd = GetEnding(objType);
				if(!newName.EndsWith(toAdd))
					newName += toAdd;

				if(newName != obj.name)
					AssetDatabase.RenameAsset(path, newName);
			}
		}
		private static string GetEnding(System.Type type)
		{
			switch (type.Name)
			{
				case "SoundEffect":
					return "SFX";
				case "ParticleEffect":
					return "Particles";
				case "ScreenBlur":
					return "Blur";
				case "ScreenShake":
					return "Shake";
				case "ScreenBorderColor":
					return "BorderColor";
				case "CameraFOVWarp":
					return "FOVWarp";
				case "CustomUI":
					return "UI";
				case "ControllerRumble":
					return "Rumble";
				default:
					return type.Name;
			}
		}
		//Context menu
		[MenuItem("GameObject/UI/EoEButton")]
		public static void CreateEoEButton(MenuCommand menuCommand)
		{
			Transform parent = Selection.activeTransform;
			GameObject buttonMain = new GameObject("ControllerButton");
			buttonMain.AddComponent<RectTransform>().sizeDelta = new Vector2(200,50);
			GameObjectUtility.SetParentAndAlign(buttonMain, menuCommand.context as GameObject);

			UI.ControllerMenuItem m = buttonMain.AddComponent<UI.ControllerMenuItem>();

			GameObject inActiveTextMain = new GameObject("InActiveText");
			inActiveTextMain.transform.SetParent(buttonMain.transform);
			TextMeshProUGUI inActiveText = inActiveTextMain.AddComponent<TextMeshProUGUI>();
			inActiveText.text = "Button";
			inActiveText.alignment = TextAlignmentOptions.Center;

			//Clone the inActiveText
			TextMeshProUGUI activeText = Object.Instantiate(inActiveText, buttonMain.transform);
			GameObject activeTextMain = activeText.gameObject;
			activeTextMain.name = "ActiveText";
			activeText.color = Color.magenta;
			activeTextMain.SetActive(false);

			activeText.rectTransform.anchoredPosition = Vector2.zero;
			inActiveText.rectTransform.anchoredPosition = Vector2.zero;
			activeText.rectTransform.localScale = Vector2.one;
			inActiveText.rectTransform.localScale = Vector2.one;

			m.onSelectedEvent = new UnityEvent();
			m.onDeSelectedEvent = new UnityEvent();

			UnityAction<bool> inActiveSetter = new UnityAction<bool>(inActiveTextMain.SetActive);
			UnityEventTools.AddBoolPersistentListener(m.onSelectedEvent, inActiveSetter, false);
			UnityEventTools.AddBoolPersistentListener(m.onDeSelectedEvent, inActiveSetter, true);

			UnityAction<bool> activeSetter = new UnityAction<bool>(activeTextMain.SetActive);
			UnityEventTools.AddBoolPersistentListener(m.onSelectedEvent, activeSetter, true);
			UnityEventTools.AddBoolPersistentListener(m.onDeSelectedEvent, activeSetter, false);

			Undo.RegisterCreatedObjectUndo(buttonMain, "Create " + buttonMain.name);
			Selection.activeGameObject = buttonMain;
		}
		//Other
		public static T AssetCreator<T>(params string[] pathParts) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			string name = "/New " + typeof(T).Name;
			string path = "";

			for (int i = 0; i < pathParts.Length; i++)
			{
				path += "/" + pathParts[i];
				if (!Directory.Exists(Application.dataPath + path))
				{
					Directory.CreateDirectory(Application.dataPath + path);
				}
			}
			AssetDatabase.CreateAsset(asset, "Assets" + path + name + ".asset");

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = null;
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(asset);

			Debug.Log("Created: '" + name.Substring(1) + "' at: Assets" + path + "/..");
			return asset;
		}
	}

}