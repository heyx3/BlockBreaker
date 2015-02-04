using UnityEngine;
using System.Collections;


public class KillAfterTime : MonoBehaviour
{
	public float KillTime = 5.0f;

	void Update()
	{
		KillTime -= Time.deltaTime;
		if (KillTime <= 0.0f)
			Destroy(gameObject);
	}
}