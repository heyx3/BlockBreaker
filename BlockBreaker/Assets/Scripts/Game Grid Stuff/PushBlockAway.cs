using UnityEngine;


/// <summary>
/// Pushes this object's GameGridBlock away from a certain point.
/// </summary>
[RequireComponent(typeof(GameGridBlock))]
public class PushBlockAway : MonoBehaviour
{
	public float PushTime, HangTime;
	public Vector3 PushAwayFrom;
	public float PushFalloffExponent, PushSpeed;

	private float elapsed;
	private GameGridBlock block;


	void Awake()
	{
		block = GetComponent<GameGridBlock>();
	}
	void Start()
	{
		elapsed = 0.0f;
	}

	void OnDestroy()
	{
		if (block != null)
			block.DisableMovement = false;
	}

	void Update()
	{
		block.DisableMovement = true;

		elapsed += Time.deltaTime;

		if (elapsed < PushTime)
		{
			Vector3 moveDir = block.MyTransform.position - PushAwayFrom;
			float dist = moveDir.magnitude;
			moveDir /= dist;
			moveDir *= (1.0f / (Mathf.Pow(dist, PushFalloffExponent)));
			moveDir *= PushSpeed;

			block.MyTransform.position += moveDir;
		}

		if (elapsed >= PushTime + HangTime)
			Destroy(this);
	}
}