using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A gameplay controller that lets the local player clear blocks whenever he wants,
///     with just a small pause in between moves to allow for animations to finish.
/// Drops extra blocks in whenever another block is cleared.
/// </summary>
public class GameplayController_SinglePlayer : GameplayController
{
	/// <summary>
	/// The amount of time the player must wait between moves.
	/// </summary>
	public float MoveWaitTime = 1.0f;
	/// <summary>
	/// The amount of time between a block being destroyed and a replacement being spawned.
	/// </summary>
	public float BlockSpawnWaitTime = 0.6f;


	protected override void Awake()
	{
		base.Awake();

		//Pause the player's input whenever he clears a row.
		OnBlocksClearedChanged += (clearedBlocks, reason) =>
			{
				if (reason == ClearBlockActions.PlayerInput)
				{
					StartCoroutine(PauseInputCoroutine());
				}
			};
		//Drop extra blocks in for every block that was cleared.
		OnBlocksClearedChanged += (clearedBlocks, reason) =>
			{
				StartCoroutine(WaitSpawnCoroutine(clearedBlocks));
			};
	}
	

	/// <summary>
	/// Disables the player's input for 'MoveWaitTime' seconds.
	/// </summary>
	private System.Collections.IEnumerator PauseInputCoroutine()
	{
		LocalPlayer.IsInputDisabled = true;
		yield return new WaitForSeconds(MoveWaitTime);
		LocalPlayer.IsInputDisabled = false;
	}
	private System.Collections.IEnumerator WaitSpawnCoroutine(List<Vector2i> clearedBlocks)
	{
		yield return new WaitForSeconds(BlockSpawnWaitTime);

		//Get how many blocks cleared in each column.
		int[] clearsPerCol = new int[Grid.GetGridSize().X];
		for (int x = 0; x < clearsPerCol.Length; ++x)
		{
			clearsPerCol[x] = 0;
		}
		foreach (Vector2i loc in clearedBlocks)
		{
			clearsPerCol[loc.X] += 1;
		}
		
		//Add blocks to each column to compensate for the missing ones.
		for (int x = 0; x < clearsPerCol.Length; ++x)
		{
			if (clearsPerCol[x] > 0)
			{
				//First find the lowest spot with no block above it.
				int lowestY = 0;
				while (Grid.GetBlock(new Vector2i(x, lowestY)) != null)
				{
					lowestY += 1;
				}
				//Now start at that spot and stack up the new blocks so they fall into place.
				for (int y = lowestY; y < lowestY + clearsPerCol[x]; ++y)
				{
					int deltaStart = y - lowestY;
					CreateBlock(new Vector2((float)x, (float)(Grid.GetGridSize().Y + deltaStart)),
					            new Vector2i(x, y),
					            UnityEngine.Random.Range(0, Constants.NBlockTypes));
				}
			}
		}
	}
}