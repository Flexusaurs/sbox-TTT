using Sandbox;
using System;

namespace TTT;

/// <summary>
/// Allows a prop to be controlled by a spectator.
/// </summary>
public partial class PropPossession : EntityComponent<Prop>
{
	[Net, Local]
	public int Punches { get; private set; }

	[Net, Local]
	public int MaxPunches { get; private set; }
	private const float PunchRechargeTime = 1f;

	private Player _owner;
	private UI.PunchOMeter _meter;
	private UI.PossessionNameplate _nameplate;
	private TimeUntil _timeUntilNextPunch = 0;
	private TimeUntil _timeUntilRecharge = 0;

	public void Punch()
	{
		if ( Punches <= 0 )
			return;

		if ( !_timeUntilNextPunch )
			return;

		var physicsBody = Entity.PhysicsBody;

		var mass = Math.Min( 150f, physicsBody.Mass );
		var force = 110f * 75f;
		var mf = mass * force;

		_timeUntilNextPunch = 0.15f;

		if ( Input.Pressed( InputButton.Jump ) )
		{
			physicsBody.ApplyForceAt( physicsBody.MassCenter, new Vector3( 0, 0, mf ) );
			_timeUntilNextPunch = 0.2f;
		}
		else if ( _owner.InputDirection.x != 0f )
		{
			physicsBody.ApplyForceAt( physicsBody.MassCenter, _owner.InputDirection.x * (Vector3.Forward * _owner.ViewAngles.ToRotation()) * mf );
		}
		else if ( _owner.InputDirection.y != 0f )
		{
			physicsBody.ApplyForceAt( physicsBody.MassCenter, _owner.InputDirection.y * (Vector3.Left * _owner.ViewAngles.ToRotation()) * mf );
		}

		Punches = Math.Max( Punches - 1, 0 );
	}

	protected override void OnActivate()
	{
		base.OnActivate();

		_owner = Entity.Owner as Player;

		MaxPunches = (int)Math.Min( Math.Max( 0, _owner.ActiveKarma / 100 ), 13 );

		if ( Game.LocalPawn is Player localPlayer && !localPlayer.IsAlive )
			_nameplate = new( Entity );

		if ( _owner.IsLocalPawn )
		{
			_meter = new( this );
			CameraMode.Current = new FollowEntityCamera( Entity );
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		if ( !_owner.Prop.IsValid() )
			_owner.CancelPossession();

		_nameplate?.Delete( true );

		if ( _owner.IsLocalPawn )
		{
			_meter?.Delete( true );

			if ( !_owner.IsAlive )
				CameraMode.Current = new FreeCamera();
		}
	}

	// The player currently possessing this prop has spawned, we need to
	// cancel the current prop possession.
	// (Note "static" since we need to remove the current instance of this component)
	[GameEvent.Player.Spawned]
	private static void OnPlayerSpawned( Player player )
	{
		player.CancelPossession();
	}

	// Another player has spawned and needs the nameplate of this current
	// prop possession removed (since they are alive now).
	[GameEvent.Player.Spawned]
	private void DeleteNameplate( Player player )
	{
		if ( player.IsLocalPawn )
			_nameplate?.Delete( true );
	}

	[GameEvent.Player.Killed]
	private void CreateNameplate( Player player )
	{
		if ( player.IsLocalPawn )
			_nameplate = new( Entity );
	}

	[Event.Tick.Server]
	private void RechargePunches()
	{
		if ( !_timeUntilRecharge )
			return;

		Punches = Math.Min( Punches + 1, MaxPunches );
		_timeUntilRecharge = PunchRechargeTime;
	}
}
