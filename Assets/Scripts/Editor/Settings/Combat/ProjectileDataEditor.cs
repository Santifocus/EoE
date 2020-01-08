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
		protected override void CustomInspector()
		{
			DrawInFoldoutHeader(new GUIContent("Direction Settings"), ref DirectionSettingsOpen, DirectionSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Flight Settings"), ref FlightSettingsOpen, FlightSettingsArea);
			DrawInFoldoutHeader(new GUIContent("Collision Settings"), ref CollisionSettingsOpen, CollisionSettingsArea);
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

			SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.StartEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffects))), ref settings.StartEffects, startEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 1);

			LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));

			SerializedProperty whileEffectsProperty = serializedObject.FindProperty(nameof(settings.WhileEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffects))), ref settings.WhileEffects, whileEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileTickTime))), ref settings.WhileTickTime, 1);
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

			SerializedProperty collisionEffectsProperty = serializedObject.FindProperty(nameof(settings.CollisionEffects));
			DrawArray<ActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CollisionEffects))), ref settings.CollisionEffects, collisionEffectsProperty, DrawActivationEffect, new GUIContent(". Effect"), 1);

			//Direct hit effects
			ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DirectHit))), ref settings.DirectHit, 1);
		}
	}
}