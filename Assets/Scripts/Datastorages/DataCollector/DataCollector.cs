using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EoE.Information
{
	public abstract class DataCollector<T> : ScriptableObject where T : ScriptableObject
	{
		public T[] Data;
#if UNITY_EDITOR
		public void CollectData()
		{
			string[] foundAssetsGUIDs = AssetDatabase.FindAssets("t:" + typeof(T).Name.ToLower());
			List<T> allFoundData = new List<T>(foundAssetsGUIDs.Length);
			for(int i = 0; i < foundAssetsGUIDs.Length; i++)
			{
				allFoundData.Add((T)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(foundAssetsGUIDs[i]), typeof(T)));
			}
			SortData(allFoundData);
			Data = allFoundData.ToArray();
		}
#endif
		protected virtual List<T> SortData(List<T> input) => input;
	}
}