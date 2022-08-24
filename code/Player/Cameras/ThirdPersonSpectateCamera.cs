using Sandbox;

namespace TTT;

public class ThirdPersonSpectateCamera : CameraMode, ISpectateCamera
{
	private Player _owner;
	private Vector3 _targetPos;
	private Angles _lookAngles;

	protected override void OnActivate()
	{
		_owner = Entity as Player;
		_owner.UpdateSpectatedPlayer();
	}

	public override void Deactivated()
	{
		_owner.CurrentPlayer = null;
	}

	public override void Update()
	{
		if ( !_owner.CurrentPlayer.IsValid() )
		{
			_owner.Camera = new FreeSpectateCamera();
			return;
		}

		_targetPos = Vector3.Lerp( _targetPos, _owner.CurrentPlayer.EyePosition, 50f * RealTime.Delta );

		var trace = Trace.Ray( _targetPos, _targetPos + Rotation.Forward * -120 )
			.WorldOnly()
			.Run();

		Rotation = Rotation.From( _lookAngles );
		Position = trace.EndPosition;
	}

	public override void BuildInput( InputBuilder input )
	{
		_lookAngles += input.AnalogLook;
		_lookAngles.roll = 0;

		if ( input.Pressed( InputButton.PrimaryAttack ) )
			_owner.UpdateSpectatedPlayer( 1 );
		else if ( input.Pressed( InputButton.SecondaryAttack ) )
			_owner.UpdateSpectatedPlayer( -1 );

		base.BuildInput( input );
	}
}
