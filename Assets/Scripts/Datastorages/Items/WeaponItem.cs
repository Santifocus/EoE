using EoE.Entities;

namespace EoE.Information
{
	public class WeaponItem : Item
	{
		protected override bool OnUse(Entitie user)
		{
			return false;
		}
	}
}