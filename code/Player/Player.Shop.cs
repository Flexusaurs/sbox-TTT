using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	[Net, Local]
	public IList<string> PurchasedLimitedShopItems { get; set; }

	public bool CanPurchase( ItemInfo item )
	{
		if ( !item.Buyable )
			return false;

		if ( Credits < item.Price )
			return false;

		if ( item is CarriableInfo carriable && !Inventory.HasFreeSlot( carriable.Slot ) )
			return false;

		if ( !Role.AvailableItems.Contains( item.LibraryName ) )
			return false;

		if ( item.IsLimited && PurchasedLimitedShopItems.Contains( item.LibraryName ) )
			return false;

		return true;
	}

	[ConCmd.Server]
	public static void PurchaseItem( int itemId )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.FromId<ItemInfo>( itemId );
		if ( itemInfo is null )
			return;

		if ( !player.CanPurchase( itemInfo ) )
			return;

		if ( itemInfo.IsLimited )
			player.PurchasedLimitedShopItems.Add( itemInfo.LibraryName );

		player.Credits -= itemInfo.Price;
		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( TypeLibrary.Create<Carriable>( itemInfo.LibraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( TypeLibrary.Create<Perk>( itemInfo.LibraryName ) );
	}
}
