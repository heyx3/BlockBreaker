using System;
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
	/// Gets or sets the number of blocks that have been cleared so far.
	/// </summary>
	public int BlocksCleared
	{
		get { return blocksCleared; }
		set
		{
			int oldVal = blocksCleared;
			blocksCleared = value;

			if (OnBlocksClearedChanged != null)
			{
				OnBlocksClearedChanged(oldVal, value);
			}
		}
	}
	private int blocksCleared;

	/// <summary>
	/// A function that reacts to the "BlocksCleared" property being changed.
	/// </summary>
	public delegate void BlocksClearedChangedReaction(int oldVal, int newVal);
	/// <summary>
	/// Raised when this instance's "BlocksCleared" property is set.
	/// </summary>
	public event BlocksClearedChangedReaction OnBlocksClearedChanged;


	/// <summary>
	/// Will be called after the player does an action.
	/// </summary>
	public abstract void OnPlayerClearedBlock();

	/// <summary>
	/// Override this function to add behavior on awake.
	/// Default behavior: sets the static reference to this controller.
	/// </summary>
	protected virtual void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one 'GameplayController' instance currently active!");
		}
		Instance = this;
	}

	/// <summary>
	/// Override this function to add behavior every update cycle.
	/// </summary>
	protected virtual void Update() { }
}