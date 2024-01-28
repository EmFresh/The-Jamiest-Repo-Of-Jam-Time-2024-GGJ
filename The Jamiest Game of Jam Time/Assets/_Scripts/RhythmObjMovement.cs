using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RhythmObjMovement : MonoBehaviour
{
	public float speed = 1;
	public Vector3 dir = new Vector3();
	public ScoreSystem score;

	// Update is called once per frame
	void Update()
	{
		transform.position += dir.normalized * speed * Time.deltaTime;
	}

	private void OnDestroy()
	{
		score.DecreaseScore(60);
	}
}
