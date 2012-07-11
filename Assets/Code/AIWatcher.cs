
using UnityEngine;
using System.Collections;

/// <summary>
/// Monitors the AI <c>NodesEvaluated</c> property and provides a GUI text field displaying it.
/// </summary>
public class AIWatcher : MonoBehaviour
{
	private string nodesExplored = "";
	
	// Use this for initialization
	void Start ()
	{
		StartCoroutine (AnimationLoop (1.0f));
	}

	
	void OnGUI ()
	{
		GUI.TextField (new Rect (10, Screen.height - 60, 150, 50), nodesExplored);
	}

	
	private IEnumerator AnimationLoop (float pulseTime)
	{
		while (true)
		{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / pulseTime)
			{
				yield return null;
			}
			
			nodesExplored = Game.AI ().NodesEvaluated.ToString ();
		}
	}
}
