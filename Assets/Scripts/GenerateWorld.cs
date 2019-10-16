using EoE.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour
{
	[SerializeField] private MeshRenderer planeMesh = default;
	[SerializeField] private int textureSize = default;
	[SerializeField] private int octaves = 3;
	[SerializeField] private float noiseZoom = 50;
	private Texture2D generatedTexture;

	private void OnEnable()
	{
		GenerateNoise();
	}

	private void GenerateNoise()
	{
		generatedTexture = new Texture2D(textureSize, textureSize);
		float xOff = (Random.value - 0.5f) * 200;
		float yOff = (Random.value - 0.5f) * 200;

		float totalMultiplier = 1;
		float[] octaveMutliplier = new float[octaves];
		for (int i = 0; i < octaves; i++)
		{
			float part = 1 / Mathf.Pow(2, i + 1);
			octaveMutliplier[i] = part;
			totalMultiplier += part;
		}
		totalMultiplier = 1 / totalMultiplier;

		for (int x = 0; x < textureSize; x++)
		{
			for (int y = 0; y < textureSize; y++)
			{
				float point = Mathf.PerlinNoise(x / noiseZoom + xOff, y / noiseZoom + yOff);

				for (int i = 0; i < octaves; i++)
				{
					point += Mathf.PerlinNoise(x / (noiseZoom * octaveMutliplier[i]) + xOff, y / (noiseZoom * octaveMutliplier[i]) + yOff) * octaveMutliplier[i];
				}
				point *= totalMultiplier;
				generatedTexture.SetPixel(x, y, new Color(point, point, point, 1));
			}
		}
		generatedTexture.Apply();
		planeMesh.material.SetTexture("_MainTex", generatedTexture);
	}
}
