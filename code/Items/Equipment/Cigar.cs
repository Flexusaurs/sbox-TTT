using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_cigar", Title = "Cigar" )]
public partial class Cigar : Carriable
{
	private TimeSince _timeSinceLastSmoke = 5;
	private Particles _trailParticle;

	public override void Simulate( Client client )
	{
		if ( Input.Pressed( InputButton.Attack1 ) && _timeSinceLastSmoke > 5 )
			Smoke();
	}

	private void Smoke()
	{
		_timeSinceLastSmoke = 0;


		Particles.Create( "particles/swb/smoke/swb_smokepuff_1", this, "muzzle" );
		_trailParticle = null;
		_trailParticle ??= Particles.Create( "particles/swb/muzzle/barrel_smoke", this, "muzzle" );

		Owner.TakeDamage( DamageInfo.Generic( 1 )
			.WithAttacker( Owner )
			.WithWeapon( this )
		);
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		_trailParticle?.Destroy( true );
	}
}