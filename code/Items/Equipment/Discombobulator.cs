﻿using Sandbox;
using System;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_grenade_discombobulator", Title = "Discombobulator" )]
public class Discombobulator : Grenade
{
	protected override void OnExplode()
	{
		base.OnExplode();

		PlaySound( RawStrings.DiscombobulatorExplodeSound );

		var overlaps = Entity.FindInSphere( Position, 800 );

		foreach ( var overlap in overlaps )
		{
			if ( overlap is not ModelEntity entity || !entity.IsValid() )
				continue;

			if ( !entity.IsAlive() )
				continue;

			if ( !entity.PhysicsBody.IsValid() )
				continue;

			if ( entity.IsWorld )
				continue;

			var targetPos = entity.PhysicsBody.MassCenter;

			var dist = Vector3.DistanceBetween( Position, targetPos );
			if ( dist > 400 )
				continue;

			var trace = Trace.Ray( Position, targetPos )
				.Ignore( this )
				.WorldOnly()
				.Run();

			if ( trace.Fraction < 0.98f )
				continue;

			float distanceMul = 1.0f - Math.Clamp( dist / 400, 0.0f, 1.0f );
			float force = 800 * distanceMul;
			var forceDir = (targetPos - Position).Normal;

			if ( entity is not Player )
				entity.ApplyAbsoluteImpulse( force * forceDir );
			else
				entity.ApplyAbsoluteImpulse( force * forceDir * 2 );
		}
	}
}