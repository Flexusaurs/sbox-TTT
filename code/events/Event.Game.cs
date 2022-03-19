using Sandbox;

namespace TTT;

public static partial class TTTEvent
{
	public static class Game
	{
		public const string RoundChanged = "ttt.game.roundchanged";

		/// <summary>
		/// Called everytime the round changes.
		/// <para>Event is passed the <strong><see cref="TTT.BaseRound"/></strong> instance of the old round.</para>
		/// <para>Event is passed the <strong><see cref="TTT.BaseRound"/></strong> instance of the new round.</para>
		/// </summary>
		public class RoundChangedAttribute : EventAttribute
		{
			public RoundChangedAttribute() : base( RoundChanged ) { }
		}
	}
}
