namespace EoE
{
	public static class ConstantCollector
	{
		public const int TERRAIN_LAYER = 8;
		public const int ENTITIE_LAYER = 9;
		public const int SHIELD_LAYER = 10;

		public const int TERRAIN_COLLIDING_LAYER = 15;
		public const int ENTITIE_COLLIDING_LAYER = 16;

		public const int TERRAIN_LAYER_MASK = 1 << TERRAIN_LAYER;
		public const int ENTITIE_LAYER_MASK = 1 << ENTITIE_LAYER;
		public const int SHIELD_LAYER_MASK = 1 << SHIELD_LAYER;

		public const int MAIN_MENU_SCENE_INDEX = 0;
		public const int GAME_SCENE_INDEX = 1;
		public const int TUTORIAL_SCENE_INDEX = 2;
		public const int LOAD_SCENE_INDEX = 3;

		public const string MENU_NAV_SOUND = "MenuNavigation";
		public const string FAILED_MENU_NAV_SOUND = "FailedMenuNavigation";
		public const string MENU_SCROLL_SOUND = "MenuScroll";
	}
}