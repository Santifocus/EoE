using EoE.Weapons;
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
		private static bool CastParticlesOpen;
		private static bool CastStartEffectsOpen;
		private static bool CastWhileEffectsOpen;

		private static bool StartInfoOpem;
		private static bool StartParticlesOpen;
		private static bool StartEffectsOpen;

		private static bool ProjectileHeaderOpen;
		private bool ListsSetup;
		private List<bool> ProjectileInfoOpen = new List<bool>();
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
				FloatField(new GUIContent("Manacost"), ref settings.ManaCost, 1);
				FloatField(new GUIContent("Base Damage"), ref settings.BaseDamage, 1);
				FloatField(new GUIContent("Base Knockback"), ref settings.BaseKnockback, 1);
				FloatField(new GUIContent("Spell Cooldown"), ref settings.SpellCooldown, 1);

				EnumFlagField(new GUIContent("ContainedParts"), ref settings.ContainedParts, 1);
				EnumFlagField(new GUIContent("MovementRestrictions"), ref settings.MovementRestrictions, 1);
			}
			EndFoldoutHeader();
		}
		private void CastInfoArea()
		{
			Spell settings = target as Spell;
			if (!settings.HasPart(SpellPart.Cast))
				return;

			FoldoutHeader("Cast Info", ref CastInfoOpen);
			if (CastInfoOpen)
			{
				FloatField(new GUIContent("Duration"), ref settings.CastInfo.Duration, 1);
				ObjectArrayField<ParticleEffect>(new GUIContent("Particle Effects"), ref settings.CastInfo.ParticleEffects, ref CastParticlesOpen, new GUIContent("Particle "), 1);
				ObjectArrayField<SpellEffect>(new GUIContent("Start Effects"), ref settings.CastInfo.StartEffects, ref CastStartEffectsOpen, new GUIContent("Effect "), 1);
				ObjectArrayField<SpellEffect>(new GUIContent("While Effects"), ref settings.CastInfo.WhileEffects, ref CastWhileEffectsOpen, new GUIContent("Effect "), 1);
			}
			EndFoldoutHeader();
		}
		private void BeginningInfoArea()
		{
			Spell settings = target as Spell;
			if (!settings.HasPart(SpellPart.Start))
				return;

			FoldoutHeader("Start Info", ref StartInfoOpem);
			if (StartInfoOpem)
			{
				ObjectArrayField<ParticleEffect>(new GUIContent("Particle Effects"), ref settings.StartInfo.ParticleEffects, ref StartParticlesOpen, new GUIContent("Particles "), 1);
				ObjectArrayField<SpellEffect>(new GUIContent("Start Effects"), ref settings.StartInfo.Effects, ref StartEffectsOpen, new GUIContent("Effect "), 1);
			}
			EndFoldoutHeader();
		}
		private void ProjectileInfoArea()
		{
			Spell settings = target as Spell;
			if (!settings.HasPart(SpellPart.Projectile))
				return;

			int preSize = settings.ProjectileInfo.Length;
			DrawArray<SpellProjectilePart>(new GUIContent("Projectile Info"), new System.Func<int, int, bool>(DrawProjectileInfo), ref settings.ProjectileInfo, ref ProjectileHeaderOpen, 0, true);
			if(preSize != settings.ProjectileInfo.Length)
			{
				UpdateAllListSizes();

				for(int i = 0; i < settings.ProjectileInfo.Length; i++)
				{
					if (settings.ProjectileInfo[i] == null)
						settings.ProjectileInfo[i] = new SpellProjectilePart();
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
		private bool DrawProjectileInfo(int index, int offset)
		{
			Spell settings = target as Spell;
			bool changed = false;

			bool open = ProjectileInfoOpen[index];
			changed |= Foldout(new GUIContent("Projectile " + (index + 1)), ref open, offset);
			ProjectileInfoOpen[index] = open;
			if (open)
			{
				//Direction style
				changed |= EnumField(new GUIContent("Direction Style"), ref settings.ProjectileInfo[index].DirectionStyle, offset + 1);

				//Fallback direction style
				if (settings.ProjectileInfo[index].DirectionStyle == InherritDirection.Target)
				{
					if (EnumField(new GUIContent("Fallback Direction Style"), ref settings.ProjectileInfo[index].FallbackDirectionStyle, offset + 1))
					{
						changed = true;
						if (settings.ProjectileInfo[index].FallbackDirectionStyle == InherritDirection.Target)
						{
							settings.ProjectileInfo[index].FallbackDirectionStyle = InherritDirection.Local;
						}
					}
				}

				//Projectile flight
				{
					//Direction Base
					changed |= EnumField(new GUIContent("Direction"), ref settings.ProjectileInfo[index].Direction, offset + 1);

					if (FloatField(new GUIContent("Hitbox Size"), ref settings.ProjectileInfo[index].HitboxSize, offset + 1))
					{
						changed = true;
						settings.ProjectileInfo[index].HitboxSize = Mathf.Max(0, settings.ProjectileInfo[index].HitboxSize);
					}
					changed |= FloatField(new GUIContent("Max Lifetime"), ref settings.ProjectileInfo[index].Duration, offset + 1);
					changed |= FloatField(new GUIContent("Flight Speed"), ref settings.ProjectileInfo[index].FlightSpeed, offset + 1);

					changed |= Vector3Field(new GUIContent("CreateOffsetToCaster"), ref settings.ProjectileInfo[index].CreateOffsetToCaster, offset + 1);

					open = ProjectileParticlesOpen[index];
					changed |= ObjectArrayField(new GUIContent("Particle Effects"), ref settings.ProjectileInfo[index].ParticleEffects, ref open, new GUIContent("Particle "), offset + 1);
					ProjectileParticlesOpen[index] = open;

					open = ProjectileStartEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("Start Effects"), ref settings.ProjectileInfo[index].StartEffects, ref open, new GUIContent("Effect "), offset + 1);
					ProjectileStartEffectsOpen[index] = open;

					open = ProjectileWhileEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("While Effects"), ref settings.ProjectileInfo[index].WhileEffects, ref open, new GUIContent("Effect "), offset + 1);
					ProjectileWhileEffectsOpen[index] = open;
				}

				//Collision
				{
					changed |= EnumFlagField(new GUIContent("Collide Mask"), ref settings.ProjectileInfo[index].CollideMask, offset + 1);

					//Bounce
					changed |= IntField(new GUIContent("Bounces"), ref settings.ProjectileInfo[index].Bounces, offset + 1);
					if (settings.ProjectileInfo[index].Bounces > 0)
					{
						changed |= BoolField(new GUIContent("Bounce Off Entities"), ref settings.ProjectileInfo[index].BounceOffEntities, offset + 2);
						changed |= BoolField(new GUIContent("Collision Effects On Bounce"), ref settings.ProjectileInfo[index].CollisionEffectsOnBounce, offset + 2);
					}

					//Effects
					open = ProjectileCollisionParticleEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("Collision Particle Effects"), ref settings.ProjectileInfo[index].CollisionParticleEffects, ref open, new GUIContent("Particle "), offset + 1);
					ProjectileCollisionParticleEffectsOpen[index] = open;

					open = ProjectileCollisionEffectsOpen[index];
					changed |= ObjectArrayField(new GUIContent("Collision Effects"), ref settings.ProjectileInfo[index].CollisionEffects, ref open, new GUIContent("Effect "), offset + 1);
					ProjectileCollisionEffectsOpen[index] = open;
				}

				//Remenants
				BoolField(new GUIContent("Creates Remenants"), ref settings.ProjectileInfo[index].CreatesRemenants, offset + 1);
				if (settings.ProjectileInfo[index].CreatesRemenants)
				{
					changed |= RemenantsInfoArea(index, offset + 2);
				}
			}

			//Delay to next projectile
			if (index < settings.ProjectileInfo.Length - 1)
			{
				LineBreak();
				changed |= FloatField(new GUIContent("Projectile Delay"), ref settings.DelayToNextProjectile[index], offset);
			}
			LineBreak();
			return changed;
		}

		private bool RemenantsInfoArea(int spellProjectileIndex, int offset)
		{
			Spell settings = target as Spell;
			bool changed = false;

			changed |= FloatField(new GUIContent("Duration"), ref settings.ProjectileInfo[spellProjectileIndex].Remenants.Duration, offset);

			bool open = ProjectileRemenantsInfosOpen[spellProjectileIndex].Item1;
			changed |= ObjectArrayField(new GUIContent("Particle Effects"), ref settings.ProjectileInfo[spellProjectileIndex].Remenants.ParticleEffects, ref open, new GUIContent("Particle "), offset);
			ProjectileRemenantsInfosOpen[spellProjectileIndex] = (open, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item2, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item3);

			open = ProjectileRemenantsInfosOpen[spellProjectileIndex].Item2;
			changed |= ObjectArrayField(new GUIContent("Start Effects"), ref settings.ProjectileInfo[spellProjectileIndex].Remenants.StartEffects, ref open, new GUIContent("Effect "), offset);
			ProjectileRemenantsInfosOpen[spellProjectileIndex] = (ProjectileRemenantsInfosOpen[spellProjectileIndex].Item1, open, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item3);

			open = ProjectileRemenantsInfosOpen[spellProjectileIndex].Item3;
			changed |= ObjectArrayField(new GUIContent("While Effects"), ref settings.ProjectileInfo[spellProjectileIndex].Remenants.WhileEffects, ref open, new GUIContent("Effect "), offset);
			ProjectileRemenantsInfosOpen[spellProjectileIndex] = (ProjectileRemenantsInfosOpen[spellProjectileIndex].Item1, ProjectileRemenantsInfosOpen[spellProjectileIndex].Item2, open);

			changed |= BoolField(new GUIContent("Try Ground Remenants"), ref settings.ProjectileInfo[spellProjectileIndex].Remenants.TryGroundRemenants, offset);
			return changed;
		}
	}
}