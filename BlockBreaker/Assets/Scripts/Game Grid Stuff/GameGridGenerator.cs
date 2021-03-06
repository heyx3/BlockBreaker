﻿using UnityEngine;
using System.Collections;


/// <summary>
/// Generates a game grid upon Start.
/// </summary>
public class GameGridGenerator : MonoBehaviour
{
	/// <summary>
	/// The template for creating game grid blocks.
	/// </summary>
	public GameObject BlockPrefab = null;

	public int Seed = 12345679;
	public int NBlocksX = 4,
			   NBlocksY = 7;


	public GameGridBlock GenerateBlock(Vector2 position, Vector2i gridLoc, int colorID)
	{
		Transform tr = ((GameObject)Instantiate(BlockPrefab)).transform;
		GameGridBlock block = tr.GetComponent<GameGridBlock>();

		tr.position = new Vector3(position.x, position.y, tr.position.z);
		block.ColorID = colorID;

		GameGrid.Instance.AddBlock(gridLoc, block);

		return block;
	}


	void Awake()
	{
		if (BlockPrefab == null)
		{
			Debug.LogError("'BlockPrefab' field in 'GameGridGenerator' " +
						     "component isn't set!");
		}
	}
	void Start()
	{
		//Set up the grid.
		//Start the objects pushed radially away from their target positions
		//    to create an interesting intro animation.
		Random.seed = Seed;
		GameGrid.Instance.ResetGrid(new Vector2i(NBlocksX, NBlocksY));
		Vector2 gridCenter = new Vector2((float)(NBlocksX * 0.5f) - 0.5f,
										 (float)(NBlocksY * 0.5f) - 0.5f);
		for (int x = 0; x < NBlocksX; ++x)
		{
			for (int y = 0; y < NBlocksY; ++y)
			{
				Vector2 target = new Vector2((float)x, (float)y);
				Vector2 startPosDelta = (target - gridCenter) *
										(1.0f * Mathf.Pow((target - gridCenter).magnitude, 0.35f));
				GenerateBlock(target + startPosDelta, new Vector2i(x, y),
							  Random.Range(0, GameConstants.Instance.NBlockTypes)).TempMoveSpeedScale = 0.25f;
			}
		}


		//Disable the player's input for a short time to let the blocks settle into place.
		StartCoroutine(PauseInputCoroutine());
	}

	private IEnumerator PauseInputCoroutine()
	{
		PlayerInput.Instance.IsInputDisabled = true;
		yield return new WaitForSeconds(2.5f);
		PlayerInput.Instance.IsInputDisabled = false;
	}
}