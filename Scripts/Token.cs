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
		idlePreview,
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

	//adjacent same suit
	public int adjacentX = 0;
	public int adjacentY = 0;

	//click event
	public void _on_input_event(Node camera, InputEvent evt, Vector3 event_position,
			Vector3 normal, int shape_idx)
	{
		if (evt.IsActionPressed("click"))
		{
			currentState = State.dragging;
			GameManager.GRID.setValidCells(x0, y0, true);
		}
	}

	//release click event
	public override void _Input(InputEvent evt)
	{
		if (currentState == State.dragging && evt.IsActionReleased("click"))
		{
			//TRY TO submit?


			//snap back
			currentState = State.snapBack;
			GameManager.GRID.setValidCells(x0, y0, false);
			GameManager.GRID.doSnapBack();
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
		
		//DEBUG
		GetNode<Label3D>("tokenLbl").Text = "(" + adjacentX + "," + adjacentY + ")";

		if (currentState == State.idle || currentState == State.idlePreview)
		{
			return;
		}

		else if (currentState == State.dragging)
		{
			Vector3 mouseVector = GameManager.GM.GetMouseCoordinates3D();
			this.Position = this.Position.Lerp(mouseVector, (float)(25 * delta));
			GameManager.GRID.processDrag(mouseVector, x0, y0);
		}

		else if (currentState == State.snapPreview)
		{
			Vector3 snapTo = new Vector3(x0 + dx, y0 + dy, 0);

			if (this.Position != snapTo)
			{
				this.Position = this.Position.MoveToward(snapTo, (float)(25 * delta));
			}
			else
			{
				currentState = State.idlePreview;
			}
		}

		else if (currentState == State.snapBack)
		{
			if (this.Position != this.getInitialPosition())
			{
				this.Position = this.Position.MoveToward(this.getInitialPosition(), (float)(25 * delta));
			}
			else
			{
				currentState = State.idle;
			}
		}
	}


	public void doPreview(int dx, int dy)
	{
		this.dx = dx;
		this.dy = dy;
		this.currentState = State.snapPreview;
	}

	public void doSnapBack()
	{
		this.dx = 0;
		this.dy = 0;
		this.adjacentX = 0;
		this.adjacentY = 0;

		if(currentState == State.idlePreview || currentState == State.snapPreview)
		{
			this.currentState = State.snapBack;
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
}