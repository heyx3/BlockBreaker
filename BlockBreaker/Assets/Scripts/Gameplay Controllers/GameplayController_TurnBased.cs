using System;
using System.Collections;
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
							   TurnsLeftUIText,
							   Score1UIText,
							   Score2UIText;

	/// <summary>
	/// The minimum amount of time between moves.
	/// </summary>
	public float TurnWaitTime = 2.0f;

	/// <summary>
	/// The number of turns remaining until the game ends.
	/// </summary>
	public int TurnsLeft = 20;
	
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

			//Change turns.
			IsPlayer1Turn = !IsPlayer1Turn;
			TurnsLeft -= 1;
			if (TurnsLeft < 0)
			{
				throw new NotImplementedException("Add a \"ReviewTurnBased\" scene!");
			}
		};
	}
	protected override void Update ()
	{
		base.Update ();
		
		CurrentPlayerUIText.text = (IsPlayer1Turn ? "Player 1" : "Player 2");
		TurnsLeftUIText.text = TurnsLeft.ToString () + " left";
		Score1UIText.text = PlayerOneScore.ToString();
		Score2UIText.text = PlayerTwoScore.ToString();
	}
}