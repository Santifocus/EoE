using EoE.Combatery;
using EoE.Information;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE
{
	public static class FXManager
	{
		#region FXInstance Creation
		/// <summary>
		/// Non-allocating Execution of a FXObject Array.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static void ExecuteFX(FXObject[] effects, Transform target, bool allowPlayerOnlyEffects, float multiplier = 1)
		{
			if (effects == null)
				return;

			for (int i = 0; i < effects.Length; i++)
			{
				ExecuteFX(effects[i], target, allowPlayerOnlyEffects, multiplier);
			}
		}
		/// <summary>
		/// Non allocating Execution of a CustomFXObject Array.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static void ExecuteFX(CustomFXObject[] effects, Transform target, bool allowPlayerOnlyEffects, float multiplier = 1)
		{
			if (effects == null)
				return;

			for (int i = 0; i < effects.Length; i++)
			{
				ExecuteFX(effects[i], target, allowPlayerOnlyEffects, multiplier);
			}
		}
		/// <summary>
		/// Returns an array of FXInstances that were created via the given FXObject Array.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static void ExecuteFX(FXObject[] effects, Transform target, bool allowPlayerOnlyEffects, out FXInstance[] createdEffects, float multiplier = 1)
		{
			if(effects == null)
			{
				createdEffects = new FXInstance[0];
				return;
			}

			createdEffects = new FXInstance[effects.Length];
			for (int i = 0; i < effects.Length; i++)
			{
				createdEffects[i] = ExecuteFX(effects[i], target, allowPlayerOnlyEffects, multiplier);
			}
		}
		/// <summary>
		/// Returns an array of FXInstances that were created via the given CustomFXObject Array.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static void ExecuteFX(CustomFXObject[] effects, Transform target, bool allowPlayerOnlyEffects, out FXInstance[] createdEffects, float multiplier = 1)
		{
			if (effects == null)
			{
				createdEffects = new FXInstance[0];
				return;
			}

			createdEffects = new FXInstance[effects.Length];
			for (int i = 0; i < effects.Length; i++)
			{
				createdEffects[i] = ExecuteFX(effects[i], target, allowPlayerOnlyEffects, multiplier);
			}
		}
		/// <summary>
		/// Allocates an array of FXInstances based on the given FXObject array.
		/// Allows the List to be null.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static void ExecuteFX(FXObject[] effects, Transform target, bool allowPlayerOnlyEffects, ref List<FXInstance> referencedInstanceList, float multiplier = 1)
		{
			if (effects == null)
				return;
			if (referencedInstanceList == null)
				referencedInstanceList = new List<FXInstance>(effects.Length);

			//Make sure the list does not have to resize while adding the new instances
			referencedInstanceList.Capacity = System.Math.Max(referencedInstanceList.Capacity, referencedInstanceList.Count + effects.Length);

			for (int i = 0; i < effects.Length; i++)
			{
				referencedInstanceList.Add(ExecuteFX(effects[i], target, allowPlayerOnlyEffects, multiplier));
			}
		}
		/// <summary>
		/// Allocates an array of FXInstances based on the given CustomFXObject array.
		/// Allows the List to be null.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static void ExecuteFX(CustomFXObject[] effects, Transform target, bool allowPlayerOnlyEffects, ref List<FXInstance> referencedInstanceList, float multiplier = 1)
		{
			if (effects == null)
				return;
			if (referencedInstanceList == null)
				referencedInstanceList = new List<FXInstance>(effects.Length);

			//Make sure the list does not have to resize while adding the new instances
			referencedInstanceList.Capacity = System.Math.Max(referencedInstanceList.Capacity, referencedInstanceList.Count + effects.Length);

			for (int i = 0; i < effects.Length; i++)
			{
				referencedInstanceList.Add(ExecuteFX(effects[i], target, allowPlayerOnlyEffects, multiplier));
			}
		}
		#region Base Execution of FXObjects
		/// <summary>
		/// Creates and returns a FXInstance based on the given CustomFXObject.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static FXInstance ExecuteFX(CustomFXObject effect, Transform target, bool allowPlayerOnlyEffects, float multiplier = 1) {
			return ExecuteFX(	effect.FX,
								target,
								allowPlayerOnlyEffects,
								multiplier,
								effect.HasPositionOffset ? ((Vector3?)effect.PositionOffset) : null,
								effect.HasRotationOffset ? ((Vector3?)effect.RotationOffset) : null,
								effect.HasCustomScale ? ((Vector3?)effect.CustomScale) : null);
		}
		/// <summary>
		/// Creates and returns a FXInstance based on the given FXObject.
		/// </summary>
		/// <param name="effects"></param>
		/// <param name="target"></param>
		/// <param name="allowPlayerOnlyEffects"></param>
		/// <param name="multiplier"></param>
		public static FXInstance ExecuteFX(FXObject effect, Transform target, bool allowPlayerOnlyEffects, float multiplier = 1, Vector3? customOffset = null, Vector3? customRotationOffset = null, Vector3? customScale = null)
		{
			if (allowPlayerOnlyEffects)
			{
				if (effect is ScreenShake)
				{
					return EffectManager.ShakeScreen(effect as ScreenShake, multiplier);
				}
				else if (effect is ControllerRumble)
				{
					return EffectManager.RumbleController(effect as ControllerRumble, multiplier);
				}
				else if (effect is ScreenBlur)
				{
					return EffectManager.BlurScreen(effect as ScreenBlur, multiplier);
				}
				else if (effect is ScreenBorderColor)
				{
					return EffectManager.ColorScreenBorder(effect as ScreenBorderColor, multiplier);
				}
				else if (effect is ScreenTint)
				{
					return EffectManager.TintScreen(effect as ScreenTint, multiplier);
				}
				else if (effect is CameraFOVWarp)
				{
					return EffectManager.WarpCameraFOV(effect as CameraFOVWarp, multiplier);
				}
				else if (effect is CustomUI)
				{
					return EffectManager.PlayCustomUI(effect as CustomUI, multiplier);
				}
				else if (effect is Notification)
				{
					return EffectManager.ShowNotification(effect as Notification, multiplier);
				}
				else if (effect is Dialogue)
				{
					return EffectManager.ShowDialogue(effect as Dialogue);
				}
			}

			if (effect is TimeDilation)
			{
				return EffectManager.DilateTime(effect as TimeDilation, multiplier);
			}
			else if (effect is SoundEffect)
			{
				return EffectManager.PlaySound(effect as SoundEffect, target, customOffset, multiplier);
			}
			else if (effect is ParticleEffect)
			{
				return EffectManager.PlayParticleEffect(effect as ParticleEffect, target, customOffset, customRotationOffset, customScale, multiplier);
			}
			return null;
		}
		#endregion
		#endregion
		#region FXFinish
		public static void FinishFX(ref FXInstance[] toFinish)
		{
			if (toFinish == null || toFinish.Length == 0)
				return;

			int size = toFinish.Length;
			for (int i = 0; i < size; i++)
			{
				if (toFinish[i] != null)
				{
					toFinish[i].FinishFX();
				}
			}
			toFinish = null;
		}
		public static void FinishFX(ref List<FXInstance> toFinish)
		{
			if (toFinish == null || toFinish.Count == 0)
				return;

			int size = toFinish.Count;
			for (int i = 0; i < size; i++)
			{
				if (toFinish[i] != null)
				{
					toFinish[i].FinishFX();
				}
			}
			toFinish = null;
		}
		#endregion
	}
}
