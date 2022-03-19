﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class RolePlateComponent : EntityComponent<Player>
{
	RolePlate _rolePlate;

	protected override void OnActivate()
	{
		_rolePlate = new RolePlate();
	}

	protected override void OnDeactivate()
	{
		_rolePlate?.Delete();
		_rolePlate = null;
	}

	/// <summary>
	/// Called for every plate, while it's active
	/// </summary>
	[Event.Frame]
	private void FrameUpdate()
	{
		var tx = Entity.GetAttachment( "hat" ) ?? Entity.Transform;
		tx.Position += Vector3.Up * 5.0f;
		tx.Rotation = Rotation.LookAt( -CurrentView.Rotation.Forward );

		_rolePlate.Transform = tx;
	}

	[TTTEvent.Player.Role.Changed]
	private static void OnRoleChanged( Player player, BaseRole oldRole )
	{
		if ( !Host.IsClient || !player.IsAlive() || player.IsLocalPawn )
			return;

		if ( oldRole is TraitorRole )
		{
			player.Components.RemoveAny<RolePlateComponent>();
			return;
		}

		var localPlayer = Local.Pawn as Player;
		if ( localPlayer.Role is TraitorRole && player.Role is TraitorRole )
			player.Components.GetOrCreate<RolePlateComponent>();
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsClient || player.IsLocalPawn || player.Team != Team.Traitors )
			return;

		player.Components.RemoveAny<RolePlateComponent>();
	}
}

public partial class RolePlate : WorldPanel
{
	public RolePlate()
	{
		StyleSheet.Load( "/ui/player/roleplate/RolePlate.scss" );

		Add.Image( "ui/traitor-icon.png", "icon" );
	}
}
