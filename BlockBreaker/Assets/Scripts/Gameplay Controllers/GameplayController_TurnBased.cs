using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplayController_TurnBased : GameplayController
{
	public static int PlayerOneScore, PlayerTwoScore;

	/// <summary>
	/// There are two players, and it's either player 1's turn or player 2's turn.
	/// </summary>
	[NonSerialized]
	public bool IsPlayer1Turn = true;


	public UnityEngine.UI.Text CurrentPlayerUIText,
							   TurnsLeftUIText;

	/// <summary>
	/// The minimum amount of time between moves.
	/// </summary>
	public float TurnWaitTime = 2.0f;

	/// <summary>
	/// The number of turns remaining until the game ends.
	/// </summary>
	public int TurnsLeft = 20;

	/// <summary>
	/// The amount of time between a block being destroyed and a replacement being spawned.
	/// </summary>
	public float BlockSpawnWaitTime = 0.6f;
	
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

	
	public void Abandon()
	{
		Application.LoadLevel("MainMenu");
	}

	protected override void Awake()
	{
		base.Awake();

		PlayerOneScore = 0;
		PlayerTwoScore = 0;

		//When the blocks are cleared, change turns.
		OnBlocksClearedChanged += (clearedBlocks, reasonForClearing) =>
		{
			//Pause the player's input.
			StartCoroutine(PauseInputCoroutine(TurnWaitTime));

			//Update score.
			float blockMultiplier = Mathf.Pow (clearedBlocks.Count, ScoreBlockExponent);
			if (IsPlayer1Turn)
			{
				PlayerOneScore += Mathf.RoundToInt(BaseBlockScoreValue * blockMultiplier);
			}
			else
			{
				PlayerTwoScore += Mathf.RoundToInt(BaseBlockScoreValue * blockMultiplier);
			}

			//Drop extra blocks in for every block that was cleared.
			StartCoroutine(WaitSpawnCoroutine(clearedBlocks));

			//Change turns.
			IsPlayer1Turn = !IsPlayer1Turn;
			TurnsLeft -= 1;
		};
	}
	protected override void Update ()
	{
		base.Update ();

		//Update UI text.
		CurrentPlayerUIText.text = (IsPlayer1Turn ? "Player 1" : "Player 2");
		TurnsLeftUIText.text = TurnsLeft.ToString () + " left";

		//Check for end of game.
		if (!LocalPlayer.IsInputDisabled && TurnsLeft <= 0)
		{
			Application.LoadLevel("PresentTurnBasedScore");
		}
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