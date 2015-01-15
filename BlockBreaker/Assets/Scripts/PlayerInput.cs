using UnityEngine;
using System.Collections;


/// <summary>
/// Handles player touch/click input.
/// </summary>
public class PlayerInput : MonoBehaviour
{
	public Camera BlockViewCam = null;


	void Awake()
	{
		if (BlockViewCam == null)
		{
			Debug.LogError("'BlockViewCam' object in 'PlayerInput' component isn't set!");
		}
	}
	
	void Update()
	{
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
				GameGridBlock tryBlock = hit.GetComponent<GameGridBlock>();
				if (tryBlock != null)
				{
					int score = GameGrid.Instance.ClearBlock(GameGrid.Instance.GetLocation (tryBlock));
					Debug.Log ("Score: " + score.ToString ());
				}
			}
		}
	}
}