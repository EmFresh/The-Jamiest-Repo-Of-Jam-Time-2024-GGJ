using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleActions : MonoBehaviour
{
	public void OnMap1()
	{
		SceneManager.LoadScene(1);
	}

	public void OnMap2()
	{
		SceneManager.LoadScene(2);

	}

	public void OnExit()
	{
		Application.Quit();
	}
}
