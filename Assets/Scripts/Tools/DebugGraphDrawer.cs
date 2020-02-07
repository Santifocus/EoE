using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VisualDebug
{
	public class DebugGraphDrawer : VisualDebugger
	{
		private const float SCOPE_PADDING = 0.1f;
		private const float MIN_SCOPE_PADDING = 2f;
		private const float LINE_HEIGHT = 4;

		private const float ZERO_LINE_HEIGHT = 0.15f;
		private const float ZERO_LINE_MIN_HEIGHT = 5;
		private const float ZERO_LINE_MAX_HEIGHT = 12;

		private static Dictionary<string, DebugGraphDrawer> NamedGraphDrawers = new Dictionary<string, DebugGraphDrawer>();

		//Colors
		private static readonly Color BackgroundColor = new Color(0.75f, 0.75f, 0.75f, 0.5f);
		private static readonly Color YAxisColor = new Color(0.2f, 0.8f, 0.15f, 0.75f);
		private static readonly Color ZeroAxisColor = new Color(0.8f, 0.2f, 0.15f, 0.65f);

		//Data
		private int maxTrackedValues;
		public int MaxTrackedValues
		{
			get => maxTrackedValues;
			set
			{
				value = value < 0 ? 0 : value;
				int dif = maxTrackedValues - value;
				maxTrackedValues = value;

				for (int i = (dif - 1); i >= 0; i--)
				{
					for(int j = 0; j < graphs.Length; j++)
					{
						if (graphs[j].TrackedValues.Count > i)
							graphs[j].TrackedValues.RemoveAt(i);
					}
				}
			}
		}

		//Graph Components
		protected override Vector2 Dimensions => GraphSize;
		protected override RectTransform SelfRect => background;
		public Vector2 GraphSize = new Vector2(CanvasSize.x * 0.2f, CanvasSize.y * 0.25f);
		private RectTransform background;
		private RectTransform zeroAxis;
		private RectTransform yAxis;

		private GraphData[] graphs;

		//Scope values
		private float lowestValue;
		private float highestValue;

		private float lowerScope;
		private float upperScope;
		private float scopeDifference;

		public static DebugGraphDrawer TrackValue(string graphName, float value)
		{
			DebugGraphDrawer targetDrawer;
			if(NamedGraphDrawers.TryGetValue(graphName, out targetDrawer))
			{
				targetDrawer.TrackValue(value);
			}
			else
			{
				targetDrawer = new DebugGraphDrawer(50, new GraphBaseData(Color.black, value));
				NamedGraphDrawers.Add(graphName, targetDrawer);
			}

			return targetDrawer;
		}
		public DebugGraphDrawer(int maxTrackedValues, params GraphBaseData[] graphData)
		{
			Setup(maxTrackedValues, graphData);
		}
		public DebugGraphDrawer(int maxTrackedValues, Vector2 GraphSize, params GraphBaseData[] graphData)
		{
			this.GraphSize = GraphSize;
			Setup(maxTrackedValues, graphData);
		}
		private void Setup(int maxTrackedValues, params GraphBaseData[] graphData)
		{
			EnsureCanvas();

			//Insert seeded values
			graphs = new GraphData[graphData.Length];
			for (int i = 0; i < graphs.Length; i++)
			{
				graphs[i] = new GraphData()
				{
					LineColor = graphData[i].GraphColor,
					PooledLines = new List<RectTransform>(),
					TrackedValues = new List<float>(graphData[i].SeedValues)
				};
			}
			MaxTrackedValues = maxTrackedValues;

			BuildGraph();
			OnCreate();
		}
		protected override void OnDestroy()
		{
			if(background)
				Object.Destroy(background.gameObject);
		}
		public void TrackValue(float newValue, int targetGraph = 0)
		{
			if (!background || MaxTrackedValues <= 0 || targetGraph < 0 || targetGraph >= graphs.Length)
				return;

			if (graphs[targetGraph].TrackedValues.Count >= MaxTrackedValues)
			{
				graphs[targetGraph].TrackedValues.RemoveAt(0);
			}
			graphs[targetGraph].TrackedValues.Add(newValue);

			CalculateScope();
			UpdateGraph();
		}
		private void BuildGraph()
		{
			BuildBackground();
			DrawYAxis();
			DrawZeroAxis();

			CalculateScope();
			UpdateGraph();
		}
		private void BuildBackground()
		{
			GameObject backgroundObject = new GameObject("DebugGraphDrawerBackground");
			backgroundObject.transform.SetParent(DebugCanvas.transform);

			Image backgroundImage = backgroundObject.AddComponent<Image>();
			backgroundImage.color = BackgroundColor;

			background = backgroundObject.GetComponent<RectTransform>();
			background.anchorMin = background.anchorMax = background.pivot = new Vector2(0, 1);
			background.sizeDelta = GraphSize;
		}
		private void DrawYAxis()
		{
			Image yAxisImage = GetLineObject(YAxisColor);
			yAxis = yAxisImage.rectTransform;

			yAxis.sizeDelta = new Vector2(LINE_HEIGHT, GraphSize.y);
			yAxis.anchoredPosition = new Vector2(0, GraphSize.y / 2);
			yAxis.gameObject.SetActive(true);
		}
		private void DrawZeroAxis()
		{
			Image zeroAxisImage = GetLineObject(ZeroAxisColor);
			zeroAxis = zeroAxisImage.rectTransform;
		}
		private Image GetLineObject(Color lineColor)
		{
			GameObject lineObject = new GameObject("Line");
			lineObject.transform.SetParent(background.transform);
			lineObject.transform.localScale = Vector3.one;

			Image lineVisual = lineObject.AddComponent<Image>();
			lineVisual.color = lineColor;
			lineVisual.raycastTarget = false;

			lineVisual.rectTransform.anchorMin = lineVisual.rectTransform.anchorMax = Vector2.zero; //Left Bottom
			lineVisual.rectTransform.pivot = Vector2.one / 2;
			lineObject.SetActive(false);

			return lineVisual;
		}
		private Vector2 GetValueCoordinates(float value, int index)
		{
			float xCoordinate = (index / ((float)maxTrackedValues - 1)) * GraphSize.x;

			float offSetFromLower = Mathf.Abs(value - lowerScope);
			float yCoordinate = (offSetFromLower / scopeDifference) * GraphSize.y;
			return new Vector2(xCoordinate, yCoordinate);
		}
		private void DrawLine(RectTransform targetTransform, Vector2 pointOne, Vector2 pointTwo)
		{
			Vector2 difference = pointOne - pointTwo;
			float distance = difference.magnitude;
			Vector2 direction = distance > 0 ? difference / distance : Vector2.up;

			targetTransform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
			targetTransform.anchoredPosition = (pointOne + pointTwo) / 2;
			targetTransform.sizeDelta = new Vector2(distance, LINE_HEIGHT);

			targetTransform.gameObject.SetActive(true);
		}
		private void UpdateGraph()
		{
			UpdateZeroAxis();
			UpdateValueLines();
		}
		private void UpdateZeroAxis()
		{
			bool shouldBeActive = lowerScope < 0 && upperScope >= 0;
			zeroAxis.gameObject.SetActive(shouldBeActive);
			if (shouldBeActive)
			{
				float lineHeight = Mathf.Clamp(ZERO_LINE_HEIGHT * (1 / scopeDifference) * GraphSize.y, ZERO_LINE_MIN_HEIGHT, ZERO_LINE_MAX_HEIGHT);

				zeroAxis.sizeDelta = new Vector2(GraphSize.x, lineHeight);
				zeroAxis.anchoredPosition = new Vector2(GraphSize.x / 2, (-lowerScope / scopeDifference) * GraphSize.y);
			}
		}
		private void UpdateValueLines()
		{
			for (int i = 0; i < graphs.Length; i++)
			{
				if (graphs[i].TrackedValues.Count < 2)
					continue;

				int biggerListSize = System.Math.Max(graphs[i].TrackedValues.Count, graphs[i].PooledLines.Count);
				Vector2 lastPointPos = GetValueCoordinates(graphs[i].TrackedValues[0], 0);

				for (int j = 1; j < biggerListSize; j++)
				{
					if (j < graphs[i].TrackedValues.Count)
					{
						Vector2 curPointPos = GetValueCoordinates(graphs[i].TrackedValues[j], j);

						while (j >= graphs[i].PooledLines.Count)
							graphs[i].PooledLines.Add(GetLineObject(graphs[i].LineColor).rectTransform);

						DrawLine(graphs[i].PooledLines[j], lastPointPos, curPointPos);
						lastPointPos = curPointPos;
					}
					else
					{
						for (int k = j; k < graphs[i].PooledLines.Count; k++)
						{
							if (graphs[i].PooledLines[k].gameObject.activeSelf)
								graphs[i].PooledLines[k].gameObject.SetActive(false);
							else
								break;
						}
						break;
					}
				}
			}
		}
		private void CalculateScope()
		{
			bool noValues = true;
			lowestValue = float.MaxValue;
			highestValue = float.MinValue;

			for (int i = 0; i < graphs.Length; i++)
			{
				noValues = noValues && (graphs[i].TrackedValues.Count == 0);
				for (int j = 0; j < graphs[i].TrackedValues.Count; j++)
				{
					if (lowestValue > graphs[i].TrackedValues[j])
						lowestValue = graphs[i].TrackedValues[j];

					if (highestValue < graphs[i].TrackedValues[j])
						highestValue = graphs[i].TrackedValues[j];
				}
			}

			if(noValues)
			{
				lowestValue = highestValue = 0;
			}

			float dif = highestValue - lowestValue;
			lowerScope = Mathf.Min(lowestValue - (dif * SCOPE_PADDING / 2), lowestValue - MIN_SCOPE_PADDING);
			upperScope = Mathf.Max(highestValue + (dif * SCOPE_PADDING / 2), highestValue + MIN_SCOPE_PADDING);
			scopeDifference = Mathf.Abs(upperScope - lowerScope);
		}
		public struct GraphBaseData
		{
			public Color GraphColor;
			public float[] SeedValues;
			public GraphBaseData(Color GraphColor, params float[] SeedValues)
			{
				this.GraphColor = GraphColor;
				this.SeedValues = SeedValues;
			}
		}
		private class GraphData
		{
			public List<float> TrackedValues;
			public List<RectTransform> PooledLines = new List<RectTransform>();
			public Color LineColor;
		}
	}
}