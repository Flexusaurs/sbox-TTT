using Sandbox;
using System;

namespace TTT;

[ClassName( "ttt_equipment_binoculars" )]
[Title( "Binoculars" )]
public partial class Binoculars : Carriable
{
	[Net, Predicted]
	private int ZoomLevel { get; set; }

	public bool IsZoomed => ZoomLevel > 0;
	private Corpse _corpse;
	private float _defaultFOV;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		_defaultFOV = Owner.CameraMode.FieldOfView;
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		_corpse = null;
		ZoomLevel = 0;
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
			ChangeZoomLevel();

		if ( Input.Pressed( InputButton.Reload ) )
		{
			// Reset zoom.
			ZoomLevel = 4;
			ChangeZoomLevel();
		}

		if ( !IsZoomed )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.MaxHintDistance )
			.Ignore( this )
			.Ignore( Owner )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		_corpse = trace.Entity as Corpse;

		if ( !IsServer || !_corpse.IsValid() )
			return;

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			_corpse.Search( Owner, Input.Down( InputButton.Run ), false );
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		if ( IsZoomed )
			input.ViewAngles = Angles.Lerp( input.OriginalViewAngles, input.ViewAngles, 0.5f / MathF.Pow( 2.5f, ZoomLevel ) );
	}

	public override void DestroyHudElements()
	{
		base.DestroyHudElements();

		PreviousOwner.CameraMode.FieldOfView = _defaultFOV;
	}

	private void ChangeZoomLevel()
	{
		if ( ZoomLevel >= 4 )
		{
			_corpse = null;
			ZoomLevel = 0;
			Owner.CameraMode.FieldOfView = _defaultFOV;

			return;
		}

		PlaySound( Strings.ScopeInSound );
		ZoomLevel++;
		Owner.CameraMode.FieldOfView = 40f / MathF.Pow( 2.5f, ZoomLevel );
	}
}
