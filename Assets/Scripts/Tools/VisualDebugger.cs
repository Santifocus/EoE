using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VisualDebug
{
	public abstract class VisualDebugger
	{
		//Static
		private static List<VisualDebugger> CreatedVisualDebugger = new List<VisualDebugger>();
		protected static Canvas DebugCanvas;
		protected static readonly Vector2 CanvasSize = new Vector2(1920, 1080); //Normal 16:9
		protected static readonly Vector2 DebuggerSpacing = new Vector2(0.05f * CanvasSize.x, 0.05f * CanvasSize.y);
		protected static void EnsureCanvas()
		{
			if (!DebugCanvas)
				CreateDebugCanvas();
		}
		private static void CreateDebugCanvas()
		{
			GameObject canvasObject = new GameObject("Debug Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
			canvasObject.layer = 5; //UI
			canvasObject.transform.SetAsFirstSibling();

			DebugCanvas = canvasObject.GetComponent<Canvas>();
			DebugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
			DebugCanvas.sortingOrder = 100; //Infront of everything

			CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
			scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			scaler.referenceResolution = CanvasSize;
		}

		//Inhereting data
		protected abstract Vector2 Dimensions { get; }
		protected abstract RectTransform SelfRect { get; }
		protected void OnCreate()
		{
			CreatedVisualDebugger.Add(this);
			SetAnchoredPosition();
		}
		private void SetAnchoredPosition()
		{
			//First find out which column we are in
			int selfColumn = 0;
			float curColumnRequiredHeight = DebuggerSpacing.y;
			for(int i = 0; i < CreatedVisualDebugger.Count; i++)
			{
				curColumnRequiredHeight += CreatedVisualDebugger[i].Dimensions.y;
				if(curColumnRequiredHeight >= CanvasSize.y)
				{
					curColumnRequiredHeight = CreatedVisualDebugger[i].Dimensions.y + DebuggerSpacing.y * 2;
					selfColumn += 1;
				}
				else
				{
					curColumnRequiredHeight += DebuggerSpacing.y;
				}

				if (CreatedVisualDebugger[i] == this)
					break;
			}
			float yPos = curColumnRequiredHeight - (Dimensions.y + DebuggerSpacing.y);

			float totalWidht = DebuggerSpacing.x;
			if (selfColumn > 0)
			{
				float[] columnWidhts = new float[selfColumn];
				int curColumn = 0;
				curColumnRequiredHeight = DebuggerSpacing.y;
				for (int i = 0; i < CreatedVisualDebugger.Count; i++)
				{
					if (curColumn == selfColumn)
						break;

					curColumnRequiredHeight += CreatedVisualDebugger[i].Dimensions.y;
					if (curColumnRequiredHeight >= CanvasSize.y)
					{
						curColumnRequiredHeight = CreatedVisualDebugger[i].Dimensions.y;
						curColumn += 1;
					}
					else
					{
						curColumnRequiredHeight += DebuggerSpacing.y;
						if (CreatedVisualDebugger[i].Dimensions.x > columnWidhts[curColumn])
							columnWidhts[curColumn] = CreatedVisualDebugger[i].Dimensions.x;
					}
				}
				for (int i = 0; i < columnWidhts.Length; i++)
				{
					totalWidht += columnWidhts[i] + DebuggerSpacing.x;
				}
			}
			
			SelfRect.anchoredPosition = new Vector2(totalWidht, -yPos);
		}

		public void Destroy()
		{
			CreatedVisualDebugger.Remove(this);
			OnDestroy();
			for (int i = 0; i < CreatedVisualDebugger.Count; i++)
				CreatedVisualDebugger[i].SetAnchoredPosition();
		}
		protected virtual void OnDestroy() { }
	}
}