using System;
using UnityEngine;


/// <summary>
/// Manages a game grid block's material.
/// </summary>
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(GameGridBlock))]
public class GameGridBlockRenderer : MonoBehaviour
{
	private Renderer rend;
	private GameGridBlock block;


	void Awake()
	{
		rend = renderer;
		block = GetComponent<GameGridBlock>();
	}


	void LateUpdate()
	{
		rend.sharedMaterial = GameConstants.Instance.BlockMaterials[block.ColorID];
	}
}