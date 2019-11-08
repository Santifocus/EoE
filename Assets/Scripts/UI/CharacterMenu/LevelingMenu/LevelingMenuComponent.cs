using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public abstract class LevelingMenuComponent : MonoBehaviour
	{
		[SerializeField] protected Image pointer;
		public static LevelingMenuComponent SelectedComponent { get; private set; }
		protected bool selected { get; private set; }
		public void SelectComponent()
		{
			if (SelectedComponent != null)
				SelectedComponent.DeSelectComponent();

			selected = true;
			pointer.gameObject.SetActive(true);
			SelectedComponent = this;
			Select();
		}
		public void DeSelectComponent()
		{
			if (SelectedComponent == this)
				SelectedComponent = null;

			selected = false;
			pointer.gameObject.SetActive(false);
			DeSelect();
		}
		protected abstract void Select();
		protected abstract void DeSelect();
	}
}