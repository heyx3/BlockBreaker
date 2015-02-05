using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreDisplayTurnBased : MonoBehaviour
{
	public enum Players
	{
		One,
		Two,
	}

	public Players Player;


	private Text tx;


	void Awake()
	{
		tx = GetComponent<Text>();
	}
	void Update()
	{
		switch (Player)
		{
			case Players.One:
				tx.text = GameplayController_TurnBased.PlayerOneScore.ToString();
				break;
			case Players.Two:
				tx.text = GameplayController_TurnBased.PlayerTwoScore.ToString();
				break;

			default: throw new NotImplementedException();
		}
	}
}