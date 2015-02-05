using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A gameplay controller that lets the local player clear blocks whenever he wants,
///     with just a small pause in between moves to allow for animations to finish.
/// Drops extra blocks in whenever another block is cleared.
/// </summary>
public class GameplayController_Timed : GameplayController
{
	/// <summary>
	/// The player's score, stored statically so that it persists between scenes.
	/// </summary>
	public static int FinalScore;


	/// <summary>
	/// The score gained from clearing a single block.
	/// Larger values are preferable because they mean more precision
	///     in the exponential score gain from clearing extra blocks.
	/// </summary>
	public int BaseBlockScoreValue = 100;
	/// <summary>
	/// The exponent determining how much better the player's score is
	/// when he clears a few large chunks compared to a lot of small chunks.
	/// Should always be greater than (or equal to) 1.0.
	/// </summary>
	public float ScoreBlockExponent = 2.0f;

	/// <summary>
	/// The amount of time the player must wait between moves.
	/// </summary>
	public float MoveWaitTime = 1.0f;
	/// <summary>
	/// The amount of time between a block being destroyed and a replacement being spawned.
	/// </summary>
	public float BlockSpawnWaitTime = 0.6f;

	/// <summary>
	/// The amount of time left before the game ends.
	/// </summary>
	public float TimeLeft = 45.0f;

	/// <summary>
	/// The UI element that displays how much time is left.
	/// </summary>
	public UnityEngine.UI.Text TimeLeftLabel;


	public void Abandon()
	{
		Application.LoadLevel("MainMenu");
	}


	protected override void Awake()
	{
		base.Awake();

		FinalScore = 0;

		OnBlocksClearedChanged += (clearedBlocks, reason) =>
		{
			//Pause the player's input.
			if (reason == ClearBlockActions.PlayerInput)
			{
				StartCoroutine(PauseInputCoroutine());
			}

			//Drop extra blocks in for every block that was cleared.
			StartCoroutine(WaitSpawnCoroutine(clearedBlocks));


			//Calculate the player's score.

			float blockMultiplier = Mathf.Pow (clearedBlocks.Count, ScoreBlockExponent);
			FinalScore += Mathf.RoundToInt(BaseBlockScoreValue * blockMultiplier);
		};
	}

	protected override void Update ()
	{
		base.Update ();

		//Update timing.
		TimeLeft -= Time.deltaTime;
		if (TimeLeft <= 0.0f)
		{
			Application.LoadLevel("PresentTimedScore");
		}

		//Update UI.
		TimeLeftLabel.text = GameConstants.CutOffDecimals(TimeLeft, 1).ToString();
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