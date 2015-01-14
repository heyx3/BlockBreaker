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
	/// Starts a new block grid with the given blocks.
	/// Use "null" for grid elements that should be empty.
	/// </summary>
	public void InitializeGrid(GameGridBlock[,] _blocks)
	{
		blocks = new GameGridBlock[_blocks.GetLength(0), _blocks.GetLength(1)];

		for (Vector2i loc = new Vector2i(0, 0); loc.X < _blocks.GetLength(0); ++loc.X)
		{
			for (loc.Y = 0; loc.Y < _blocks.GetLength(1); ++loc.Y)
			{
				blocks[loc.X, loc.Y] = _blocks[loc.X, loc.Y];

				if (blocks[loc.X, loc.Y] != null)
				{
					poses.Add(blocks[loc.X, loc.Y], loc);
				}
			}
		}
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
	/// Clears the given location and all blocks that are near it, causing blocks above to fall.
	/// Returns the number of blocks that were cleared in total.
	/// If the given location is empty, returns 0.
	/// </summary>
	public int ClearBlock(Vector2i loc)
	{
		if (blocks[loc.X, loc.Y] == null)
		{
			return 0;
		}

		int colorID = blocks[loc.X, loc.Y].ColorID;

		blocks[loc.X, loc.Y].OnBeingCleared();
		poses.Remove(blocks[loc.X, loc.Y]);
		Destroy(blocks[loc.X, loc.Y].gameObject);

		//Clear all the adjacent blocks of the same color and count how many there were.
		int count = 1;
		if (loc.X > 0 && blocks[loc.X - 1, loc.Y].ColorID == colorID)
		{
			count += ClearBlock(new Vector2i(loc.X - 1, loc.Y));
		}
		if (loc.Y > 0 && blocks[loc.X, loc.Y - 1].ColorID == colorID)
		{
			count += ClearBlock(new Vector2i(loc.X, loc.Y - 1));
		}
		if (loc.X < blocks.GetLength(0) - 1 && blocks[loc.X + 1, loc.Y].ColorID == colorID)
		{
			count += ClearBlock(new Vector2i(loc.X + 1, loc.Y));
		}
		if (loc.Y < blocks.GetLength(1) - 1 && blocks[loc.X, loc.Y + 1].ColorID == colorID)
		{
			count += ClearBlock(new Vector2i(loc.X, loc.Y + 1));
		}

		return count;
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