using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour
{
	public void OnSingleplayerButton()
	{
		Debug.Log ("Starting single-player");
	}

	public void OnMultiplayerButton()
	{
		Debug.Log ("Starting multi-player");
	}
}
