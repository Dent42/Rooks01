using Godot;
using System;

public partial class Grid : Node3D
{

	private System.Random random;
	public static readonly string[] suits = { "clubs", "diamonds", "hearts", "spades" };


	public int dragX = -1;
	public int dragY = -1;

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

	public Gem[,] matrix;
	public Gem[,] preview;
	public Cell[,] cells;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.GRID = this;
		random = new System.Random();

		cells = new Cell[xSize, ySize];
		fillCells();

		matrix = new Gem[xSize, ySize];
		fillGrid();

		preview = new Gem[xSize, ySize];

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
				Gem gem = GameManager.GemScene.Instantiate<Gem>();
				int i = random.Next(suits.Length);
				gem.suit = suits[i];

				gem.Position = new Vector3(x, y, 0);
				gem.x0 = x;
				gem.y0 = y;
				matrix[x, y] = gem;

				this.AddChild(gem);
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


	public bool trySubmit()
	{
		return false;
	}

	public bool processDrag(Vector3 mouseVector, int x0, int y0)
	{
		bool validDragSpot = updateDragPosition(mouseVector, x0, y0);

		// if (currentState == State.idle)
		// {
		if (validDragSpot)
		{
			//do preview
			doPreview(x0, y0);
		}
		else
		{
			doSnapBack();
		}
		return validDragSpot;
	}

	//updates the dragX and dragY in the grid
	private bool updateDragPosition(Vector3 mouseVector, int x0, int y0)
	{
		int xInt = (int)Math.Round(mouseVector.X);
		int yInt = (int)Math.Round(mouseVector.Y);

		// if the drag position is off the grid --> invalid
		if (xInt < 0 || yInt < 0 || xInt >= xSize || yInt >= ySize)
		{
			dragX = -1;
			dragY = -1;
			return false;
		}

		//if the drag position is not the same x or same y --> invalid
		else if (xInt != x0 && yInt != y0)
		{
			dragX = -1;
			dragY = -1;
			return false;
		}

		//if the drag position is exactly the same as x0,y0 --> invalid
		else if (xInt == x0 && yInt == y0)
		{
			dragX = -1;
			dragY = -1;
			return false;
		}

		//valid drag point
		else
		{
			dragX = xInt;
			dragY = yInt;
			return true;
		}
	}

	private void doPreview(int x0, int y0)
	{
		if (dragX == x0 && dragY == y0)
		{
			GD.Print("TRYING TO PREVIEW DROP AT ORIGIN");
			return;
		}

		resetPreviewGrid();
		preview[dragX, dragY] = matrix[x0, y0];

		if (dragX > x0)
		{
			for (int x = x0 + 1; x < xSize; x++)
			{
				if (x <= dragX)
				{
					matrix[x, y0].doPreview(-1, 0);
					preview[x - 1, y0] = matrix[x, y0];
				}
				else
				{
					matrix[x, y0].doSnapBack();
				}
			}
		}
		else if (dragX < x0)
		{
			for (int x = 0; x < x0; x++)
			{
				if (x >= dragX)
				{
					matrix[x, y0].doPreview(1, 0);
					preview[x + 1, y0] = matrix[x, y0];
				}
				else
				{
					matrix[x, y0].doSnapBack();
				}
			}
		}
		else if (dragY > y0)
		{
			for (int y = y0 + 1; y < ySize; y++)
			{
				if (y <= dragY)
				{
					matrix[x0, y].doPreview(0, -1);
					preview[x0, y - 1] = matrix[x0, y];
				}
				else
				{
					matrix[x0, y].doSnapBack();
				}
			}
		}
		else if (dragY < y0)
		{
			for (int y = 0; y < y0; y++)
			{
				if (y >= dragY)
				{
					matrix[x0, y].doPreview(0, 1);
					preview[x0, y + 1] = matrix[x0, y];
				}
				else
				{
					matrix[x0, y].doSnapBack();
				}
			}
		}

		updateAdjacentAll();
	}

	private void updateAdjacentAll()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				updateAdjacent(x, y);
			}
		}
	}

	private void updateAdjacent(int xPreview, int yPreview)
	{
		Gem gem = preview[xPreview, yPreview];

		gem.adjacentX = 0;
		gem.adjacentY = 0;

		//check -x
		for (int x = xPreview - 1; x >= 0; x--)
		{
			if (gem.suit == preview[x, yPreview].suit)
			{
				gem.adjacentX += 1;
			}
			else break;
		}

		//check +x
		for (int x = xPreview + 1; x < xSize; x++)
		{
			if (gem.suit == preview[x, yPreview].suit)
			{
				gem.adjacentX += 1;
			}
			else break;
		}

		//check -y
		for (int y = yPreview - 1; y >= 0; y--)
		{
			if (gem.suit == preview[xPreview, y].suit)
			{
				gem.adjacentY += 1;
			}
			else break;
		}

		//check +y
		for (int y = yPreview + 1; y < ySize; y++)
		{
			if (gem.suit == preview[xPreview, y].suit)
			{
				gem.adjacentY += 1;
			}
			else break;
		}
	}




	public void doSnapBack()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				matrix[x, y].doSnapBack();
			}
		}
	}

	private void resetPreviewGrid()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				preview[x, y] = matrix[x, y];
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
