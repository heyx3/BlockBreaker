using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreDisplayTimed : MonoBehaviour
{
	Text tx;

	void Awake()
	{
		tx = GetComponent<Text>();
	}
	void Update()
	{
		tx.text = GameplayController_Timed.FinalScore.ToString();
	}
}