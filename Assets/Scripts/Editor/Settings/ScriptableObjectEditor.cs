using UnityEditor;

namespace EoE.Information
{
	[CustomEditor(typeof(ObjectSettings), true), CanEditMultipleObjects]
	public class ScriptableObjectEditor : Editor
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