using Godot;
using System;

public partial class Grid : Node3D
{

	private System.Random random;
	public static readonly string[] suits = { "clubs", "diamonds", "hearts", "spades" };


	public int dragX = -1;
	public int dragY = -1;

	// public enum State : int
	// {
	// 	idle,
	// 	preview,
	// 	// draggingValid,
	// 	// draggingInvalid,//maybe consolidate
	// 	// generatingPreview,
	// 	// snapBack,
	// 	// submitting,
	// 	// calculatingNeighbors,
	// 	// checkingPops,
	// 	// processingPops,
	// 	// processingDrops,
	// 	nothing
	// }

	// public State currentState;

	[Export]
	public int xSize;

	[Export]
	public int ySize;

	public Gem[,] matrix;
	public Gem[,] preview;
	public Cell[,] cells;

	public int numFrozen;
	public int totalGems;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameManager.GRID = this;
		random = new System.Random();

		matrix = new Gem[xSize, ySize];
		fillGrid();

		preview = new Gem[xSize, ySize];

		cells = new Cell[xSize, ySize];
		fillCells();

		numFrozen = 0;
		totalGems = xSize * ySize;

		// currentState = State.idle;
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
				Gem gem = createRandomGem();

				gem.Position = new Vector3(x, y, 0);
				gem.x0 = x;
				gem.y0 = y;
				matrix[x, y] = gem;

				this.AddChild(gem);
			}
		}
	}

	private Gem createRandomGem()
	{
		Gem gem = GameManager.GemScene.Instantiate<Gem>();
		int i = random.Next(suits.Length);
		gem.suit = suits[i];
		return gem;
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

			//TODO is below code necessary?
			resetPreviewGrid();
			updateAdjacentAll(preview);
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
			//GD.Print("TRYING TO PREVIEW DROP AT ORIGIN");
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

		updateAdjacentAll(preview);
	}

	private void updateAdjacentAll(Gem[,] grid)
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				updateAdjacent(grid, x, y);
			}
		}
	}

	private void updateAdjacent(Gem[,] grid, int xUpdate, int yUpdate)
	{
		Gem gem = grid[xUpdate, yUpdate];

		gem.adjacentX = 0;
		gem.adjacentY = 0;

		//check -x
		for (int x = xUpdate - 1; x >= 0; x--)
		{
			if (gem.suit == grid[x, yUpdate].suit)
			{
				gem.adjacentX += 1;
			}
			else break;
		}

		//check +x
		for (int x = xUpdate + 1; x < xSize; x++)
		{
			if (gem.suit == grid[x, yUpdate].suit)
			{
				gem.adjacentX += 1;
			}
			else break;
		}

		//check -y
		for (int y = yUpdate - 1; y >= 0; y--)
		{
			if (gem.suit == grid[xUpdate, y].suit)
			{
				gem.adjacentY += 1;
			}
			else break;
		}

		//check +y
		for (int y = yUpdate + 1; y < ySize; y++)
		{
			if (gem.suit == grid[xUpdate, y].suit)
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

	public bool canFreezePreview()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (preview[x, y].adjacentX > 1 || preview[x, y].adjacentY > 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void doFreezePreview()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				// 1) update the matrix first
				Gem g = preview[x, y];
				int xm = g.x0 + g.dx;
				int ym = g.y0 + g.dy;
				g.x0 = xm;
				g.y0 = ym;
				g.dx = 0;
				g.dy = 0;
				matrix[xm, ym] = g;

				// 2) then trigger state change
				g.doFreeze();
			}
		}
	}

	public void processBurst(Gem g)
	{
		matrix[g.x0, g.y0] = null;
		preview[g.x0, g.y0] = null;
	}

	public bool checkFall(Gem g)
	{
		int drop = 0;

		for (int y = g.y0 - 1; y >= 0; y--)
		{
			if (matrix[g.x0, y] != null)
			{
				break;
			}
			else
			{
				drop--;
			}
		}

		if (drop != 0)
		{
			//null old matrix position
			matrix[g.x0, g.y0] = null;

			//adjust y0
			g.y0 += drop;

			//add to new position
			matrix[g.x0, g.y0] = g;
			return true;
		}

		else return false;
	}


	public void checkTopRow()
	{
		int yTop = ySize - 1;

		for (int x = 0; x < xSize; x++)
		{
			if (matrix[x, yTop] == null)
			{
				//create gem, set above the grid
				Gem gem = createRandomGem();

				gem.Position = new Vector3(x, ySize, 0);
				gem.x0 = x;
				gem.y0 = yTop;
				matrix[x, yTop] = gem;

				this.AddChild(gem);
				gem.changeState(gem.fall);
			}
		}
	}

	public bool checkAdjacents()
	{
		updateAdjacentAll(matrix);
		return hasUnprocessedBursts();
	}

	private bool hasUnprocessedBursts()
	{
		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				if (matrix[x, y].adjacentX > 1 || matrix[x, y].adjacentY > 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void doIdle()
	{
		resetPreviewGrid();

		for (int x = 0; x < xSize; x++)
		{
			for (int y = 0; y < ySize; y++)
			{
				Gem g = matrix[x,y];
				g.currentState.Trigger(g, Gem.IDLE);
			}
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
