using EoE.Information;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(ShieldData), true), CanEditMultipleObjects]
	public class ShieldDataEditor : ScriptableObjectEditor
	{
		private static bool BaseSettingsOpen;
		private static bool FollowSettingsOpen;
		private static bool FXSettingsOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseSettingsOpen, DrawBaseSettings);
			DrawInFoldoutHeader(new GUIContent("Follow Settings"), ref FollowSettingsOpen, DrawFollowSettings);
			DrawInFoldoutHeader(new GUIContent("FX Settings"), ref FXSettingsOpen, DrawFXSettings);
		}
		private void DrawBaseSettings()
		{
			ShieldData settings = target as ShieldData;

			ObjectField<CombatObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseData))), ref settings.BaseData, 1);
			DrawObjectCost(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FirstActivationCost))), ref settings.FirstActivationCost, serializedObject.FindProperty(nameof(settings.FirstActivationCost)), 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldResistance))), ref settings.ShieldResistance, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldDrain))), ref settings.ShieldDrain, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldRegeneration))), ref settings.ShieldRegeneration, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldSize))), ref settings.ShieldSize, 1);

			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ReflectProjectiles))), ref settings.ReflectProjectiles, 1);
		}
		private void DrawFollowSettings()
		{
			ShieldData settings = target as ShieldData;

			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FollowOwner))), ref settings.FollowOwner, 1);
			if(settings.FollowOwner)
				Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OffsetToOwner))), ref settings.OffsetToOwner, 1);

			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.InheritOwnerRotation))), ref settings.InheritOwnerRotation, 1);
			if (settings.InheritOwnerRotation)
				Vector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RotationOffsetToOwner))), ref settings.RotationOffsetToOwner, 1);
		}
		private void DrawFXSettings()
		{
			ShieldData settings = target as ShieldData;

			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldStartEffects))), ref settings.ShieldStartEffects, serializedObject.FindProperty(nameof(settings.ShieldStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldFailedStartEffects))), ref settings.ShieldFailedStartEffects, serializedObject.FindProperty(nameof(settings.ShieldFailedStartEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldActiveEffects))), ref settings.ShieldActiveEffects, serializedObject.FindProperty(nameof(settings.ShieldActiveEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldHitEffects))), ref settings.ShieldHitEffects, serializedObject.FindProperty(nameof(settings.ShieldHitEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldDisableEffects))), ref settings.ShieldDisableEffects, serializedObject.FindProperty(nameof(settings.ShieldDisableEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ShieldBreakEffects))), ref settings.ShieldBreakEffects, serializedObject.FindProperty(nameof(settings.ShieldBreakEffects)), DrawActivationEffect, new GUIContent(". Effect"), 1);
		}
	}
}