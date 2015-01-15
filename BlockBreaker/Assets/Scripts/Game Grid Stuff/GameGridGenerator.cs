using UnityEngine;
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
	/// <summary>
	/// The camera that renders the grid blocks.
	/// </summary>
	public Camera BlockViewCam = null;
	/// <summary>
	/// The orthographic size of the camera for rendering the generated grid.
	/// </summary>
	public float OrthoSize = 3.5f;

	public int Seed = 12345679;
	public int NBlockTypes = 4;
	public int NBlocksX = 4,
			   NBlocksY = 7;


	private GameGridBlock GenerateBlock(Vector2 position, Vector2i gridLoc, int colorID)
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
		if (BlockViewCam == null)
		{
			Debug.LogError("'BlockViewCam' field in 'GameGridGenerator' " +
						      "component isn't set!");
		}
	}
	void Start ()
	{
		//Set up the grid.
		Random.seed = Seed;
		GameGrid.Instance.ResetGrid(new Vector2i(NBlocksX, NBlocksY));
		for (int x = 0; x < NBlocksX; ++x)
		{
			for (int y = 0; y < NBlocksY; ++y)
			{
				GenerateBlock(new Vector2(x, y)	, new Vector2i(x, y), Random.Range(0, NBlockTypes));
			}
		}

		//Set up the camera.
		BlockViewCam.transform.position = new Vector3(((float)NBlocksX * 0.5f) - 0.5f,
													  ((float)NBlocksY * 0.5f) - 0.5f,
													  BlockViewCam.transform.position.z);
		BlockViewCam.orthographicSize = OrthoSize;

		Destroy(this);
	}
}