using Godot;
using System;

public partial class ScoreLabel : Label
{
	private int _score = 0;
	// Called when the node enters the scene tree for the first time.
	public void OnMobSquashed()
	{
		_score += 1;
		Text = $"Score: {_score}";
	}
}
