using EoE.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public class EnduranceBarController : MonoBehaviour
	{
		[SerializeField] private Image mainBarParent = default;
		[SerializeField] private GridLayoutGroup smallBarsGrid = default;
		[SerializeField] private Image smallBarPrefab = default;
		private Image mainBar;
		private List<Image> smallBars;

		private void Update()
		{
		}
	}
}