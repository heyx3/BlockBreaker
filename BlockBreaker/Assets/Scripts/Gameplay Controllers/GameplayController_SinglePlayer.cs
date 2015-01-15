using System;
using UnityEngine;


/// <summary>
/// A gameplay controller that lets the local player clear blocks whenever he wants,
///     with just a small pause in between moves to allow for animations to finish.
/// </summary>
public class GameplayController_SinglePlayer : GameplayController
{
	/// <summary>
	/// The amount of time the player must wait between moves.
	/// </summary>
	public float MoveWaitTime = 1.0f;


	public override void OnPlayerClearedBlock()
	{
		StartCoroutine(PauseInputCoroutine());
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
}