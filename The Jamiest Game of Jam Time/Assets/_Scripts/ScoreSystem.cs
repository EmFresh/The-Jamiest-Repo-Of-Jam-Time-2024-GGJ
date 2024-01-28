using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Jobs;

public class ScoreSystem : MonoBehaviour
{
	public float score;
	public TMPro.TextMeshProUGUI text;

	public void IncreaseScore(float value)
	{
		score += value;
	}
	public void DecreaseScore(float value) =>
		IncreaseScore(-value);

	private void Update()
	{
		if(text)
			text.text = $" {(int)score}";
	}

}
