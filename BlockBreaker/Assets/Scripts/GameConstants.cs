using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A singleton script that holds game constants for the current scene.
/// </summary>
public class GameConstants : MonoBehaviour
{
	public static GameConstants Instance { get; private set; }


	/// <summary>
	/// The rate per second at which blocks fall towards their target positions.
	/// </summary>
	public float BlockFallSpeed = 3.0f;

	/// <summary>
	/// The block colors.
	/// Each one is a different material to give the artist the maximum amount of freedom.
	/// </summary>
	public List<Material> BlockMaterials = new List<Material>();
	
	/// <summary>
	/// The number of different regular block types allowed in this scene.
	/// </summary>
	public int NBlockTypes = 4;

	public GameObject ClearBlocksEffectPrefab;

	[Serializable]
	public class BlockPushEffectData
	{
		public float PushTime = 0.5f, HangTime = 0.3f;
		public float PushSpeed = 1.0f;
		public float PushFalloffExponent = 1.0f;
	}
	public BlockPushEffectData BlockPushEffect = new BlockPushEffectData();


	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There is more than one 'GameConstants' component in the scene!");
		}
		Instance = this;

		if (BlockMaterials.Count < NBlockTypes)
		{
			Debug.LogError("This scene uses " + NBlockTypes.ToString() + " different block types, " +
						   "but only has " + BlockMaterials.Count.ToString() + " different materials!");
		}
	}

	public void CreateBlockPush(GameGridBlock blockObj, Vector3 pushAwayFrom)
	{
		PushBlockAway push = blockObj.gameObject.AddComponent<PushBlockAway>();
		push.PushAwayFrom = pushAwayFrom;
		push.PushTime = BlockPushEffect.PushTime;
		push.HangTime = BlockPushEffect.HangTime;
		push.PushSpeed = BlockPushEffect.PushSpeed;
		push.PushFalloffExponent = BlockPushEffect.PushFalloffExponent;
	}
}