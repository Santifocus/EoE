using UnityEngine.SceneManagement;

namespace EoE.UI
{
	public class QuitGameButton : CMenuItem
	{
		protected override void OnPress()
		{
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadSceneAsync(ConstantCollector.MAIN_MENU_SCENE_INDEX);
		}
	}
}