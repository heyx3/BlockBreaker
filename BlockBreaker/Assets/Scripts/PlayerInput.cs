using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// A singleton component that handles the local player's touch/click input.
/// </summary>
public class PlayerInput : MonoBehaviour
{
	public static PlayerInput Instance { get; private set; }


	/// <summary>
	/// The camera that renders the grid blocks.
	/// </summary>
	public Camera BlockViewCam = null;
	/// <summary>
	/// While this is true, touch/mouse input will not be noticed by this component.
	/// </summary>
	public bool IsInputDisabled = false;


	void Awake()
	{
		if (Instance != null)
		{
			Debug.LogError("More than one 'PlayerInput' instance currently active!");
		}
		Instance = this;

		if (BlockViewCam == null)
		{
			Debug.LogError("'BlockViewCam' object in 'PlayerInput' component isn't set!");
		}
	}
	
	void Update()
	{
		if (IsInputDisabled)
		{
			return;
		}


		//Get any mouse/touch input.
		Vector2? worldInputPos = null;
		if (Input.GetMouseButtonDown(0))
		{
			worldInputPos = (Vector2)BlockViewCam.ScreenToWorldPoint(Input.mousePosition);
		}
		else if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			worldInputPos = (Vector2)BlockViewCam.ScreenToWorldPoint ((Vector3)Input.touches[0].position);
		}

		//If there was mouse/touch input, see if the point is overlapping anything in the scene.
		if (worldInputPos.HasValue)
		{
			Collider2D hit = Physics2D.OverlapPoint(worldInputPos.Value);
			if (hit != null)
			{
				//If a block was tapped/clicked, clear it.
				GameGridBlock tryBlock = hit.GetComponent<GameGridBlock>();
				if (tryBlock != null)
				{
					List<Vector2i> cleared = GameGrid.Instance.ClearBlock(GameGrid.Instance.GetLocation (tryBlock));
					GameplayController.Instance.ClearedBlocks(cleared, GameplayController.ClearBlockActions.PlayerInput);
				}
			}
		}
	}
}