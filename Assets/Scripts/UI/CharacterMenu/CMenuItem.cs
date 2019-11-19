using EoE.Controlls;
using EoE.Sounds;
using UnityEngine;
using UnityEngine.UI;

namespace EoE.UI
{
	public abstract class CMenuItem : MonoBehaviour
	{
		[SerializeField] protected Image pointer = default;
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

		protected void PlayFeedback(bool successSound)
		{
			SoundManager.SetSoundState(successSound ? ConstantCollector.MENU_NAV_SOUND : ConstantCollector.FAILED_MENU_NAV_SOUND, true);
		}
		protected virtual void OnPress() { }
		protected virtual void OnBack() { }
		protected virtual void Select() { }
		protected virtual void DeSelect() { }
	}
}