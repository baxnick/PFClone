using UnityEngine;
using System.Collections;
using PushFightLogic;
using System.Collections.Generic;
using SeeSharpMessenger;
using System.Threading;
using System;

public static class Game
{
	public static Player Controlling = Player.P1;
	private static GameStateManager stateMan = null;
	private static GameMaster master = null;
	private static AIEngine TheAI;
	
	public static GameMaster Master()
	{
		return master;
	}
	
	public static GameStateManager StateMan()
	{	
		return stateMan;
	}
	
	public static AIEngine AI()
	{
		return TheAI;
	}
	
	public static void NewMaster()
	{
		master = new GameMaster();
		TheAI = new AIEngine(master);
	}
	
	public static void NewStateMan(GameStateManager manager)
	{
		stateMan = manager;
	}
}

public class ManageGameProgression : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		Game.NewMaster();
		
		if (MenuChoice.Mode == GameMode.AI)
		{
			Messenger<Player, string>.AddListener("turn.phase", MakeAIPlayTurns);	
		}
	}
	
	void OnDisable() {
		Game.AI ().KeepSearching = false;
		
		if (MenuChoice.Mode == GameMode.AI)
		{
			Messenger<Player, string>.RemoveListener("turn.phase", MakeAIPlayTurns);	
		}
	}
	
	const float AI_THINKING_TIME = 15.0f;
	const float AI_CLEANUP_TIME = 5.0f;
	
	bool AIFinished;
	private IEnumerator AITurn(string phase)
	{
		// Wait a moment before beginning because AI disables message passing globally unfortunately
		for (float t = 0.0f; t < 0.1f; t += Time.deltaTime)
	    {
			yield return null;
		}
		
		// Wait 20 seconds before signalling AI to stop work, give it 
		// some time afterward to prepare a play instruction.
		AIFinished = false;
	 	ThreadPool.QueueUserWorkItem(AICallback, phase);
		for (float t = 0.0f; t < AI_THINKING_TIME; t += Time.deltaTime)
	    {
			if (AIFinished) break;
			yield return null;
		}
		
		Game.AI().KeepSearching = false;
		for (float t = 0.0f; t < AI_CLEANUP_TIME; t += Time.deltaTime)
	    {
			if (AIFinished) break;
			yield return null;	
		}
		
		if (!AIFinished || Game.AI().Instructions.Count == 0)
		{
			var ai = Game.AI();
			throw new Exception("AI did not give any instructions");
		}
		else
		{
			foreach (Action instruction in Game.AI ().Instructions)
			{
				for (float t = 0.0f; t < 0.5f; t += Time.deltaTime)
	    		{
					yield return null;
				}
				
				instruction();
			}
		}
		
		yield return null;
			
	}
	
	void AICallback(object Context)
	{
		Game.AI ().Act(Context as string);
		AIFinished = true;
	}
	
	void AIDebug(object obj)
	{
		string asString = obj as string;
		if (asString != null) 
			Debug.Log(asString);
		else
		{
			//Debug.Log ("Object of type " + obj.GetType() + " sent for analysis");
		}
	}
	
	void MakeAIPlayTurns (Player p, string phase)
	{
		Debug.Log("PHASE " + p.ToString() + " " + phase);
		
		if (p != Game.Controlling)
		{
			if (phase == "Placement" || phase == "Movement")
			{
				Game.AI().DebugFn = AIDebug;
				Debug.Log("AI Playing Turn");
				StartCoroutine(AITurn(phase));
			}
		}
	}
}
