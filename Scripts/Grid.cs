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
	public Token[,] preview;
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

		preview = new Token[xSize, ySize];

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
		bool validDropSpot = updateDropPosition(mouseVector, x0, y0);

		// if (currentState == State.idle)
		// {
		if (validDropSpot)
		{
			//do preview
			doPreview(x0, y0);
		}
		else
		{
			doSnapBack();
		}
		// }
		// else if (currentState == State.preview)
		// {
		// 	//undo preview
		// }
	}

	//updates the dropX and dropY in the grid
	private bool updateDropPosition(Vector3 mouseVector, int x0, int y0)
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
		else if (xInt != x0 && yInt != y0)
		{
			dropX = -1;
			dropY = -1;
			return false;
		}

		//if the drop position is exactly the same as x0,y0 --> invalid
		else if (xInt == x0 && yInt == y0)
		{
			dropX = -1;
			dropY = -1;
			return false;
		}

		//valid drop point
		else
		{
			dropX = xInt;
			dropY = yInt;
			return true;
		}
	}

	private void doPreview(int x0, int y0)
	{
		if (dropX == x0 && dropY == y0)
		{
			GD.Print("TRYING TO PREVIEW DROP AT ORIGIN");
			return;
		}

		resetPreviewGrid();
		preview[dropX, dropY] = matrix[x0, y0];

		if (dropX > x0)
		{
			for (int x = x0 + 1; x < xSize; x++)
			{
				if (x <= dropX)
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
		else if (dropX < x0)
		{
			for (int x = 0; x < x0; x++)
			{
				if (x >= dropX)
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
		else if (dropY > y0)
		{
			for (int y = y0 + 1; y < ySize; y++)
			{
				if (y <= dropY)
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
		else if (dropY < y0)
		{
			for (int y = 0; y < y0; y++)
			{
				if (y >= dropY)
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
		Token token = preview[xPreview, yPreview];

		token.adjacentX = 0;
		token.adjacentY = 0;

		//check -x
		for (int x = xPreview - 1; x >= 0; x--)
		{
			if (token.suit == preview[x, yPreview].suit)
			{
				token.adjacentX += 1;
			}
			else break;
		}

		//check +x
		for (int x = xPreview + 1; x < xSize; x++)
		{
			if (token.suit == preview[x, yPreview].suit)
			{
				token.adjacentX += 1;
			}
			else break;
		}

		//check -y
		for (int y = yPreview - 1; y >= 0; y--)
		{
			if (token.suit == preview[xPreview, y].suit)
			{
				token.adjacentY += 1;
			}
			else break;
		}

		//check +y
		for (int y = yPreview + 1; y < ySize; y++)
		{
			if (token.suit == preview[xPreview, y].suit)
			{
				token.adjacentY += 1;
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
