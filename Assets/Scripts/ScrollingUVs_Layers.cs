using UnityEngine;
using System.Collections;

namespace EoE.Other
{
	public class ScrollingUVs_Layers : MonoBehaviour
	{
		//public int materialIndex = 0;
		public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
		public string textureName = "_MainTex";

		Vector2 uvOffset = Vector2.zero;
		private Renderer materialRenderer;
		private void Start()
		{
			materialRenderer = GetComponent<Renderer>();
		}
		void LateUpdate()
		{
			uvOffset += (uvAnimationRate * Time.deltaTime);
			if (materialRenderer.enabled)
			{
				materialRenderer.sharedMaterial.SetTextureOffset(textureName, uvOffset);
			}
		}
		private void OnDestroy() => ResetMat();
		private void OnApplicationQuit() => ResetMat();
		private void ResetMat() => materialRenderer.sharedMaterial.SetTextureOffset(textureName, Vector2.zero);
	}
}