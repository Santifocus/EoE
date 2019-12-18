using EoE.Combatery;
using UnityEditor;
using UnityEngine;
using static EoE.EoEEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(Spell), true), CanEditMultipleObjects]
	public class SpellEditor : ObjectSettingEditor
	{
		private static bool BaseInfoOpen;
		private static bool CastInfoOpen;
		private static bool StartSettingsOpen;

		protected override void CustomInspector()
		{
			Spell settings = target as Spell;

			DrawInFoldoutHeader(new GUIContent("Base Settings"), ref BaseInfoOpen, BaseInfoArea);

			if (settings.HasSpellPart(SpellPart.Cast))
				DrawInFoldoutHeader(new GUIContent("Casting Settings"), ref CastInfoOpen, CastInfoArea);
			if (settings.HasSpellPart(SpellPart.Start))
				DrawInFoldoutHeader(new GUIContent("Start Settings"), ref StartSettingsOpen, StartInfoArea);

			if (settings.HasSpellPart(SpellPart.Projectile))
			{
				SerializedProperty projectileArrayProperty = serializedObject.FindProperty(nameof(settings.ProjectileInfos));
				bool projectileSettingsOpen = projectileArrayProperty.isExpanded;
				DrawInFoldoutHeader(new GUIContent("Projectile Settings"), ref projectileSettingsOpen, () => ProjectileInfoArea(projectileArrayProperty));
				if (projectileSettingsOpen != projectileArrayProperty.isExpanded)
					projectileArrayProperty.isExpanded = projectileSettingsOpen;
			}
		}

		private void BaseInfoArea()
		{
			Spell settings = target as Spell;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseDamage))), ref settings.BaseDamage, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseManaCost))), ref settings.BaseManaCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseEnduranceCost))), ref settings.BaseEnduranceCost, 1);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseKnockback))), ref settings.BaseKnockback, 1);
			SliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseCritChance))), ref settings.BaseCritChance, 0, 1, 1);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.SpellCooldown))), ref settings.SpellCooldown, 1);

			EnumFlagField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ContainedParts))), ref settings.ContainedParts, 1);
			EnumFlagField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MovementRestrictions))), ref settings.MovementRestrictions, 1);
		}
		private void CastInfoArea()
		{
			Spell settings = target as Spell;

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.Duration))), ref settings.CastInfo.Duration, 1);
			bool open;
			{
				SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.CastInfo)).FindPropertyRelative(nameof(settings.CastInfo.StartEffects));
				open = startEffectsProperty.isExpanded;
				ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.StartEffects))), ref settings.CastInfo.StartEffects, ref open, new GUIContent(". Effect"), 1);
				if (open != startEffectsProperty.isExpanded)
					startEffectsProperty.isExpanded = open;
			}
			{
				SerializedProperty whileEffectsProperty = serializedObject.FindProperty(nameof(settings.CastInfo)).FindPropertyRelative(nameof(settings.CastInfo.WhileEffects));
				open = whileEffectsProperty.isExpanded;
				ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.WhileEffects))), ref settings.CastInfo.WhileEffects, ref open, new GUIContent(". Effect"), 1);
				if (open != whileEffectsProperty.isExpanded)
					whileEffectsProperty.isExpanded = open;
			}

			//Custom FX Objects
			{
				SerializedProperty visualCastEffectsProperty = serializedObject.FindProperty(nameof(settings.CastInfo)).FindPropertyRelative(nameof(settings.CastInfo.VisualEffects));
				DrawCustomFXObjectArray(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CastInfo.VisualEffects))), ref settings.CastInfo.VisualEffects, visualCastEffectsProperty, 1);
			}
		}
		private void StartInfoArea()
		{
			Spell settings = target as Spell;

			SerializedProperty startEffectsProperty = serializedObject.FindProperty(nameof(settings.StartInfo)).FindPropertyRelative(nameof(settings.StartInfo.Effects));
			bool open = startEffectsProperty.isExpanded;
			ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartInfo.Effects))), ref settings.StartInfo.Effects, ref open, new GUIContent(". Effect"), 1);
			if (open != startEffectsProperty.isExpanded)
				startEffectsProperty.isExpanded = open;

			SerializedProperty visualStartEffectProperty = serializedObject.FindProperty(nameof(settings.StartInfo)).FindPropertyRelative(nameof(settings.StartInfo.VisualEffects));
			DrawCustomFXObjectArray(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartInfo.VisualEffects))), ref settings.StartInfo.VisualEffects, visualStartEffectProperty, 1);
		}
		private void ProjectileInfoArea(SerializedProperty projectileArrayProperty)
		{
			Spell settings = target as Spell;
			int newSize = settings.ProjectileInfos.Length;
			DelayedIntField("Size", ref newSize, 1);

			for (int i = 0; i < settings.ProjectileInfos.Length; i++)
			{
				DrawProjectileInfo(projectileArrayProperty.GetArrayElementAtIndex(i), settings.ProjectileInfos[i], i, 1);
			}

			if (settings.ProjectileInfos.Length != newSize)
			{
				isDirty = true;
				ProjectileInfo[] newArray = new ProjectileInfo[newSize];
				for (int i = 0; i < newSize; i++)
				{
					if (i < settings.ProjectileInfos.Length)
						newArray[i] = settings.ProjectileInfos[i];
					else
						newArray[i] = new ProjectileInfo();
				}
				settings.ProjectileInfos = newArray;
				projectileArrayProperty.arraySize = newSize;
			}
		}
		private void DrawProjectileInfo(SerializedProperty projectileProperty, ProjectileInfo info, int index, int offSet)
		{
			if (info == null)
			{
				info = new ProjectileInfo();
			}
			FoldoutFromSerializedProperty(new GUIContent((index + 1) + ". Projectile Info"), projectileProperty, offSet);
			if (projectileProperty.isExpanded)
			{
				ObjectField<ProjectileData>(new GUIContent("ProjectileData"), ref info.Projectile, offSet + 1);

				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(info.ExecutionDelay))), ref info.ExecutionDelay, offSet + 1);

				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(info.ExecutionCount))), ref info.ExecutionCount, offSet + 1);
				if (info.ExecutionCount > 1)
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(info.ExecutionRepeatDelay))), ref info.ExecutionRepeatDelay, offSet + 1);
			}
		}
	}
}