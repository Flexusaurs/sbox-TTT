using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

public partial class ScoreboardEntry : Panel
{
	public Player Player { get; set; }

	private static readonly ColorGroup[] _tagGroups = new ColorGroup[]
	{
		new ColorGroup("Friend", Color.FromBytes(0, 255, 0)),
		new ColorGroup("Suspect", Color.FromBytes(179, 179, 20)),
		new ColorGroup("Missing", Color.FromBytes(130, 190, 130)),
		new ColorGroup("Kill", Color.FromBytes(255, 0, 0))
	};

	private bool _isExpanded = false;

	protected override int BuildHash()
	{
		return HashCode.Combine(
			Player.Role,
			Player.SteamId,
			Player.BaseKarma,
			Player.Score,
			Player.Client.Ping,
			Player.TagGroup,
			Player.IsLocalPawn,
			_isExpanded
		);
	}

	private void SetTag( ColorGroup tagGroup )
	{
		if ( tagGroup.Title == Player.TagGroup.Title )
		{
			ResetTag();
			return;
		}

		Player.TagGroup = tagGroup;
	}

	private void ResetTag()
	{
		if ( Player.IsValid() )
			Player.TagGroup = default;
	}

	[Event.Entity.PostCleanup]
	private void OnRoundStart()
	{
		ResetTag();
	}
}