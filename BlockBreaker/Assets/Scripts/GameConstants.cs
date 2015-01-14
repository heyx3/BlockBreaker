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


	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("There is more than one 'GameConstants' component in the scene!");
		}

		Instance = this;
	}
}