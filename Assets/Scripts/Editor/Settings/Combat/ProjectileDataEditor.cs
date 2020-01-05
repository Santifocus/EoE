using EoE.Information;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Combatery
{
	[CustomEditor(typeof(ProjectileData), true), CanEditMultipleObjects]
	public class ProjectileDataEditor : ObjectSettingEditor
	{
		private static bool DirectionSettingsOpen;
		private static bool FlightSettingsOpen;
		private static bool CollisionSettingsOpen;
		private static bool RemenantsSettingsOpen;
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Direction Settings"), ref DirectionSettingsOpen, DirectionSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Flight Settings"), ref FlightSettingsOpen, FlightSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Collision Settings"), ref CollisionSettingsOpen, CollisionSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Remenants Settings"), ref RemenantsSettingsOpen, RemenantsSettingsArea);
		}

		private void DirectionSettingsArea()
		{
			ProjectileData settings = target as ProjectileData;
			//Direction style
			EnumField(new GUIContent("Direction Style"), ref settings.DirectionStyle, 1);

			//Fallback direction style
			if (settings.DirectionStyle == InherritDirection.Target)
			{
				EnumField(new GUIContent("Fallback Direction Style"), ref settings.FallbackDirectionStyle, 1);
				if (settings.FallbackDirectionStyle == InherritDirection.Target)
					settings.FallbackDirectionStyle = InherritDirection.Local;
			}
			//Direction Base
			EnumField(new GUIContent("Direction"), ref settings.Direction, 2);
		}
		private void FlightSettingsArea()
		{
			ProjectileData settings = target as ProjectileData;

			FloatField(new GUIContent("Max Lifetime"), ref settings.Duration, 1);
			FloatField(new GUIContent("Flight Speed"), ref settings.FlightSpeed, 1);
			Vector3Field(new GUIContent("CreateOffsetToCaster"), ref settings.CreateOffsetToCaster, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

			SerializedProperty flightEffectsProperty = serializedObject.FindProperty(nameof(settings.VisualStartEffects));
			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.VisualStartEffects))), ref settings.VisualStartEffects, flightEffectsProperty, DrawCustomFXObject, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

			SerializedProperty startEffectsOpen = serializedObject.FindProperty(nameof(settings.StartEffects));
			ObjectArrayField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffects))), ref settings.StartEffects, startEffectsOpen, new GUIContent(". Effect"), 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

			SerializedProperty whileEffectsOpen = serializedObject.FindProperty(nameof(settings.WhileEffects));
			ObjectArrayField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffects))), ref settings.WhileEffects, whileEffectsOpen, new GUIContent(". Effect"), 1);
		}
		private void CollisionSettingsArea()
		{
			ProjectileData settings = target as ProjectileData;
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.TerrainHitboxSize))), ref settings.TerrainHitboxSize, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EntitieHitboxSize))), ref settings.EntitieHitboxSize, 1);

			EnumFlagField<ColliderMask>(ObjectNames.NicifyVariableName(nameof(settings.CollideMask)), ref settings.CollideMask, 1);

			IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Bounces))), ref settings.Bounces, 1);
			if (settings.Bounces > 0)
			{
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DestroyOnEntiteBounce))), ref settings.DestroyOnEntiteBounce, 2);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CollisionEffectsOnBounce))), ref settings.CollisionEffectsOnBounce, 2);
			}

			SerializedProperty collisionEffectsProperty = serializedObject.FindProperty(nameof(settings.VisualCollisionEffects));
			DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.VisualCollisionEffects))), ref settings.VisualCollisionEffects, collisionEffectsProperty, DrawCustomFXObject, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
			//AOE Effects
			SerializedProperty collisionEffectsAOEProperty = serializedObject.FindProperty(nameof(settings.CollisionEffectsAOE));
			ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CollisionEffectsAOE))), ref settings.CollisionEffectsAOE, collisionEffectsAOEProperty, new GUIContent(". Effect"), 1);

			//Direct hit effects
			ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DirectHit))), ref settings.DirectHit, 1);
		}
		private void RemenantsSettingsArea()
		{
			ProjectileData settings = target as ProjectileData;
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CreatesRemenants))), ref settings.CreatesRemenants, 1);
			if (settings.CreatesRemenants)
			{
				if (settings.Remenants == null)
					settings.Remenants = new ProjectileRemenants();

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

				SerializedProperty effectsProperty = serializedObject.FindProperty(nameof(settings.Remenants)).FindPropertyRelative(nameof(settings.Remenants.VisualEffects));
				DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Remenants.VisualEffects))), ref settings.Remenants.VisualEffects, effectsProperty, DrawCustomFXObject, 2);

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

				SerializedProperty startEffectsOpen = serializedObject.FindProperty(nameof(settings.Remenants)).FindPropertyRelative(nameof(settings.Remenants.StartEffects));
				ObjectArrayField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Remenants.StartEffects))), ref settings.Remenants.StartEffects, startEffectsOpen, new GUIContent(". Effect"), 2);

				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

				SerializedProperty whileEffectsOpen = serializedObject.FindProperty(nameof(settings.Remenants)).FindPropertyRelative(nameof(settings.Remenants.WhileEffects));
				ObjectArrayField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Remenants.WhileEffects))), ref settings.Remenants.WhileEffects, whileEffectsOpen, new GUIContent(". Effect"), 2);
			}
		}
	}
}