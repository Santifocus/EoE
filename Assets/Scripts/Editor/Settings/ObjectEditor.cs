using UnityEditor;
using UnityEngine;

namespace EoE.Information
{
	[CustomEditor(typeof(ObjectSettings), true), CanEditMultipleObjects]
	public class ObjectEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			CustomInspector();
			if (EoEEditor.isDirty)
			{
				EoEEditor.isDirty = false;
				EditorUtility.SetDirty(target);
			}
		}
		protected virtual void CustomInspector() { }
	}
}