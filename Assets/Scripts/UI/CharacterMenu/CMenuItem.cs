using EoE.Controlls;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public abstract class CMenuItem : MonoBehaviour
	{
		[SerializeField] protected Image pointer;
		public static CMenuItem SelectedComponent { get; private set; }
		protected bool selected { get; private set; }
		public void SelectMenuItem()
		{
			if (SelectedComponent != null)
				SelectedComponent.DeSelectMenuItem();

			selected = true;
			if (pointer)
				pointer.gameObject.SetActive(true);
			SelectedComponent = this;
			Select();
		}
		public void DeSelectMenuItem()
		{
			if (SelectedComponent == this)
				SelectedComponent = null;

			selected = false;
			if (pointer)
				pointer.gameObject.SetActive(false);
			DeSelect();
		}
		protected virtual void Update()
		{
			if (!selected)
				return;

			if (InputController.MenuEnter.Down)
				OnPress();
			if (InputController.MenuBack.Down)
				OnBack();
		}
		protected virtual void OnPress() { }
		protected virtual void OnBack() { }
		protected virtual void Select() { }
		protected virtual void DeSelect() { }
	}
}