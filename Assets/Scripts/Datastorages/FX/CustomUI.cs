﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EoE.Information
{
	public class CustomUI : FXObject
	{
		[Tooltip("The UI Paremt that will be created on the canvas.")]
		public GameObject UIPrefabObject = null;
		[Tooltip("The child index will decide on the rendering order, higher means later, 0 means every other UI object will be rendered infront of this object.")]
		public int CustomChildIndex = 9999;
	}
}