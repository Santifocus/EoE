using EoE.Combatery;
using System.Collections.Generic;
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
		private static bool CastFXEffectsOpen;
		private static bool CastStartEffectsOpen;
		private static bool CastWhileEffectsOpen;

		private static bool StartInfoOpem;
		private static bool StartFXEffectsOpen;
		private static bool StartEffectsOpen;

		private static bool ProjectileHeaderOpen;
		private bool ListsSetup;

		private List<bool> ProjectileInfoOpen = new List<bool>();

		private List<bool> ProjectileDirectionSettingsOpen = new List<bool>();
		private List<bool> ProjectileFlightSettingsOpen = new List<bool>();
		private List<bool> ProjectileCollisionSettingsOpen = new List<bool>();

		private List<bool> ProjectileParticlesOpen = new List<bool>();
		private List<bool> ProjectileStartEffectsOpen = new List<bool>();
		private List<bool> ProjectileWhileEffectsOpen = new List<bool>();
		private List<bool> ProjectileCollisionEffectsOpen = new List<bool>();
		private List<bool> ProjectileCollisionParticleEffectsOpen = new List<bool>();

		private List<(bool, bool, bool)> ProjectileRemenantsInfosOpen = new List<(bool, bool, bool)>();

		protected override void CustomInspector() 
		{
			if (!ListsSetup)
			{
				ListsSetup = true;
				UpdateAllListSizes();
			}

			BaseInfoArea();
			CastInfoArea();
			BeginningInfoArea();
			ProjectileInfoArea();
		}

		private void BaseInfoArea()
		{
			Spell settings = target as Spell;
			FoldoutHeader("Base Info", ref BaseInfoOpen);
			if (BaseInfoOpen)
			{
				FloatField(new GUIContent("Base Damage"), ref settings.BaseDamage, 1);
				FloatField(new GUIContent("Base Mana Cost"), ref settings.BaseManaCost, 1);
				FloatField(new GUIContent("Base Endurance Cost"), ref settings.BaseEnduranceCost, 1);
				FloatField(new GUIContent("Base Knockback"), ref settings.BaseKnockback, 1);
				SliderField(new GUIContent("Base Crit Chance"), ref settings.BaseCritChance, 0, 1, 1);

				GUILayout.Space(4);
				FloatField(new GUIContent("Spell Cooldown"), ref settings.SpellCooldown, 1);

				EnumFlagField(new GUIContent("ContainedParts"), ref settings.ContainedParts, 1);
				EnumFlagField(new GUIContent("MovementRestrictions"), ref settings.MovementRestrictions, 1);
			}
			EndFoldoutHeader();
		}
		private void CastInfoArea()
		{
			Spell settings = target as Spell;
			if (!settings.HasSpellPart(SpellPart.Cast))
				return;

			FoldoutHeader("Cast Info", ref CastInfoOpen);
			if (CastInfoOpen)
			{
				FloatField(new GUIContent("Duration"), ref settings.CastInfo.Duration, 1);
				DrawArray<CustomFXObject>(new GUIContent("FX Effects"), DrawCustomFXObject, ref settings.CastInfo.CustomEffects, ref CastFXEffectsOpen, 1);
				ObjectArrayField<EffectAOE>(new GUIContent("Start Effects"), ref settings.CastInfo.StartEffects, ref CastStartEffectsOpen, new GUIContent("Effect "), 1);
				ObjectArrayField<EffectAOE>(new GUIContent("While Effects"), ref settings.CastInfo.WhileEffects, ref CastWhileEffectsOpen, new GUIContent("Effect "), 1);
			}
			EndFoldoutHeader();
		}
		private void BeginningInfoArea()
		{
			Spell settings = target as Spell;
			if (!settings.HasSpellPart(SpellPart.Start))
				return;

			FoldoutHeader("Start Info", ref StartInfoOpem);
			if (StartInfoOpem)
			{
				DrawArray<CustomFXObject>(new GUIContent("FX Effects"), DrawCustomFXObject, ref settings.StartInfo.CustomEffects, ref StartFXEffectsOpen, 1);
				ObjectArrayField<EffectAOE>(new GUIContent("Start Effects"), ref settings.StartInfo.Effects, ref StartEffectsOpen, new GUIContent("Effect "), 1);
			}
			EndFoldoutHeader();
		}
		private void ProjectileInfoArea()
		{
			Spell settings = target as Spell;
			if (!settings.HasSpellPart(SpellPart.Projectile))
				return;

			int preSize = settings.ProjectileInfo.Length;
			DrawArray<ProjectileData>(new GUIContent("Projectile Info"), DrawProjectileInfo, ref settings.ProjectileInfo, ref ProjectileHeaderOpen, 0, true);
			if(preSize != settings.ProjectileInfo.Length)
			{
				UpdateAllListSizes();

				for(int i = 0; i < settings.ProjectileInfo.Length; i++)
				{
					if (settings.ProjectileInfo[i] == null)
						settings.ProjectileInfo[i] = new ProjectileData();
				}

				int delayNewSize = settings.ProjectileInfo.Length - 1;
				float[] newArray = new float[delayNewSize];
				for (int i = 0; i < delayNewSize; i++)
				{
					if (i < settings.DelayToNextProjectile.Length)
						newArray[i] = settings.DelayToNextProjectile[i];
					else
						break;
				}

				settings.DelayToNextProjectile = newArray;
			}
		}
		private void UpdateAllListSizes()
		{
			Spell settings = target as Spell;
			int size = settings.ProjectileInfo.Length;

			UpdateListSize(ProjectileInfoOpen, size);

			UpdateListSize(ProjectileDirectionSettingsOpen, size);
			UpdateListSize(ProjectileFlightSettingsOpen, size);
			UpdateListSize(ProjectileCollisionSettingsOpen, size);

			UpdateListSize(ProjectileParticlesOpen, size);
			UpdateListSize(ProjectileStartEffectsOpen, size);
			UpdateListSize(ProjectileWhileEffectsOpen, size);
			UpdateListSize(ProjectileCollisionEffectsOpen, size);
			UpdateListSize(ProjectileCollisionParticleEffectsOpen, size);

			UpdateListSize(ProjectileRemenantsInfosOpen, size);
		}
		private void UpdateListSize<T>(List<T> old, int size)
		{
			Spell settings = target as Spell;
			int dif = size - old.Count;
			for(int i = 0; i < dif; i++)
			{
				old.Add(default);
			}
		}
		private bool DrawProjectileInfo(int index, int offset, ProjectileData[] parentArray)
		{
			Spell settings = target as Spell;
			bool changed = false;

			//Execution info
			changed |= IntField(new GUIContent("Execution Count"), ref parentArray[index].ExecutionCount, offset);
			if(parentArray[index].ExecutionCount > 1)
				changed |= FloatField(new GUIContent("Delay Per Execution"), ref parentArray[index].DelayPerExecution, offset + 1);
			GUILayout.Space(4);

			bool open = ProjectileInfoOpen[index];
			changed |= Foldout(new GUIContent("Projectile " + (index + 1)), ref open, offset);
			ProjectileInfoOpen[index] = open;

			if (open)
			{
				//Direction settings
				open = ProjectileDirectionSettingsOpen[index];
				Foldout(new GUIContent("Direction settings"), ref open, offset + 1);
				ProjectileDirectionSettingsOpen[index] = open;
				if (open)
				{
					//Direction style
					changed |= EnumField(new GUIContent("Direction Style"), ref parentArray[index].DirectionStyle, offset + 2);

					//Fallback direction style
					if (parentArray[index].DirectionStyle == InherritDirection.Target)
					{
						if (EnumField(new GUIContent("Fallback Direction Style"), ref parentArray[index].FallbackDirectionStyle, offset + 2))
						{
							changed = true;
							if (parentArray[index].FallbackDirectionStyle == InherritDirection.Target)
							{
								parentArray[index].FallbackDirectionStyle = InherritDirection.Local;
							}
						}
					}
					//Direction Base
					changed |= EnumField(new GUIContent("Direction"), ref parentArray[index].Direction, offset + 3);
					GUILayout.Space(5);
				}

				//Projectile flight
				open = ProjectileFlightSettingsOpen[index];
				Foldout(new GUIContent("Projectile Flight"), ref open, offset + 1);
				ProjectileFlightSettingsOpen[index] = open;
				if (open)
				{

					changed |= FloatField(new GUIContent("Max Lifetime"), ref parentArray[index].Duration, offset + 2);
					changed |= FloatField(new GUIContent("Flight Speed"), ref parentArray[index].FlightSpeed, offset + 2);

					changed |= Vector3Field(new GUIContent("CreateOffsetToCaster"), ref parentArray[index].CreateOffsetToCaster, offset + 2);

					open = ProjectileParticlesOpen[index];
					changed |= DrawArray<CustomFXObject>(new GUIContent("FX Effects"), DrawCustomFXObject, ref parentArray[index].CustomEffects, ref open, offset + 2);
					ProjectileParticlesOpen[index] = open;

					open = ProjectileStartEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("Start Effects"), ref parentArray[index].StartEffects, ref open, new GUIContent("Effect "), offset + 2);
					ProjectileStartEffectsOpen[index] = open;

					open = ProjectileWhileEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("While Effects"), ref parentArray[index].WhileEffects, ref open, new GUIContent("Effect "), offset + 2);
					ProjectileWhileEffectsOpen[index] = open;
					GUILayout.Space(5);
				}

				//Collision
				open = ProjectileCollisionSettingsOpen[index];
				Foldout(new GUIContent("Collision settings"), ref open, offset + 1);
				ProjectileCollisionSettingsOpen[index] = open;
				if (open)
				{
					changed |= EnumFlagField(new GUIContent("Collide Mask"), ref parentArray[index].CollideMask, offset + 2);

					if (FloatField(new GUIContent("Terrain Hitbox Size"), ref parentArray[index].TerrainHitboxSize, offset + 2))
					{
						changed = true;
						parentArray[index].TerrainHitboxSize = Mathf.Max(0, parentArray[index].TerrainHitboxSize);
					}

					if (((parentArray[index].CollideMask | ColliderMask.Terrain) == parentArray[index].CollideMask) &&
						FloatField(new GUIContent("Entitie Hitbox Size"), ref parentArray[index].EntitieHitboxSize, offset + 2))
					{
						changed = true;
						parentArray[index].EntitieHitboxSize = Mathf.Max(0, parentArray[index].EntitieHitboxSize);
					}

					//Bounce
					changed |= IntField(new GUIContent("Bounces"), ref parentArray[index].Bounces, offset + 2);
					if (parentArray[index].Bounces > 0)
					{
						changed |= BoolField(new GUIContent("Destroy On Entite Bounce"), ref parentArray[index].DestroyOnEntiteBounce, offset + 3);
						changed |= BoolField(new GUIContent("Collision Effects On Bounce"), ref parentArray[index].CollisionEffectsOnBounce, offset + 3);
					}

					//Effects
					open = ProjectileCollisionParticleEffectsOpen[index];
					changed |= DrawArray<CustomFXObject>(new GUIContent("Collision FX Effects"), DrawCustomFXObject, ref parentArray[index].CollisionCustomEffects, ref open, offset + 2);
					ProjectileCollisionParticleEffectsOpen[index] = open;

					open = ProjectileCollisionEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("Collision Effects"), ref parentArray[index].CollisionEffects, ref open, new GUIContent("Effect "), offset + 2);
					ProjectileCollisionEffectsOpen[index] = open;
					GUILayout.Space(5);
				}

				//DirectHit
				ObjectField(new GUIContent("Direct Hit Settings"), ref parentArray[index].DirectHit, offset + 1);

				//Remenants
				BoolField(new GUIContent("Creates Remenants"), ref parentArray[index].CreatesRemenants, offset + 1);
				if (parentArray[index].CreatesRemenants)
				{
					changed |= RemenantsInfoArea(index, offset + 2, parentArray);
				}
			}

			//Delay to next projectile
			if (index < parentArray.Length - 1)
			{
				LineBreak();
				changed |= FloatField(new GUIContent("Delay To Next Projectile"), ref settings.DelayToNextProjectile[index], offset);
			}
			LineBreak();
			return changed;
		}

		private bool DrawCustomFXObject(int index, int offset, CustomFXObject[] parentArray)
		{
			bool changed = false;
			if (parentArray == null)
			{
				return false;
			}
			else if(parentArray[index] == null)
			{
				parentArray[index] = new CustomFXObject();
			}

			changed |= Foldout(new GUIContent("Effect " + index), ref parentArray[index].openInInspector, offset);
			if (parentArray[index].openInInspector)
			{
				changed |= ObjectField<FXObject>("Effect", ref parentArray[index].FX, offset + 1);
				changed |= NullableVector3Field(new GUIContent("Has Custom Offset"), new GUIContent("Custom Offset"), ref parentArray[index].CustomOffset, ref parentArray[index].HasCustomOffset, offset + 1);
				changed |= NullableVector3Field(new GUIContent("Has Rotation Offset"), new GUIContent("Custom Rotation Offset"), ref parentArray[index].CustomRotation, ref parentArray[index].HasCustomRotationOffset, offset + 1);
				changed |= NullableVector3Field(new GUIContent("Has Custom Scale"), new GUIContent("Custom Scale"), ref parentArray[index].CustomScale, ref parentArray[index].HasCustomScale, offset + 1);
			}

			return changed;
		}

		private bool RemenantsInfoArea(int spellProjectileIndex, int offset, ProjectileData[] parentArray)
		{
			bool changed = false;

			changed |= FloatField(new GUIContent("Duration"), ref parentArray[spellProjectileIndex].Remenants.Duration, offset);

			bool open = ProjectileRemenantsInfosOpen[spellProjectileIndex].Item1;
			changed |= ObjectArrayField(new GUIContent("Particle Effects"), ref parentArray[spellProjectileIndex].Remenants.ParticleEffects, ref open, new GUIContent("Particle "), offset);
			ProjectileRemenantsInfosOpen[spellProjectileIndex] = (open, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item2, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item3);

			open = ProjectileRemenantsInfosOpen[spellProjectileIndex].Item2;
			changed |= ObjectArrayField(new GUIContent("Start Effects"), ref parentArray[spellProjectileIndex].Remenants.StartEffects, ref open, new GUIContent("Effect "), offset);
			ProjectileRemenantsInfosOpen[spellProjectileIndex] = (ProjectileRemenantsInfosOpen[spellProjectileIndex].Item1, open, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item3);

			open = ProjectileRemenantsInfosOpen[spellProjectileIndex].Item3;
			changed |= ObjectArrayField(new GUIContent("While Effects"), ref parentArray[spellProjectileIndex].Remenants.WhileEffects, ref open, new GUIContent("Effect "), offset);
			ProjectileRemenantsInfosOpen[spellProjectileIndex] = (ProjectileRemenantsInfosOpen[spellProjectileIndex].Item1, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item2, open);

			changed |= BoolField(new GUIContent("Try Ground Remenants"), ref parentArray[spellProjectileIndex].Remenants.TryGroundRemenants, offset);
			return changed;
		}
	}
}