using Godot;
using System;

public partial class GameManager : Node
{

	public static GameManager GM;
	public static Grid GRID;

	public static PackedScene CellScene;
	public static PackedScene GemScene;
	private Plane zeroPlane;

	

	[Export]
	public Camera3D camera;



	public Vector3 GetMouseCoordinates3D()
	{
		Vector2 position2D = this.GetViewport().GetMousePosition();
		Vector3 position3D = (Vector3)zeroPlane.IntersectsRay(camera.ProjectRayOrigin(position2D),
			camera.ProjectLocalRayNormal(position2D));

		//set the z position to exactly zero
		position3D.Z = 0;

		return position3D;
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GM = this;
		GemScene = GD.Load<PackedScene>("res://Scenes/gem.tscn");
		CellScene = GD.Load<PackedScene>("res://Scenes/cell.tscn");
		zeroPlane = new Plane(new Vector3(0, 0, 1));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
