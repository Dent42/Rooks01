using Godot;
using System;


public partial class Token : Area3D
{

	public string suit = "default";

	public enum State : int
	{
		idle,
		dragging,
		snapPreview,
		snapBack
	}

	public State currentState;

	//initial positions
	public int x0;
	public int y0;
	public Vector3 getInitialPosition()
	{
        return new Vector3(x0, y0, 0);
	}
	
	//preview position
	public int dx = 0;
	public int dy = 0;


	public void _on_input_event(Node camera, InputEvent evt, Vector3 event_position,
			Vector3 normal, int shape_idx)
	{
		if (evt.IsActionPressed("click"))
		{
			currentState = State.dragging;
			GameManager.GRID.setValidCells(x0, y0, true);
			GD.Print("Pressed: " + this.Name);
		}
	}




	public void _on_mouse_entered()
	{
		MeshInstance3D mi = this.GetNode<MeshInstance3D>(suit);
		mi.Transparency = 0.5f;
	}

	public void _on_mouse_exited()
	{
		MeshInstance3D mi = this.GetNode<MeshInstance3D>(suit);
		mi.Transparency = 0f;
	}

	public override void _Input(InputEvent evt)
	{
		if (currentState == State.dragging && evt.IsActionReleased("click"))
		{
			currentState = State.snapBack;
			GameManager.GRID.setValidCells(x0, y0, false);
			GD.Print("Released: " + this.Name);
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.GetNode<MeshInstance3D>(suit).Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (currentState == State.dragging)
		{
			Vector3 mouseVector = GameManager.GM.GetMouseCoordinates3D();
			this.GlobalPosition = this.GlobalPosition.Lerp(mouseVector, (float)(25 * delta));

			bool valid = GameManager.GRID.updateDropPosition(mouseVector, x0, y0);
			// int xInt = (int)Math.Round(mouseVector.X);
			// int yInt = (int)Math.Round(mouseVector.Y);
			// GameManager.GRID.dropX = xInt;
			// GameManager.GRID.dropY = yInt;
		}
		else if (currentState == State.snapBack)
		{
			// GD.Print("This: " + this.GlobalPosition.ToString());
			// GD.Print("Start: " + this.startPosition.ToString());

			if (this.GlobalPosition == this.getInitialPosition())
			{
				// GD.Print("equal");
				this.GlobalPosition = this.getInitialPosition();
				currentState = State.idle;
			}
			else
			{
				// GD.Print("not equal");
				this.Position = this.Position.MoveToward(this.getInitialPosition(), (float)(20 * delta));
			}
		}
	}

}