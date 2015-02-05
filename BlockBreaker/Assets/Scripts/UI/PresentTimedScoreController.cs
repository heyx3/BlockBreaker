using UnityEngine;
using System.Collections;

public class PresentTimedScoreController : MonoBehaviour
{
	public void OnMainMenuButton()
	{
		Application.LoadLevel("MainMenu");
	}
}