using UnityEngine.SceneManagement;

namespace EoE.UI
{
	public class LoadGameButton : CMenuItem
	{
		protected override void OnPress()
		{
			EffectUtils.ResetScreenEffects();
			SceneManager.LoadScene(ConstantCollector.GAME_SCENE_INDEX);
		}
	}
}