using Godot;
using System;
using System.Runtime.Serialization;


public partial class Gem : Area3D
{

	[Export]
	public Label3D debugLbl;

	public string suit = "Default";
	public readonly float speed = 25f;

	public static readonly string PREVIEW = "preview";
	public static readonly string CANCEL = "cancel";
	public static readonly string FREEZE = "freeze";
	public static readonly string IDLE = "idle";


	//state machine
	public StateGem currentState;

	public StateGem idle = new StateGemIdle();
	public StateGem drag = new StateGemDrag();
	public StateGem snapBack = new StateGemSnapBack();
	public StateGem snapPreview = new StateGemSnapPreview();
	public StateGem idlePreview = new StateGemIdlePreview();
	public StateGem tryFreeze = new StateGemTryFreeze();
	public StateGem freeze = new StateGemFreeze();
	public StateGem burst = new StateGemBurst();
	public StateGem fall = new StateGemFall();

	public void changeState(StateGem nextState)
	{
		currentState.ExitState(this, nextState);
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
	// public Vector3 getNewPosition()
	// {
	// 	return new Vector3(x0 + dx, y0 + dy, 0);
	// }

	//adjacent same suit
	public int adjacentX = 0;
	public int adjacentY = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<MeshInstance3D>(suit).Visible = true;
		// currentState = idle;
		currentState = freeze;
		currentState.EnterState(this);
		debugLbl.Visible = GameManager.DEBUG;
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
	}

	//release click event
	public override void _Input(InputEvent evt)
	{
		//requires pre-check for some reason
		if(currentState == drag && evt.IsActionReleased("click"))
		{
			currentState.HandleInput(this, evt);
		}
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		//DEBUG
		if(GameManager.DEBUG)
		{
			debugLbl.Text = "(" + adjacentX + "," + adjacentY + ")";
		}	

		currentState.Update(this, delta);
	}


	public void doPreview(int dx, int dy)
	{
		this.dx = dx;
		this.dy = dy;
		currentState.Trigger(this, PREVIEW);
	}

	public void doSnapBack()
	{
		currentState.Trigger(this, CANCEL);
	}

	public void doFreeze()
	{
		currentState.Trigger(this, FREEZE);
	}

	public void doIdle()
	{
		currentState.Trigger(this, IDLE);
	}


	public void _on_mouse_entered()
	{
		MeshInstance3D mi = this.GetNode<MeshInstance3D>(suit);
		mi.Transparency = 0.5f;
		GameManager.DEBUG_GEM = this.Name + "\n" + 
			"(x0,y0) = " + this.x0 + "," + this.y0 + "\n" +
			"(dx,dy) = " + this.dx + "," + this.dy + "\n" +
			this.currentState.ToString();
	}

	public void _on_mouse_exited()
	{
		MeshInstance3D mi = this.GetNode<MeshInstance3D>(suit);
		mi.Transparency = 0f;
		GameManager.DEBUG_GEM = "-";
	}
}