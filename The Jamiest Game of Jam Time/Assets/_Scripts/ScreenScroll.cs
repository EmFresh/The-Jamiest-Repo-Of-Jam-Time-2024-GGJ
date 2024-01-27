using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ScreenScroll : MonoBehaviour
{

	public float scale = 1;

	// Update is called once per frame
	void Update()
	{
		var render = GetComponent<Renderer>();
		render.material.mainTextureOffset = new Vector2(transform.position.x * scale, 0);
	}
}
