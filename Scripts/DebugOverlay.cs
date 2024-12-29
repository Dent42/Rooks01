using Godot;
using System;
using System.Linq;

public partial class DebugOverlay : Control
{

	[Export]
	public Label coordinates;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		coordinates.Text = GameManager.GM.GetMouseCoordinates3D().ToString() + "\n" +
			GameManager.GRID.currentState.ToString() + "\n" +
			GameManager.GRID.dropX + " , " + GameManager.GRID.dropY;
	}
}
