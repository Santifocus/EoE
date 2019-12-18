using UnityEngine;

namespace EoE
{
	[ExecuteInEditMode]
	public class TextureBlitter : MonoBehaviour
	{
		[SerializeField] private Material screenEffectMat = default;

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, screenEffectMat);
		}
	}
}