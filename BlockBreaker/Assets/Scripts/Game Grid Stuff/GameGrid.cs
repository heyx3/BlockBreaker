using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// A singleton script that represents the grid of blocks.
/// </summary>
public class GameGrid : MonoBehaviour
{
	public static GameGrid Instance { get; private set; }
	
	
	/// <summary>
	/// The blocks, arranged in the order they should be in on the game grid.
	/// </summary>
	private GameGridBlock[,] blocks = null;
	/// <summary>
	/// The target position of each block. Provides a way to quickly look up any block's position.
	/// </summary>
	private Dictionary<GameGridBlock, Vector2i> poses = new Dictionary<GameGridBlock, Vector2i>();
	
	
	/// <summary>
	/// Empties the grid and resizes it to the given size.
	/// </summary>
	public void ResetGrid(Vector2i size)
	{
		blocks = new GameGridBlock[size.X, size.Y];
		for (int x = 0; x < size.X; ++x)
			for (int y = 0; y < size.Y; ++y)
				blocks[x, y] = null;
	}
	
	/// <summary>
	/// Gets the width/height of the grid.
	/// </summary>
	public Vector2i GetGridSize()
	{
		return new Vector2i(blocks.GetLength(0), blocks.GetLength(1));
	}

	
	/// <summary>
	/// Outputs the coordinates of the given block in the grid.
	/// If the given block doesn't exist in the grid, returns {-1, -1}.
	/// </summary>
	public Vector2i GetLocation(GameGridBlock block)
	{
		if (poses.ContainsKey(block))
		{
			return poses[block];
		}
		else
		{
			return new Vector2i(-1, -1);
		}
	}
	/// <summary>
	/// Gets the block at the given location.
	/// Returns "null" if the given location doesn't contain a block.
	/// </summary>
	public GameGridBlock GetBlock(Vector2i loc)
	{
		return blocks[loc.X, loc.Y];
	}

	public IEnumerable<GameGridBlock> GetBlocks()
	{
		return poses.Keys;
	}

	/// <summary>
	/// Adds the given block to this grid at the given location.
	/// If the given location is occupied, the new block is NOT placed.
	/// Returns whether the given block was successfully placed.
	/// </summary>
	public bool AddBlock(Vector2i loc, GameGridBlock block)
	{
		if (blocks[loc.X, loc.Y] != null)
		{
			return false;
		}
		
		poses.Add(block, loc);
		blocks[loc.X, loc.Y] = block;
		
		return true;
	}
	/// <summary>
	/// Moves the given block to the given new location.
	/// Assumes no block is there already.
	/// </summary>
	public void MoveBlock(GameGridBlock block, Vector2i newLoc)
	{
		blocks[newLoc.X, newLoc.Y] = block;
		blocks[poses[block].X, poses[block].Y] = null;
		poses[block] = newLoc;
	}

	/// <summary>
	/// Clears the given location and all blocks that are near it, causing blocks above to fall.
	/// Returns the location of the blocks that were cleared.
	/// </summary>
	public List<Vector2i> ClearBlock(Vector2i loc)
	{
		if (blocks[loc.X, loc.Y] == null)
		{
			return new List<Vector2i>();
		}
		
		int colorID = blocks[loc.X, loc.Y].ColorID;
		
		
		//Initialize a grid representing whether each block has just been cleared.
		bool[,] wasCleared = new bool[blocks.GetLength (0), blocks.GetLength (1)];
		for (int x = 0; x < blocks.GetLength(0); ++x)
		{
			for (int y = 0; y < blocks.GetLength(1); ++y)
			{
				wasCleared[x, y] = false;
			}
		}
		
		//Clear all the blocks.
		List<Vector2i> cleared = ClearBlockRecursive(colorID, loc, wasCleared);

		//For every cleared block (working from the top down),
		//    pull down all block above it by one space.
		for (int x = 0; x < blocks.GetLength(0); ++x)
		{
			for (int y = blocks.GetLength(1) - 1; y >= 0; --y)
			{
				if (wasCleared[x, y])
				{
					for (int yAbove = y + 1; yAbove < blocks.GetLength(1); ++yAbove)
					{
						if (blocks[x, yAbove] != null)
						{
							MoveBlock(blocks[x, yAbove], new Vector2i(x, yAbove - 1));
						}
					}
				}
			}
		}


		return cleared;
	}
	/// <summary>
	/// Clears this block, then clears all adjacent blocks
	/// of the same color as this one, recursively.
	/// Returns the blocks that have been cleared.
	private List<Vector2i> ClearBlockRecursive(int colorID, Vector2i loc, bool[,] wasCleared)
	{
		//Destroy the block.
		blocks[loc.X, loc.Y].OnBeingCleared();
		poses.Remove(blocks[loc.X, loc.Y]);
		Destroy(blocks[loc.X, loc.Y].gameObject);
		blocks[loc.X, loc.Y] = null;
		
		List<Vector2i> cleared = new List<Vector2i>() { loc };

		//Clear all the adjacent blocks of the same color and count how many there were.
		//Go up along the Y first to make sure all blocks above this one are cleared before blocks are pulled down.
		if (loc.Y < blocks.GetLength(1) - 1 && blocks[loc.X, loc.Y + 1] != null &&
		    blocks[loc.X, loc.Y + 1].ColorID == colorID)
		{
			cleared.AddRange(ClearBlockRecursive(colorID, new Vector2i(loc.X, loc.Y + 1), wasCleared));
		}
		if (loc.X > 0 && blocks[loc.X - 1, loc.Y] != null &&
		    blocks[loc.X - 1, loc.Y].ColorID == colorID)
		{
			cleared.AddRange(ClearBlockRecursive(colorID, new Vector2i(loc.X - 1, loc.Y), wasCleared));
		}
		if (loc.Y > 0 && blocks[loc.X, loc.Y - 1] != null &&
		    blocks[loc.X, loc.Y - 1].ColorID == colorID)
		{
			cleared.AddRange(ClearBlockRecursive(colorID, new Vector2i(loc.X, loc.Y - 1), wasCleared));
		}
		if (loc.X < blocks.GetLength(0) - 1 && blocks[loc.X + 1, loc.Y] != null &&
		    blocks[loc.X + 1, loc.Y].ColorID == colorID)
		{
			cleared.AddRange(ClearBlockRecursive(colorID, new Vector2i(loc.X + 1, loc.Y), wasCleared));
		}

		wasCleared[loc.X, loc.Y] = true;
		
		return cleared;
	}
	
	
	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There is more than one 'GameGrid' component in the scene!");
		}
		
		Instance = this;
	}
	
	void Update()
	{
		
	}
}