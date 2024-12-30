using Godot;
using System;


public partial class Gem : Area3D
{

	public string suit = "default";
	public readonly float speed = 25f;

	public static readonly string PREVIEW = "preview";
	public static readonly string CANCEL = "cancel";
	// public enum State : int
	// {
	// 	idle,
	// 	dragging,
	// 	snapPreview,
	// 	idlePreview,
	// 	snapBack
	// }
	
	// public State OLDState;



	//state machine
	public StateGem currentState;

	public StateGem idle = new StateGemIdle();
	public StateGem drag = new StateGemDrag();
	public StateGem snapBack = new StateGemSnapBack();
	public StateGem snapPreview = new StateGemSnapPreview();
	public StateGem idlePreview = new StateGemIdlePreview();

	public void changeState(StateGem nextState)
	{
		// GD.Print("Change State: " + this.Name + " " + currentState.ToString() +
		// 	 " --> + " + nextState.ToString());
		currentState.ExitState(this);
		currentState = nextState;
		currentState.EnterState(this);
	}
	

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


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<MeshInstance3D>(suit).Visible = true;
		currentState = idle;
		currentState.EnterState(this);
	}

	




	//press click event
	public void _on_input_event(Node camera, InputEvent evt, Vector3 event_position,
			Vector3 normal, int shape_idx)
	{
		
		//requires pre-check for some reason
		if (currentState == idle && evt.IsActionPressed("click"))
		{
			currentState.HandleInput(this, evt);
		}

		// if (evt.IsActionPressed("click"))
		// {
		// 	OLDState = State.dragging;
		// 	GameManager.GRID.setValidCells(x0, y0, true);
		// }
	}

	//release click event
	public override void _Input(InputEvent evt)
	{
		
		//requires pre-check for some reason
		if(currentState == drag && evt.IsActionReleased("click"))
		{
			currentState.HandleInput(this, evt);
		}
		
		// currentState.HandleInput(this, evt);
		
		//GD.Print("Input: " + this.Name);
		
		// if (OLDState == State.dragging && evt.IsActionReleased("click"))
		// {
		// 	Vector3 mouseVector = GameManager.GM.GetMouseCoordinates3D();
		// 	bool validDragSpot = GameManager.GRID.processDrag(mouseVector, x0, y0);

		// 	if (validDragSpot)
		// 	{
		// 		//try submit

		// 	}
		// 	else
		// 	{
		// 		//snap back
		// 		OLDState = State.snapBack;
		// 		GameManager.GRID.setValidCells(x0, y0, false);
		// 		GameManager.GRID.doSnapBack();
		// 	}
		// }
	}




	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		//DEBUG
		GetNode<Label3D>("gemLbl").Text = "(" + adjacentX + "," + adjacentY + ")";

		currentState.Update(this, delta);

		// if (OLDState == State.idle || OLDState == State.idlePreview)
		// {
		// 	return;
		// }

		// else if (OLDState == State.dragging)
		// {
		// 	Vector3 mouseVector = GameManager.GM.GetMouseCoordinates3D();
		// 	this.Position = this.Position.Lerp(mouseVector, (float)(25 * delta));
		// 	GameManager.GRID.processDrag(mouseVector, x0, y0);
		// }

		// else if (OLDState == State.snapPreview)
		// {
		// 	Vector3 snapTo = new Vector3(x0 + dx, y0 + dy, 0);

		// 	if (this.Position != snapTo)
		// 	{
		// 		this.Position = this.Position.MoveToward(snapTo, (float)(25 * delta));
		// 	}
		// 	else
		// 	{
		// 		OLDState = State.idlePreview;
		// 	}
		// }

		// else if (OLDState == State.snapBack)
		// {
		// 	if (this.Position != this.getInitialPosition())
		// 	{
		// 		this.Position = this.Position.MoveToward(this.getInitialPosition(), (float)(25 * delta));
		// 	}
		// 	else
		// 	{
		// 		OLDState = State.idle;
		// 	}
		// }
	}


	public void doPreview(int dx, int dy)
	{
		this.dx = dx;
		this.dy = dy;
		currentState.Trigger(this, PREVIEW);
		
		
		// this.OLDState = State.snapPreview;
	}

	public void doSnapBack()
	{
		currentState.Trigger(this, CANCEL);

		// this.dx = 0;
		// this.dy = 0;
		// this.adjacentX = 0;
		// this.adjacentY = 0;

		// if (OLDState == State.idlePreview || OLDState == State.snapPreview)
		// {
		// 	this.OLDState = State.snapBack;
		// }
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