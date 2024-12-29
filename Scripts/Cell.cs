using Godot;
using System;

public partial class Cell : Node3D
{
	
	// public override void _Input(InputEvent evt)
	// {
	// 	if (evt.IsActionPressed("click"))
	// 	{
	// 		GD.Print("   Clicked Cell: " + this.GlobalPosition.ToString());
	// 	}
	// 	// if (currentState == State.dragging && evt.IsActionReleased("click"))
	// 	// {
	// 	// 	currentState = State.snapBack;
			
	// 	// 	GD.Print("Released: " + this.Name);
	// 	// }
	// }

	[Export]
	public MeshInstance3D mesh;

	public void setValid(bool isValid)
	{
		if(isValid)
		{
			mesh.Transparency = 0.9f;
		}
		else
		{
			mesh.Transparency = 1f;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		setValid(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
