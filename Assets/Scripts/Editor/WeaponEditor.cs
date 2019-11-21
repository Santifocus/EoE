using UnityEditor;
using UnityEngine;

namespace EoE.Weapons
{
	[CustomEditor(typeof(Weapon), true), CanEditMultipleObjects]
	public class WeaponEditor : Editor
	{
		[MenuItem("EoE/Weapons/New Weapon")]
		public static void CreateAttackStyle()
		{
			Weapon asset = CreateInstance<Weapon>();

			AssetDatabase.CreateAsset(asset, "Assets/Settings/Weapons/New Weapon.asset");
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = asset;
			Debug.Log("Created: 'New Weapon' at: Assets/Settings/Weapons/...");
		}
		[MenuItem("EoE/Weapons/New Weapon Controller")]
		public static void CreateWeaponController()
		{
			GameObject newWeaponController = new GameObject("New Weapon Controller");
			WeaponController controller = newWeaponController.AddComponent<WeaponController>();

			WeaponHitbox newHitbox = (new GameObject("Weapon Hitbox")).AddComponent<WeaponHitbox>();

			//collider
			int layer = -1;
			int shifted = ConstantCollector.WEAPON_LAYER_MASK;
			for (; shifted > 0; layer++)
				shifted = shifted >> 1;
			newHitbox.gameObject.layer = layer;
			BoxCollider coll = newHitbox.gameObject.AddComponent<BoxCollider>();
			coll.isTrigger = true;

			//trail
			newHitbox.gameObject.AddComponent<TrailRenderer>();
			TrailRenderer trail = newHitbox.trail = newHitbox.GetComponent<TrailRenderer>();
			//default the trail
			trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			trail.receiveShadows = false;
			trail.alignment = LineAlignment.TransformZ;
			trail.time = 0.4f;
			trail.endColor = Color.white / 2;
			trail.enabled = false;

			newHitbox.transform.SetParent(newWeaponController.transform);

			controller.weaponHitboxes = new WeaponHitbox[1];
			controller.weaponHitboxes[0] = newHitbox;

			Selection.activeObject = PrefabUtility.SaveAsPrefabAsset(newWeaponController, "Assets/Prefabs/Weapons/" + newWeaponController.name + ".prefab");
			Debug.Log("Created: 'New Weapon Controller' at: Assets/Prefabs/Weapons/...");

			DestroyImmediate(newWeaponController);
		}
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			Weapon t = target as Weapon;
			if (!t.weaponAttackStyle)
			{
				EditorGUILayout.HelpBox("Weapon has no Attack-Style selected.", MessageType.Error);
			}
			if (!t.weaponPrefab)
			{
				EditorGUILayout.HelpBox("Weapon has no Weapon Prefab selected.", MessageType.Error);
			}
		}
	}
}