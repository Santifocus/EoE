using System.IO;
using UnityEditor;
using UnityEngine;

namespace EoE
{
	public static class EoEEditor
	{
		private const string LINE_BREAK = "――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――――";
		private const float STANDARD_OFFSET = 15;
		public static bool isDirty;
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
		public static void LineBreak()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(-2 * STANDARD_OFFSET);
			EditorGUILayout.LabelField(LINE_BREAK);
			EditorGUILayout.EndHorizontal();
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
		public static bool ObjectArrayField<T>(string content, ref T[] curValue, ref bool open, GUIContent objectContent = null, int offSet = 0) where T : Object => ObjectArrayField<T>(new GUIContent(content), ref curValue, ref open, objectContent, offSet);
		public static bool ObjectArrayField<T>(GUIContent content, ref T[] curValue, ref bool open, GUIContent objectContent = null, int offSet = 0) where T : Object
		{
			bool changed = false;
			if(curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			Foldout(content, ref open, offSet);
			if (open)
			{
				GUIContent targetContent = objectContent != null ? objectContent : new GUIContent("Element ");
				for(int i = 0; i < curValue.Length; i++)
				{
					changed |= ObjectField(new GUIContent(targetContent.text + i, targetContent.tooltip), ref curValue[i], offSet + 1);
				}

				GUILayout.Space(1);
				int newSize = curValue.Length;
				bool sizeChanged = IntField("Size", ref newSize, offSet + 1);
				if (sizeChanged)
				{
					changed = true;
					T[] newArray = new T[newSize];
					for(int i = 0; i < newSize; i++)
					{
						if (i < curValue.Length)
							newArray[i] = curValue[i];
						else
							break;
					}

					curValue = newArray;
				}
			}
			if (changed)
				isDirty = true;
			return changed;
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
		public static bool DrawArray<T>(GUIContent arrayContent, System.Func<int, int, T[], bool> elementBinding, ref T[] curValue, ref bool open, int offSet = 0, bool asHeader = false)
		{
			bool changed = false;
			if (curValue == null)
			{
				curValue = new T[0];
				changed = true;
			}

			if (asHeader)
				changed |= FoldoutHeader(arrayContent, ref open);
			else
				changed |= Foldout(arrayContent, ref open, offSet);
			if (open)
			{
				for(int i = 0; i < curValue.Length; i++)
				{
					changed |= (elementBinding?.Invoke(i, offSet + 1, curValue)) ?? false;
				}

				GUILayout.Space(3);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space((offSet + 1) * STANDARD_OFFSET);
				int newSize = EditorGUILayout.DelayedIntField("Size", curValue.Length);
				EditorGUILayout.EndHorizontal();

				if (newSize != curValue.Length)
				{
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
		public static void AssetCreator<T>(params string[] pathParts) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			string name = "/New " + typeof(T).Name;
			string path = "";

			for(int i = 0; i < pathParts.Length; i++)
			{
				path += "/" + pathParts[i];
				if(!Directory.Exists(Application.dataPath + path))
				{
					Directory.CreateDirectory(Application.dataPath + path);
				}
			}
			AssetDatabase.CreateAsset(asset, "Assets" + path + name + ".asset");

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow();

			Selection.activeObject = null;
			Selection.activeObject = asset;
			EditorGUIUtility.PingObject(asset);

			Debug.Log("Created: '" + name.Substring(1) + "' at: Assets" + path + "/..");
		}
	}
}