using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// A block in the game grid.
/// It will automatically seek to its grid location at a fixed speed specified in GameConstants.
/// </summary>
public class GameGridBlock : MonoBehaviour
{
	public int ColorID;

	private GameGrid grid { get { return GameGrid.Instance; } }
	private Transform tr;


	/// <summary>
	/// Should be called when this block is about to be destroyed.
	/// </summary>
	public void OnBeingCleared()
	{

	}


	void Awake()
	{
		tr = transform;
	}
	void Update()
	{
		//Constantly seek towards the position the block SHOULD be at in the grid.

		Vector3 currentPos = tr.position;
		Vector2i seekPos = grid.GetLocation(this);

		if (seekPos.X != -1 && seekPos.Y != -1)
		{
			Vector2 seekPosF = new Vector2((float)seekPos.X, (float)seekPos.Y);

			Vector2 toSeekPos = seekPosF - (Vector2)currentPos;


			//If the block is close enough to its destination, just snap to it.

			float moveSpeed = Time.deltaTime * GameConstants.Instance.BlockFallSpeed;

			if ((moveSpeed * moveSpeed) > toSeekPos.sqrMagnitude)
			{
				tr.position = new Vector3(seekPosF.x, seekPosF.y, currentPos.z);
			}
			else
			{
				Vector2 moveAmount = toSeekPos.normalized * moveSpeed;
				tr.position += new Vector3(moveAmount.x, moveAmount.y, 0.0f);
			}
		}
	}
}