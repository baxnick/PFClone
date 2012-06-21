using UnityEngine;
using System.Collections;

public class AIWatcher : MonoBehaviour {
	private string nodesExplored = "";
	
	// Use this for initialization
	void Start () {
		StartCoroutine(AnimationLoop(2.0f));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
			GUI.TextField(new Rect(10, Screen.height - 60, 150, 50), nodesExplored);
	}
	
	private IEnumerator AnimationLoop(float pulseTime)
	{
		while (true)
		{
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / pulseTime)
		    {
				yield return null;
			}
			
			nodesExplored = Game.AI().NodesEvaluated.ToString();
		}
	}
}
