using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour
{
	//TODO: Remove Script, meant as placeholder
	[SerializeField] private List<MeshRenderer> planeMeshes		= default;
	[SerializeField] private int textureSize					= default;
	[SerializeField] private int octaves						= 3;
	[SerializeField] private float noiseZoom					= 50;
	[SerializeField] private Gradient coloring					= default;
	private Texture2D generatedTexture;

	private void OnEnable()
	{
		for(int i = 0; i < planeMeshes.Count; i++)
			GenerateNoise(planeMeshes[i]);
	}

	private void GenerateNoise(MeshRenderer planeMesh)
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
				generatedTexture.SetPixel(x, y, coloring.Evaluate(point));
			}
		}
		generatedTexture.Apply();
		planeMesh.material.SetTexture("_MainTex", generatedTexture);
	}
}
