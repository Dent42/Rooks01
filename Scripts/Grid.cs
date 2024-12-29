using Godot;
using System;

public partial class Grid : Node3D
{

	private System.Random random;
	public static readonly string[] suits = { "clubs", "diamonds", "hearts", "spades" };


	public int dropX = -1;
	public int dropY = -1;

	public enum State : int
	{
		idle,
		preview,
		// draggingValid,
		// draggingInvalid,//maybe consolidate
		// generatingPreview,
		// snapBack,
		// submitting,
		// calculatingNeighbors,
		// checkingPops,
		// processingPops,
		// processingDrops,
		nothing
	}

	public State currentState;

	[Export]
	public int xSize;

	[Export]
	public int ySize;

	public Token[,] matrix;
	public Cell[,] cells;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.GRID = this;
		random = new System.Random();

		cells = new Cell[xSize, ySize];
		fillCells();

		matrix = new Token[xSize, ySize];
		fillGrid();

		currentState = State.idle;
	}

	public void fillCells()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				Cell cell = GameManager.CellScene.Instantiate<Cell>();
				cell.Position = new Vector3(x, y, 0);
				cells[x, y] = cell;

				//cell.Owner = this.GetTree().EditedSceneRoot;
				this.AddChild(cell);
			}
		}
	}

	public void fillGrid()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				Token token = GameManager.TokenScene.Instantiate<Token>();
				int i = random.Next(suits.Length);
				token.suit = suits[i];

				token.Position = new Vector3(x, y, 0);
				token.x0 = x;
				token.y0 = y;
				// token.getInitialPosition() = new Vector3(x, y, 0);
				matrix[x, y] = token;

				//token.Owner = this.GetTree().EditedSceneRoot;
				this.AddChild(token);
			}
		}
	}

	public void setValidCells(int xValue, int yValue, bool isValid)
	{
		//set all x's for given y value
		for (int x = 0; x < xSize; x++)
		{
			cells[x, yValue].setValid(isValid);
		}

		//set all y's for given x value
		for (int y = 0; y < ySize; y++)
		{
			cells[xValue, y].setValid(isValid);
		}
	}

	public void processDrag(Vector3 mouseVector, int x0, int y0)
	{

	}

	//updates the dropX and dropY in the grid
	public bool updateDropPosition(Vector3 mouseVector, int x0, int y0)
	{
		int xInt = (int)Math.Round(mouseVector.X);
		int yInt = (int)Math.Round(mouseVector.Y);

		// if the drop position is off the grid --> invalid
		if (xInt < 0 || yInt < 0 || xInt >= xSize || yInt >= ySize)
		{
			dropX = -1;
			dropY = -1;
			return false;
		}
		//if the drop position is not the same x or same y --> invalid
		else if(xInt != x0 && yInt != y0)
		{
			dropX = -1;
			dropY = -1;
			return false;
		}
		else //valid drop point
		{
			dropX = xInt;
			dropY = yInt;
			return true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
