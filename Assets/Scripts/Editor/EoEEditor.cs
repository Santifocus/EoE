using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EoE
{
	public class EoEEditor
	{
		private const string LINE_BREAK = "_________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________";
		private const float STANDARD_OFFSET = 20;
		public static bool isDirty;
		public static void Header(string content, int offSet = 0, bool spaces = true, bool bold = true) => Header(new GUIContent(content), offSet, spaces, bold);
		public static void Header(GUIContent content, int offSet = 0, bool spaces = true, bool bold = true)
		{
			if(spaces)
				GUILayout.Space(8);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			if(bold)
				GUILayout.Label(content, EditorStyles.boldLabel);
			else
				GUILayout.Label(content);
			EditorGUILayout.EndHorizontal();

			if (spaces)
				GUILayout.Space(4);
		}
		public static void LineBreak()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(-2 * STANDARD_OFFSET);
			EditorGUILayout.LabelField(LINE_BREAK);
			EditorGUILayout.EndHorizontal();
		}
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
		public static bool Foldout(string content, ref bool curValue, int offSet = 0) => Foldout(new GUIContent(content), ref curValue, offSet);
		public static bool Foldout(GUIContent content, ref bool curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			bool newValue = EditorGUILayout.Foldout(curValue, content, true);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static bool FoldoutHeader(string content, ref bool curValue) => FoldoutHeader(new GUIContent(content), ref curValue);
		public static bool FoldoutHeader(GUIContent content, ref bool curValue)
		{
			bool newValue = EditorGUILayout.BeginFoldoutHeaderGroup(curValue, content);

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
		public static void EndFoldoutHeader() => EditorGUILayout.EndFoldoutHeaderGroup();
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
		public static bool EnumField(GUIContent content, ref System.Enum curValue, int offSet = 0)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(offSet * STANDARD_OFFSET);
			System.Enum newValue = EditorGUILayout.EnumPopup(content, curValue);
			EditorGUILayout.EndHorizontal();

			if (newValue != curValue)
			{
				isDirty = true;
				curValue = newValue;
				return true;
			}
			return false;
		}
	}
}