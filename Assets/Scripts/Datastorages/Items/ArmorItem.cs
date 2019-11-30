using EoE.Entities;

namespace EoE.Information
{
	public class ArmorItem : Item
	{
		protected override bool OnUse(Entitie user)
		{
			return false;
		}
	}
}