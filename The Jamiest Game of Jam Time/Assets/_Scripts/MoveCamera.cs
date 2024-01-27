using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MoveCamera : MonoBehaviour
{

	// Update is called once per frame
	void Update()
	{
		transform.position += new Vector3(Time.deltaTime * 10, 0, 0);
	}
}
