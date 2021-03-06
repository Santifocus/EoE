﻿using UnityEditor;
using UnityEngine;
using EoE.Information;
using EoE.Combatery;
using System.Collections.Generic;
using EoE.Information.Logic;

namespace EoE
{
	public static class EoEEditor
	{
		private const int LINE_HEIGHT = 2;
		private const float STANDARD_OFFSET = 15;
		public static bool isDirty;

		#region Standard Drawing
		public static bool FloatField(string content, ref float curValue, int offSet = 0) => FloatField(new GUIContent(content), ref curValue, offSet);
		public static bool FloatField(GUIContent content, ref float curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			float newValue = EditorGUILayout.FloatField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool FloatSliderField(string content, ref float curValue, float minValue, float maxValue, int offSet = 0) => FloatSliderField(new GUIContent(content), ref curValue, minValue, maxValue, offSet);
		public static bool FloatSliderField(GUIContent content, ref float curValue, float minValue, float maxValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			float newValue = EditorGUILayout.Slider(content, curValue, minValue, maxValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool IntField(string content, ref int curValue, int offSet = 0) => IntField(new GUIContent(content), ref curValue, offSet);
		public static bool IntField(GUIContent content, ref int curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			int newValue = EditorGUILayout.IntField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool IntSliderField(string content, ref int curValue, int minValue, int maxValue, int offSet = 0) => IntSliderField(new GUIContent(content), ref curValue, minValue, maxValue, offSet);
		public static bool IntSliderField(GUIContent content, ref int curValue, int minValue, int maxValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			int newValue = EditorGUILayout.IntSlider(content, curValue, minValue, maxValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DelayedIntField(string content, ref int curValue, int offSet = 0) => DelayedIntField(new GUIContent(content), ref curValue, offSet);
		public static bool DelayedIntField(GUIContent content, ref int curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			int newValue = EditorGUILayout.DelayedIntField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DoubleField(string content, ref double curValue, int offSet = 0) => DoubleField(new GUIContent(content), ref curValue, offSet);
		public static bool DoubleField(GUIContent content, ref double curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			double newValue = EditorGUILayout.DoubleField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool StringField(string content, ref string curValue, int offSet = 0) => StringField(new GUIContent(content), ref curValue, offSet);
		public static bool StringField(GUIContent content, ref string curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			string newValue = EditorGUILayout.TextField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool BoolField(string content, ref bool curValue, int offSet = 0) => BoolField(new GUIContent(content), ref curValue, offSet);
		public static bool BoolField(GUIContent content, ref bool curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			bool newValue = EditorGUILayout.Toggle(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector2Field(string content, ref Vector2 curValue, int offSet = 0) => Vector2Field(new GUIContent(content), ref curValue, offSet);
		public static bool Vector2Field(GUIContent content, ref Vector2 curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			Vector2 newValue = EditorGUILayout.Vector2Field(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector3Field(string content, ref Vector3 curValue, int offSet = 0) => Vector3Field(new GUIContent(content), ref curValue, offSet);
		public static bool Vector3Field(GUIContent content, ref Vector3 curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			Vector3 newValue = EditorGUILayout.Vector3Field(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool Vector4Field(string content, ref Vector4 curValue, int offSet = 0) => Vector4Field(new GUIContent(content), ref curValue, offSet);
		public static bool Vector4Field(GUIContent content, ref Vector4 curValue, int offSet = 0)
		{
			bool changed = false;
			Vector4 newValue = curValue;

			float preFieldWidth = EditorGUIUtility.fieldWidth;
			EditorGUIUtility.fieldWidth = STANDARD_OFFSET * 4;

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space((offSet + 3) * STANDARD_OFFSET); 
			newValue.y = EditorGUILayout.FloatField(" ", newValue.y);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			newValue.x = EditorGUILayout.FloatField(content, newValue.x);

			GUILayout.Space(STANDARD_OFFSET * 1.3f);
			newValue.z = EditorGUILayout.FloatField("", newValue.z, GUILayout.MaxWidth(EditorGUIUtility.fieldWidth));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space((offSet + 3) * STANDARD_OFFSET);
			newValue.w = EditorGUILayout.FloatField(" ", newValue.w);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < 4; i++)
			{
				if(curValue[i] != newValue[i])
				{
					changed = true;
					curValue[i] = newValue[i];
				}
			}

			EditorGUIUtility.fieldWidth = preFieldWidth;

			if (changed)
			{
				isDirty = true;
			}
			return changed;
		}
		public static bool ColorField(string content, ref Color curValue, int offSet = 0) => ColorField(new GUIContent(content), ref curValue, offSet);
		public static bool ColorField(GUIContent content, ref Color curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			Color newValue = EditorGUILayout.ColorField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool GradientField(string content, ref Gradient curValue, int offSet = 0) => GradientField(new GUIContent(content), ref curValue, offSet);
		public static bool GradientField(GUIContent content, ref Gradient curValue, int offSet = 0)
		{
			//Create a copy so we can compare them afterwards
			Gradient newValue = new Gradient();
			newValue.colorKeys = curValue.colorKeys;
			newValue.alphaKeys = curValue.alphaKeys;

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			newValue = EditorGUILayout.GradientField(content, newValue);
			EditorGUILayout.EndHorizontal();

			if (!newValue.Equals(curValue))
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool AnimationCurveField(string content, ref AnimationCurve curValue, int offSet = 0) => AnimationCurveField(new GUIContent(content), ref curValue, offSet);
		public static bool AnimationCurveField(GUIContent content, ref AnimationCurve curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			//Create a copy so we can compare them afterwards
			AnimationCurve newValue = new AnimationCurve(curValue.keys);
			newValue = EditorGUILayout.CurveField(content, newValue);
			EditorGUILayout.EndHorizontal();

			if (!newValue.Equals(curValue))
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool EnumField<T>(string content, ref T curValue, int offSet = 0) where T : System.Enum => EnumField(new GUIContent(content), ref curValue, offSet);
		public static bool EnumField<T>(GUIContent content, ref T curValue, int offSet = 0) where T : System.Enum
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			T newValue = (T)EditorGUILayout.EnumPopup(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue.GetHashCode() != curValue.GetHashCode())
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupField(string content, ref int curValue, string[] options, int offSet = 0) => PopupField(new GUIContent(content), ref curValue, options, offSet);
		public static bool PopupField(GUIContent content, ref int curValue, string[] options, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			int newValue = EditorGUILayout.Popup(content, curValue, options);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool PopupField(string content, ref int curValue, string[] options, int[] optionValues, int offSet = 0) => PopupField(new GUIContent(content), ref curValue, options, optionValues, offSet);
		public static bool PopupField(GUIContent content, ref int curValue, string[] options, int[] optionValues, int offSet = 0)
		{
			GUIContent[] displayedOptions = new GUIContent[options.Length];
			for(int i = 0; i < options.Length; i++)
			{
				displayedOptions[i] = new GUIContent(options[i]);
			}
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			int newValue = EditorGUILayout.IntPopup(content, curValue, displayedOptions, optionValues);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool EnumFlagField<T>(string content, ref T curValue, int offSet = 0) where T : System.Enum => EnumFlagField(new GUIContent(content), ref curValue, offSet);
		public static bool EnumFlagField<T>(GUIContent content, ref T curValue, int offSet = 0) where T : System.Enum
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			T newValue = (T)EditorGUILayout.EnumFlagsField(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue.GetHashCode() != curValue.GetHashCode())
			{
				isDirty = true;
				curValue = (T)newValue;
				return true;
			}
			return false;
		}
		public static bool ObjectField<T>(string content, ref T curValue, int offSet = 0) where T : Object => ObjectField<T>(new GUIContent(content), ref curValue, offSet);
		public static bool ObjectField<T>(GUIContent content, ref T curValue, int offSet = 0) where T : Object
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			T newValue = (T)EditorGUILayout.ObjectField(content, curValue, typeof(T), false);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		#endregion
		#region Array Drawing
		public static bool DrawArray<T>(GUIContent content, ref T[] curValue, SerializedProperty arrayProperty, System.Action<GUIContent, T, SerializedProperty, int> elementBinding, GUIContent elementContent = null, int offSet = 0, bool asHeader = false)
		{
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			changed |= Foldout(content, arrayProperty, offSet, asHeader);

			if (arrayProperty.isExpanded)
			{
				elementContent = elementContent ?? new GUIContent(". Element");

				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize, offSet + 1);

				for (int i = 0; i < curValue.Length; i++)
				{
					elementBinding?.Invoke(new GUIContent((i + 1) + elementContent.text, elementContent.tooltip), curValue[i], arrayProperty.GetArrayElementAtIndex(i), offSet + 1);
				}

				if (curValue.Length != newSize)
				{
					changed = true;
					T[] newArray = new T[newSize];
					for (int i = 0; i < newSize; i++)
					{
						if (i < curValue.Length)
							newArray[i] = curValue[i];
						else
							break;
					}
					curValue = newArray;
					arrayProperty.arraySize = newSize;
				}
			}
			if (asHeader)
			{
				EndFoldoutHeader();
			}

			if (changed)
			{
				isDirty = true;
			}
			return changed;
		}
		public static bool DrawArray<T>(GUIContent content, ref T[] curValue, SerializedProperty arrayProperty, System.Action<GUIContent, T, SerializedProperty, int> elementBinding, System.Func<int, GUIContent> contentGetter, int offSet = 0, bool asHeader = false)
		{
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			changed |= Foldout(content, arrayProperty, offSet, asHeader);

			if (arrayProperty.isExpanded)
			{
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize, offSet + 1);

				for (int i = 0; i < curValue.Length; i++)
				{
					GUIContent elementContent = contentGetter?.Invoke(i) ?? new GUIContent(". Element");
					elementBinding?.Invoke(elementContent, curValue[i], arrayProperty.GetArrayElementAtIndex(i), offSet + 1);
				}

				if (curValue.Length != newSize)
				{
					changed = true;
					T[] newArray = new T[newSize];
					for (int i = 0; i < newSize; i++)
					{
						if (i < curValue.Length)
							newArray[i] = curValue[i];
						else
							break;
					}
					curValue = newArray;
					arrayProperty.arraySize = newSize;
				}
			}
			if (asHeader)
			{
				EndFoldoutHeader();
			}

			if (changed)
			{
				isDirty = true;
			}
			return changed;
		}
		public static bool ObjectArrayField<T>(GUIContent content, ref T[] curValue, SerializedProperty property, GUIContent objectContent = null, int offSet = 0, bool asHeader = false) where T : Object
		{
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}
			changed |= Foldout(content, property, offSet, asHeader);

			if (property.isExpanded)
			{
				int newSize = curValue.Length;
				DelayedIntField("Size", ref newSize, offSet + 1);

				objectContent = objectContent ?? new GUIContent(". Element");
				for (int i = 0; i < curValue.Length; i++)
				{
					changed |= ObjectField(new GUIContent((i + 1) + objectContent.text, objectContent.tooltip), ref curValue[i], offSet + 1);
				}

				if (newSize != curValue.Length)
				{
					changed = true;
					T[] newArray = new T[newSize];
					for (int i = 0; i < newSize; i++)
					{
						if (i < curValue.Length)
							newArray[i] = curValue[i];
						else
							break;
					}

					curValue = newArray;
				}
			}

			if (asHeader)
				EndFoldoutHeader();

			if (changed)
				isDirty = true;

			return changed;
		}
		#endregion
		#region Layout Drawer
		public static void Header(string content, int offSet = 0, bool spaces = true, bool bold = true) => Header(new GUIContent(content), offSet, spaces, bold);
		public static void Header(GUIContent content, int offSet = 0, bool spaces = true, bool bold = true)
		{
			if (spaces)
				GUILayout.Space(8);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			if (bold)
				GUILayout.Label(content, EditorStyles.boldLabel);
			else
				GUILayout.Label(content);
			EditorGUILayout.EndHorizontal();

			if (spaces)
				GUILayout.Space(4);
		}
		public static void LineBreak(Color col, bool spaces = true)
		{
			if (spaces)
				GUILayout.Space(3);

			Rect rect = EditorGUILayout.GetControlRect(false, LINE_HEIGHT);
			rect.height = LINE_HEIGHT;
			rect.x /= 2;
			EditorGUI.DrawRect(rect, col);

			if (spaces)
				GUILayout.Space(3);
		}
		public static bool Foldout(string content, SerializedProperty property, int offSet = 0, bool asHeader = false)
		{
			return Foldout(new GUIContent(content), property, offSet, asHeader);
		}
		public static bool Foldout(GUIContent content, SerializedProperty property, int offSet = 0, bool asHeader = false)
		{
			bool curOpen = property.isExpanded;
			if(Foldout(content, ref curOpen, offSet, asHeader))
			{
				property.isExpanded = curOpen;
				return true;
			}
			return false;
		}
		public static bool Foldout(string content, ref bool curValue, int offSet = 0, bool asHeader = false) => Foldout(new GUIContent(content), ref curValue, offSet, asHeader);
		public static bool Foldout(GUIContent content, ref bool curValue, int offSet = 0, bool asHeader = false)
		{
			bool newValue;
			if (asHeader)
			{
				newValue = EditorGUILayout.BeginFoldoutHeaderGroup(curValue, content);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(offSet * STANDARD_OFFSET);
				newValue = EditorGUILayout.Foldout(curValue, content, true);
				EditorGUILayout.EndHorizontal();
			}

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool DrawInFoldoutHeader(string content, SerializedProperty property, System.Action drawFunction)
		{
			return DrawInFoldoutHeader(new GUIContent(content), property, drawFunction);
		}
		public static bool DrawInFoldoutHeader(GUIContent content, SerializedProperty property, System.Action drawFunction)
		{
			bool curOpen = property.isExpanded;
			if (DrawInFoldoutHeader(content, ref curOpen, drawFunction))
			{
				property.isExpanded = curOpen;
				return true;
			}
			return false;
		}
		public static bool DrawInFoldoutHeader(string content, ref bool curValue, System.Action drawFunction) => DrawInFoldoutHeader(new GUIContent(content), ref curValue, drawFunction);
		public static bool DrawInFoldoutHeader(GUIContent content, ref bool curValue, System.Action drawFunction)
		{
			bool changed = Foldout(content, ref curValue, 0, true);
			if (curValue)
			{
				drawFunction?.Invoke();
			}
			EndFoldoutHeader();
			return changed;
		}
		public static void EndFoldoutHeader() => EditorGUILayout.EndFoldoutHeaderGroup();
		#endregion
		#region DrawCustoms
		public static void DrawCombatObjectBase(CombatObject settings, SerializedObject serializedObject, int offSet)
		{
			BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.NoDefinedActionType))), ref settings.NoDefinedActionType, 1);
			if (!settings.NoDefinedActionType)
				EnumField<ActionType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ActionType))), ref settings.ActionType, 1);

			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BasePhysicalDamage))), ref settings.BasePhysicalDamage, offSet);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseMagicalDamage))), ref settings.BaseMagicalDamage, offSet);
			FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseKnockback))), ref settings.BaseKnockback, offSet);
			FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BaseCritChance))), ref settings.BaseCritChance, 0, 1, offSet);

			LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
			DrawObjectCost(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Cost))), ref settings.Cost, serializedObject.FindProperty(nameof(settings.Cost)), offSet);
		}
		public static void DrawObjectCost(GUIContent content, ref ObjectCost settings, SerializedProperty property, int offSet)
		{
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Health))), ref settings.Health, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Mana))), ref settings.Mana, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Stamina))), ref settings.Stamina, offSet + 1);

				ObjectArrayField<LogicComponent>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AdditionalConditions))), ref settings.AdditionalConditions, property.FindPropertyRelative(nameof(settings.AdditionalConditions)), new GUIContent(". Condtion"), offSet + 1);
			}
		}
		public static void DrawActivationEffect(GUIContent content, ActivationEffect settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
			{
				isDirty = true;
				settings = new ActivationEffect();
			}
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				//Activation Info
				FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChanceToActivate))), ref settings.ChanceToActivate, 0, 1, offSet + 1);
				EnumFlagField<EffectType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ContainedEffectType))), ref settings.ContainedEffectType, offSet + 1);
				LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);

				//Impulse Velocity
				if (settings.HasMaskFlag(EffectType.ImpulseVelocity))
				{
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ImpulseVelocity))), ref settings.ImpulseVelocity, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ImpulseVelocityFallOffTime))), ref settings.ImpulseVelocityFallOffTime, offSet + 1);

					EnumField<InherritDirection>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ImpulseVelocityDirection))), ref settings.ImpulseVelocityDirection, offSet + 1);
					if (settings.ImpulseVelocityDirection == InherritDirection.Target)
					{
						EnumField<InherritDirection>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ImpulseVelocityFallbackDirection))), ref settings.ImpulseVelocityFallbackDirection, offSet + 1);
						if (settings.ImpulseVelocityFallbackDirection == InherritDirection.Target)
							settings.ImpulseVelocityFallbackDirection = InherritDirection.Local;
					}

					EnumField<DirectionBase>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ImpulseDirectionBase))), ref settings.ImpulseDirectionBase, offSet + 2);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//FX
				if (settings.HasMaskFlag(EffectType.FX))
				{
					SerializedProperty fxProperty = property.FindPropertyRelative(nameof(settings.FXObjects));
					DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FXObjects))), ref settings.FXObjects, fxProperty, DrawCustomFXObject, new GUIContent(". Effect"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//AOE
				if (settings.HasMaskFlag(EffectType.AOE))
				{
					SerializedProperty aoeProperty = property.FindPropertyRelative(nameof(settings.AOEEffects));
					ObjectArrayField<EffectAOE>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AOEEffects))), ref settings.AOEEffects, aoeProperty, new GUIContent(". AOE"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//Projectile
				if (settings.HasMaskFlag(EffectType.CreateProjectile))
				{
					SerializedProperty projectileArrayProperty = property.FindPropertyRelative(nameof(settings.ProjectileInfos));
					DrawArray<ProjectileInfo>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ProjectileInfos))), ref settings.ProjectileInfos, projectileArrayProperty, DrawProjectileInfo, new GUIContent(". Projectile"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//Heal Effects
				if (settings.HasMaskFlag(EffectType.HealOnCreator))
				{
					SerializedProperty healEffectArrayProperty = property.FindPropertyRelative(nameof(settings.HealsOnUser));
					DrawArray<HealTargetInfo>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HealsOnUser))), ref settings.HealsOnUser, healEffectArrayProperty, DrawHealTargetInfo, new GUIContent(". Heal Effect"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//Buffs
				if (settings.HasMaskFlag(EffectType.BuffOnCreator))
				{
					SerializedProperty buffArrayProperty = property.FindPropertyRelative(nameof(settings.BuffsOnUser));
					ObjectArrayField<Buff>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BuffsOnUser))), ref settings.BuffsOnUser, buffArrayProperty, new GUIContent(". Buff"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//Remenants
				if (settings.HasMaskFlag(EffectType.CreateRemenants))
				{
					SerializedProperty remenantsArrayProperty = property.FindPropertyRelative(nameof(settings.CreatedRemenants));
					ObjectArrayField<RemnantsData>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CreatedRemenants))), ref settings.CreatedRemenants, remenantsArrayProperty, new GUIContent(". Remenant"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
				//PlayerItemChange
				if (settings.HasMaskFlag(EffectType.PlayerItemChange))
				{
					SerializedProperty itemChangeArrayProperty = property.FindPropertyRelative(nameof(settings.PlayerItemChanges));
					DrawArray<PlayerItemChange>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PlayerItemChanges))), ref settings.PlayerItemChanges, itemChangeArrayProperty, DrawPlayerItemChange, new GUIContent(". Item"), offSet + 1);
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1), false);
				}
			}
		}
		public static void DrawRestrictionData(GUIContent content, RestrictionData settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
			{
				settings = new RestrictionData();
				isDirty = true;
			}

			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				EnumFlagField<ActionType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ActionRestrictions))), ref settings.ActionRestrictions, offSet + 1);
				EnumFlagField<MovementType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MovementRestrictions))), ref settings.MovementRestrictions, offSet + 1);
			}
		}
		public static void DrawProjectileInfo(GUIContent content, ProjectileInfo settings, SerializedProperty property, int offSet)
		{
			if(settings == null)
			{
				settings = new ProjectileInfo();
				isDirty = true;
			}
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				ObjectField<ProjectileData>(new GUIContent("ProjectileData"), ref settings.Projectile, offSet + 1);

				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ExecutionDelay))), ref settings.ExecutionDelay, offSet + 1);

				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ExecutionCount))), ref settings.ExecutionCount, offSet + 1);
				if (settings.ExecutionCount > 1)
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ExecutionRepeatDelay))), ref settings.ExecutionRepeatDelay, offSet + 1);
			}
		}
		public static void DrawHealTargetInfo(GUIContent content, HealTargetInfo settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
			{
				settings = new HealTargetInfo();
				isDirty = true;
			}
			Foldout(content, property, offSet);
			if (property.isExpanded)
			{
				EnumField<TargetStat>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HealType))), ref settings.HealType, offSet + 1);
				BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Percent))), ref settings.Percent, offSet + 1);
				FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Amount))), ref settings.Amount, offSet + 1);
			}
		}
		public static void DrawCustomFXObject(GUIContent content, CustomFXObject settings, SerializedProperty selfProperty, int offSet)
		{
			bool changed = false;
			if (settings == null)
			{
				settings = new CustomFXObject();
				changed = true;
			}

			Foldout(content, selfProperty, offSet);

			if (selfProperty.isExpanded)
			{
				changed |= ObjectField<FXObject>("Effect", ref settings.FX, offSet + 1);
				changed |= NullableVector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasPositionOffset))),
												new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.PositionOffset))),
												ref settings.PositionOffset,
												ref settings.HasPositionOffset,
												offSet + 1);
				changed |= NullableVector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasRotationOffset))),
												new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.RotationOffset))),
												ref settings.RotationOffset,
												ref settings.HasRotationOffset,
												offSet + 1);
				changed |= NullableVector3Field(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HasCustomScale))),
												new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CustomScale))),
												ref settings.CustomScale,
												ref settings.HasCustomScale,
												offSet + 1);
			}
			if (changed)
				isDirty = true;
		}
		public static void DrawWaitSettings(GUIContent content, WaitSetting settings, SerializedProperty selfProperty, int offSet)
		{
			if(settings == null)
			{
				settings = new WaitSetting();
				isDirty = true;
			}
			Foldout(content, selfProperty, offSet);

			if (selfProperty.isExpanded)
			{

				if (FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinAnimationPoint))), ref settings.MinAnimationPoint, 0, 1, offSet + 1))
				{
					if (settings.MaxAnimationPoint < settings.MinAnimationPoint)
					{
						settings.MaxAnimationPoint = settings.MinAnimationPoint;
						isDirty = true;
					}
				}
				if (FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaxAnimationPoint))), ref settings.MaxAnimationPoint, 0, 1, offSet + 1))
				{
					if (settings.MinAnimationPoint > settings.MaxAnimationPoint)
					{
						settings.MinAnimationPoint = settings.MaxAnimationPoint;
						isDirty = true;
					}
				}

				ObjectField<LogicComponent>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Condition))), ref settings.Condition, offSet + 1);
			}
		}
		public static void DrawPlayerItemChange(GUIContent content, PlayerItemChange settings, SerializedProperty property, int offSet)
		{
			if (settings == null)
			{
				isDirty = true;
				return;
			}
			Foldout(content, property, offSet);

			if (property.isExpanded)
			{
				if (settings.ItemCount == 0)
				{
					EditorGUILayout.HelpBox("No change. \n ItemCount > 0 => Gives items. \n ItemCount < 0 => Takes items", MessageType.Warning);
				}
				else if (settings.Item != null)
				{
					if (settings.ItemCount > 0)
					{
						if (settings.ForceEquip)
						{
							int rest = settings.ItemCount - settings.Item.MaxStack;
							if(rest > 0)
							{
								EditorGUILayout.HelpBox("Equips " + settings.Item.MaxStack + "x & gives " + rest + "x " + settings.Item.ItemName.text + "." , MessageType.Info);
							}
							else
							{
								EditorGUILayout.HelpBox("Equips " + settings.ItemCount + "x " + settings.Item.ItemName.text + ".", MessageType.Info);
							}
						}
						else
						{
							EditorGUILayout.HelpBox("Gives " + settings.ItemCount + "x " + settings.Item.ItemName.text + ".", MessageType.Info);
						}
					}
					else //settings.ItemCount < 0
					{
						EditorGUILayout.HelpBox("Takes " + (-settings.ItemCount) + "x " + settings.Item.ItemName.text + ".", MessageType.Info);
					} 
				}
				else
				{
					EditorGUILayout.HelpBox("No Item to change selected.", MessageType.Warning);
				}

				ObjectField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Item))), ref settings.Item, offSet + 1);
				IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ItemCount))), ref settings.ItemCount, offSet + 1);

				if(settings.ItemCount > 0)
				{
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ForceEquip))), ref settings.ForceEquip, offSet + 1);
				}
			}
		}
		public static bool NullableVector3Field(string content, string valueContent, ref Vector3 curValue, ref bool hasValue, int offSet = 0) => NullableVector3Field(new GUIContent(content), new GUIContent(valueContent), ref curValue, ref hasValue, offSet);
		public static bool NullableVector3Field(GUIContent hasValueContent, GUIContent valueContent, ref Vector3 curValue, ref bool hasValue, int offSet = 0)
		{
			bool changed = false;

			changed |= BoolField(hasValueContent, ref hasValue, offSet);
			if (hasValue)
				changed |= Vector3Field(valueContent, ref curValue, offSet + 1);

			return changed;
		}

		#region Attack Sequence Drawing
		public static class AttackSequenceDrawer
		{
			private static float CurMinCharge = 0;
			private static float CurMaxCharge = 0;
			public static void DrawAttackSequence(GUIContent content, AttackSequence settings, Weapon dataProvider, SerializedProperty property, int offSet, bool asHeader)
			{
				Foldout(content, property, offSet, asHeader);

				if (property.isExpanded)
				{
					SerializedProperty sequenceArray = property.FindPropertyRelative(nameof(settings.AttackSequenceParts));
					for (int i = 0; i < settings.AttackSequenceParts.Length; i++)
					{
						DrawAttackStyle(settings.AttackSequenceParts[i], dataProvider, sequenceArray.GetArrayElementAtIndex(i), i, offSet + 1);
						if (i < settings.AttackSequenceParts.Length - 1)
						{
							LineBreak(new Color(0.8f, 0.5f, 0f, 1));
							FloatField(new GUIContent("Maximum Delay"), ref settings.PartsMaxDelays[i], offSet + 1);
							LineBreak(new Color(0.8f, 0.5f, 0f, 1));
						}
					}

					LineBreak(new Color(0.25f, 0.25f, 0.25f, 1));
					//Array resizer buttons
					GUILayout.BeginHorizontal();
					if (GUILayout.Button("+"))
					{
						sequenceArray.InsertArrayElementAtIndex(settings.AttackSequenceParts.Length);
						List<AttackStyle> newContent = new List<AttackStyle>(settings.AttackSequenceParts);
						newContent.Add(new AttackStyle());
						settings.AttackSequenceParts = newContent.ToArray();

						if (settings.AttackSequenceParts.Length > 1)
						{
							property.FindPropertyRelative(nameof(settings.PartsMaxDelays)).InsertArrayElementAtIndex(settings.PartsMaxDelays.Length);
							List<float> newDelayContent = new List<float>(settings.PartsMaxDelays);
							newDelayContent.Add(0);
							settings.PartsMaxDelays = newDelayContent.ToArray();
						}

						isDirty = true;
					}
					EditorGUI.BeginDisabledGroup(settings.AttackSequenceParts.Length == 0);
					if (GUILayout.Button("-"))
					{
						List<AttackStyle> newContent = new List<AttackStyle>(settings.AttackSequenceParts);
						newContent.RemoveAt(newContent.Count - 1);
						settings.AttackSequenceParts = newContent.ToArray();
						sequenceArray.arraySize = settings.AttackSequenceParts.Length;

						if (settings.AttackSequenceParts.Length > 1)
						{
							List<float> newDelayContent = new List<float>(settings.PartsMaxDelays);
							newDelayContent.RemoveAt(newDelayContent.Count - 1);
							settings.PartsMaxDelays = newDelayContent.ToArray();
							property.FindPropertyRelative(nameof(settings.PartsMaxDelays)).arraySize = settings.PartsMaxDelays.Length;
						}
						isDirty = true;
					}
					EditorGUI.EndDisabledGroup();
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
				}
				if (asHeader)
					EndFoldoutHeader();
			}
			private static void DrawAttackStyle(AttackStyle settings, Weapon dataProvider, SerializedProperty property, int index, int offSet)
			{
				Foldout(new GUIContent(IndexToName()), property, offSet);
				if (property.isExpanded)
				{
					//Animation
					Header("Animation Settings", offSet);
					EnumField<AttackAnimation>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationTarget))), ref settings.AnimationTarget, offSet + 1);
					EnumField<MultiplicationType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationMultiplicationType))), ref settings.AnimationMultiplicationType, offSet + 1);

					LineBreak(new Color(0.25f, 0.25f, 0.65f, 0.25f));
					FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationStartPoint))), ref settings.AnimationStartPoint, 0, 1, offSet + 1);
					if (settings.AnimationMultiplicationType == MultiplicationType.FlatValue)
					{
						if (FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationSpeedFlatValue))), ref settings.AnimationSpeedFlatValue, offSet + 1))
						{
							settings.AnimationSpeedFlatValue = Mathf.Max(settings.AnimationSpeedFlatValue, 0);
						}
					}
					else
					{
						AnimationCurveField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationSpeedCurve))), ref settings.AnimationSpeedCurve, offSet + 1);
						FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationSpeedCurveMultiplier))), ref settings.AnimationSpeedCurveMultiplier, offSet + 1);
						if (FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationSpeedCurveTimeframe))), ref settings.AnimationSpeedCurveTimeframe, offSet + 1))
						{
							settings.AnimationSpeedCurveTimeframe = Mathf.Max(settings.AnimationSpeedCurveTimeframe, 0);
						}
					}
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					DrawRestrictionData(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Restrictions))), settings.Restrictions, property.FindPropertyRelative(nameof(settings.Restrictions)), offSet + 1);

					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CausedCooldown))), ref settings.CausedCooldown, offSet + 1);

					//Wait Settings
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					SerializedProperty waitSettingsProperty = property.FindPropertyRelative(nameof(settings.WaitSettings));
					DrawArray<WaitSetting>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WaitSettings))), ref settings.WaitSettings, waitSettingsProperty, DrawWaitSettings, new GUIContent(". Wait Setting"), offSet + 1);

					//Charging
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					Header("Charge Settings", offSet);
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.NeedsCharging))), ref settings.NeedsCharging, offSet + 1);
					if (settings.NeedsCharging)
					{
						SerializedProperty chargeProperty = property.FindPropertyRelative(nameof(settings.ChargeSettings));
						DrawChargeSettings(settings.ChargeSettings, chargeProperty, offSet + 1);
					}

					//Multipliers
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					Header("Multiplier Settings", offSet);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DamageMultiplier))), ref settings.DamageMultiplier, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.HealthCostMultiplier))), ref settings.HealthCostMultiplier, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ManaCostMultiplier))), ref settings.ManaCostMultiplier, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StaminaCostMultiplier))), ref settings.StaminaCostMultiplier, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.KnockbackMultiplier))), ref settings.KnockbackMultiplier, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CritChanceMultiplier))), ref settings.CritChanceMultiplier, offSet + 1);

					//Collision
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					Header("Collision Settings", offSet);
					EnumFlagField<ColliderMask>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CollisionMask))), ref settings.CollisionMask, offSet + 1);
					EnumFlagField<ColliderMask>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StopOnCollisionMask))), ref settings.StopOnCollisionMask, offSet + 1);
					ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.DirectHit))), ref settings.DirectHit, offSet + 1);

					//Custom hitbox group drawing
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 0.5f));
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.UseCustomHitboxGroup))), ref settings.UseCustomHitboxGroup, offSet + 1);

					if (settings.UseCustomHitboxGroup)
					{
						if(dataProvider != null && dataProvider.WeaponPrefab)
						{
							if (settings.CustomHitboxGroup >= dataProvider.WeaponPrefab.customHitboxGroups.Length)
							{
								Debug.Log(settings.CustomHitboxGroup);
								settings.CustomHitboxGroup = dataProvider.WeaponPrefab.customHitboxGroups.Length - 1;
							}

							string[] options = new string[dataProvider.WeaponPrefab.customHitboxGroups.Length + 1];
							int[] optionValues = new int[dataProvider.WeaponPrefab.customHitboxGroups.Length + 1];

							options[0] = "Standard";
							optionValues[0] = -1;

							for (int i = 1; i < options.Length; i++)
							{
								options[i] = dataProvider.WeaponPrefab.customHitboxGroups[i - 1].Identifier;
								optionValues[i] = i - 1;
							}

							PopupField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.CustomHitboxGroup))), ref settings.CustomHitboxGroup, options, optionValues, offSet + 1);
						}
						else
						{
							EditorGUILayout.HelpBox("This object either does not support custom hitboxes or has no WeaponPrefab referenced.", MessageType.Warning);
						}
					}

					//Overrides
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					Header("Override Settings", offSet);
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OverrideElement))), ref settings.OverrideElement, offSet + 1);
					if (settings.OverrideElement)
						EnumField<ElementType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OverridenElement))), ref settings.OverridenElement, offSet + 2);
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OverrideCauseType))), ref settings.OverrideCauseType, offSet + 1);
					if (settings.OverrideCauseType)
						EnumField<CauseType>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OverridenCauseType))), ref settings.OverridenCauseType, offSet + 2);

					//Combo
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					Header("Combo Settings", offSet);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ComboIncreaseMaxDelay))), ref settings.ComboIncreaseMaxDelay, offSet + 1);
					IntField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.OnHitComboWorth))), ref settings.OnHitComboWorth, offSet + 1);

					//Attack effects
					GUILayout.Space(6);
					LineBreak(new Color(0.5f, 0.25f, 0.65f, 1), false);
					LineBreak(new Color(0.5f, 0.25f, 0.65f, 1), false);
					Header("Effects", offSet);

					DrawArray<AttackActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffectsOnUser))), ref settings.StartEffectsOnUser, property.FindPropertyRelative(nameof(settings.StartEffectsOnUser)), DrawAttackActivationEffect, new GUIContent(". Effect"), offSet + 1);
					DrawArray<AttackActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffectsOnUser))), ref settings.WhileEffectsOnUser, property.FindPropertyRelative(nameof(settings.WhileEffectsOnUser)), DrawAttackActivationEffect, new GUIContent(". Effect"), offSet + 1);
					DrawArray<AttackActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartEffectsOnWeapon))), ref settings.StartEffectsOnWeapon, property.FindPropertyRelative(nameof(settings.StartEffectsOnWeapon)), DrawAttackActivationEffect, new GUIContent(". Effect"), offSet + 1);
					DrawArray<AttackActivationEffect>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WhileEffectsOnWeapon))), ref settings.WhileEffectsOnWeapon, property.FindPropertyRelative(nameof(settings.WhileEffectsOnWeapon)), DrawAttackActivationEffect, new GUIContent(". Effect"), offSet + 1);
				}
				string IndexToName()
				{
					if (index == 0)
					{
						return "Start Attack";
					}
					else
					{
						return index + ". Combo";
					}
				}
			}
			private static void DrawChargeSettings(AttackChargeSettings settings, SerializedProperty property, int offSet)
			{
				Foldout(new GUIContent("Charge Settings"), property, offSet);
				if (property.isExpanded)
				{
					EnumFlagField<AttackChargeEffectMask>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.EffectMask))), ref settings.EffectMask, offSet + 1);

					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AnimationChargeStartpoint))), ref settings.AnimationChargeStartpoint, 0, 1, offSet + 1);
					BoolField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.WaitAtFullChargeForRelease))), ref settings.WaitAtFullChargeForRelease, offSet + 1);
					DrawRestrictionData(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.Restrictions))), settings.Restrictions, property.FindPropertyRelative(nameof(settings.Restrictions)), offSet + 1);

					//Charge values
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChargeTime))), ref settings.ChargeTime, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.StartCharge))), ref settings.StartCharge, offSet + 1);
					FloatField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MaximumCharge))), ref settings.MaximumCharge, offSet + 1);

					FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.MinRequiredCharge))), ref settings.MinRequiredCharge, settings.StartCharge, settings.MaximumCharge, offSet + 1);

					//DirectHit overrides
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					SerializedProperty hitOverridesProperty = property.FindPropertyRelative(nameof(settings.ChargeBasedDirectHits));
					CurMinCharge = settings.MinRequiredCharge;
					CurMaxCharge = settings.MaximumCharge;
					DrawArray<ChargeBasedDirectHit>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.ChargeBasedDirectHits))), ref settings.ChargeBasedDirectHits, hitOverridesProperty, DrawDirectHitOverride, new GUIContent(". Charge Based Direct Hit"), offSet + 1);

					//FX
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					SerializedProperty fxProperty = property.FindPropertyRelative(nameof(settings.FXObjects));
					DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FXObjects))), ref settings.FXObjects, fxProperty, DrawCustomFXObject, new GUIContent(". Effect"), offSet + 1);

					SerializedProperty fxMultipliedProperty = property.FindPropertyRelative(nameof(settings.FXObjectsWithMutliplier));
					DrawArray<CustomFXObject>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.FXObjectsWithMutliplier))), ref settings.FXObjectsWithMutliplier, fxMultipliedProperty, DrawCustomFXObject, new GUIContent(". Effect"), offSet + 1);

					//Buffs
					LineBreak(new Color(0.25f, 0.25f, 0.65f, 1));
					SerializedProperty buffProperty = property.FindPropertyRelative(nameof(settings.BuffOnUserWhileCharging));
					ObjectArrayField<Buff>(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.BuffOnUserWhileCharging))), ref settings.BuffOnUserWhileCharging, buffProperty, new GUIContent(". Buff"), offSet + 1);
				}
			}
			private static void DrawAttackActivationEffect(GUIContent content, AttackActivationEffect settings, SerializedProperty property, int offSet)
			{
				if (settings == null)
				{
					settings = new AttackActivationEffect();
					isDirty = true;
				}
				Foldout(content, property, offSet);
				if (property.isExpanded)
				{
					FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(settings.AtAnimationPoint))), ref settings.AtAnimationPoint, 0, 1, offSet + 1);
					DrawActivationEffect(new GUIContent("Effect Data"), settings.Effect, property.FindPropertyRelative(nameof(settings.Effect)), offSet + 1);
				}
			}
			private static void DrawDirectHitOverride(GUIContent content, ChargeBasedDirectHit selfSettings, SerializedProperty selfProperty, int offSet)
			{
				if (selfSettings == null)
				{
					isDirty = true;
					selfSettings = new ChargeBasedDirectHit();
				}

				Foldout(content, selfProperty, offSet);
				if (selfProperty.isExpanded)
				{
					FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(selfSettings.MinRequiredCharge))), ref selfSettings.MinRequiredCharge, CurMinCharge, CurMaxCharge, offSet + 1);
					FloatSliderField(new GUIContent(ObjectNames.NicifyVariableName(nameof(selfSettings.MaxRequiredCharge))), ref selfSettings.MaxRequiredCharge, selfSettings.MinRequiredCharge, CurMaxCharge, offSet + 1);

					GUILayout.Space(4);
					ObjectField<EffectSingle>(new GUIContent(ObjectNames.NicifyVariableName(nameof(selfSettings.DirectHitOverride))), ref selfSettings.DirectHitOverride, offSet + 1);
				}
			}
		}
		#endregion
		#endregion
	}
}