using Godot;
using System;
using System.Collections.Generic;

public partial class Score : Control
{

	[Export]
	public Label scoreLbl;
	
	public ulong total = 0; 
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.SCORE = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void scoreGem(Gem g)
	{
		int add = 0;
		
		int ax = g.adjacentX;
		int ay = g.adjacentY;

		if(ax >= 2)
		{
			add += (ax-1);
		}
		if(ay >= 2)
		{
			add += (ay-1);
		}

		total += (ulong)add;

		scoreLbl.Text = total.ToString("N0");
	}
}
