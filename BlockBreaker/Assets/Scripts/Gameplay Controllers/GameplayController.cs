using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A singleton script that runs the gameplay
///     (be it single-player, multiplayer, turn-based multiplayer, etc.).
/// </summary>
public abstract class GameplayController : MonoBehaviour
{
	public static GameplayController Instance { get; private set; }


	//Some accessors for reducing verbosity.
	protected GameGrid Grid { get { return GameGrid.Instance; } }
	protected GameConstants Constants { get { return GameConstants.Instance; } }
	protected PlayerInput LocalPlayer { get { return PlayerInput.Instance; } }


	/// <summary>
	/// Gets the total number of blocks that have been cleared so far.
	/// </summary>
	public int BlocksCleared { get; private set; }


	/// <summary>
	/// The different kinds of actions that can result in a block being cleared.
	/// </summary>
	public enum ClearBlockActions
	{
		PlayerInput,
	}
	/// <summary>
	/// A function that reacts to the "BlocksCleared" property being changed.
	/// Note that the block locations being passed in are likely now occupied
	///     by other blocks that have fallen into the space from above.
	/// </summary>
	public delegate void BlocksClearedChangedReaction(List<Vector2i> clearedBlockLocs,
													  ClearBlockActions reasonForClearing);
	/// <summary>
	/// Raised when this instance's "BlocksCleared" property is set.
	/// </summary>
	public event BlocksClearedChangedReaction OnBlocksClearedChanged;


	public GameObject BlockPrefab = null;
	

	/// <summary>
	/// Creates a basic block with the given properties and adds it to the world grid.
	/// </summary>
	public GameGridBlock CreateBlock(Vector2 position, Vector2i gridLoc, int colorID)
	{
		Transform tr = ((GameObject)Instantiate(BlockPrefab)).transform;
		GameGridBlock block = tr.GetComponent<GameGridBlock>();

		tr.position = new Vector3(position.x, position.y, tr.position.z);
		block.ColorID = colorID;

		GameGrid.Instance.AddBlock(gridLoc, block);

		return block;
	}


	/// <summary>
	/// Reacts to blocks being cleared from the given locations.
	/// </summary>
	public void ClearedBlocks(List<Vector2i> clearedBlockLocs, ClearBlockActions reasonForClearing)
	{
		BlocksCleared += clearedBlockLocs.Count;

		if (OnBlocksClearedChanged != null)
		{
			OnBlocksClearedChanged(clearedBlockLocs, reasonForClearing);
		}
	}


	/// <summary>
	/// Override this function to add behavior on awake.
	/// Default behavior: checks and sets up some static and member fields.
	/// </summary>
	protected virtual void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one 'GameplayController' instance currently active!");
		}
		Instance = this;

		if (BlockPrefab == null)
		{
			Debug.LogError("The 'BlockPrefab' field in the '" + GetType().ToString() + "' isn't set!");
		}
	}

	/// <summary>
	/// Override this function to add behavior every update cycle.
	/// </summary>
	protected virtual void Update() { }
}