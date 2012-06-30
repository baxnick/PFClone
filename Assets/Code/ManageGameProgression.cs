
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

	
	public static GameMaster Master ()
	{
		return master;
	}

	
	public static GameStateManager StateMan ()
	{	
		return stateMan;
	}

	
	public static AIEngine AI ()
	{
		return TheAI;
	}

	
	public static void NewMaster ()
	{
		master = new GameMaster ();
		TheAI = new AIEngine (master);
	}

	
	public static void NewStateMan (GameStateManager manager)
	{
		stateMan = manager;
	}
}


public class ManageGameProgression : MonoBehaviour
{

	// Use this for initialization
	void OnEnable ()
	{
		Game.NewMaster ();
		
		if (MenuChoice.Mode == GameMode.AI)
		{
			Messenger<Player, string>.AddListener ("turn.phase", MakeAIPlayTurns);	
		}
	}

	
	void OnDisable ()
	{
		Game.AI ().KeepSearching = false;
		
		if (MenuChoice.Mode == GameMode.AI)
		{
			Messenger<Player, string>.RemoveListener ("turn.phase", MakeAIPlayTurns);	
		}
	}

	
	const float AI_PRE_TIME = 2.0f;
	const float AI_THINKING_TIME = 40.0f;
	const float AI_CLEANUP_TIME = 10.0f;
	bool AIFinished;


	private IEnumerator AITurn (string phase)
	{
		// Wait a moment before beginning so remaining events can be sent.
		// This is because AI disables message passing globally at present
		// and also because AI currently insists on CPU starving Unity
		for (float t = 0.0f; t < AI_PRE_TIME; t += Time.deltaTime)
		{
			yield return null;
		}
		
		// Wait about half a minute before signalling AI to stop work, give it 
		// some time afterward to prepare a play instruction.
		AIFinished = false;
		Thread aiThread = new Thread (AICallback);
		aiThread.IsBackground = true;
		//aiThread.Priority = System.Threading.ThreadPriority.Lowest;
		aiThread.Start (phase);
		
		for (float t = 0.0f; t < AI_THINKING_TIME; t += Time.deltaTime)
		{
			if (AIFinished)
			{
				break;
			}
			yield return null;
		}
		
		Game.AI ().KeepSearching = false;
		for (float t = 0.0f; t < AI_CLEANUP_TIME; t += Time.deltaTime)
		{
			if (AIFinished)
			{
				break;
			}
			yield return null;	
		}
		
		if (!AIFinished)
		{
			//var ai = Game.AI(); // Store variable locally to give a debug view if needed
			throw new Exception ("AI did not give any instructions");
		}
		else
		{
			while (Game.AI().ExecuteNextInstruction())
			{
				for (float t = 0.0f; t < 0.5f; t += Time.deltaTime)
				{
					yield return null;
				}
			}
		}
		
		yield return null;
			
	}
	

	void AICallback (object Context)
	{
		Game.AI ().Act (Context as string);
		AIFinished = true;
	}
	
	/**
	 * Output any string debug messages from the AI
	 */
	void AIDebug (object obj)
	{
		string asString = obj as string;
		if (asString != null)
		{
			Debug.Log (asString);
		}
		else
		{
			//Debug.Log ("Object of type " + obj.GetType() + " sent for analysis");
		}
	}

	
	void MakeAIPlayTurns (Player p, string phase)
	{
		Debug.Log ("PHASE " + p.ToString () + " " + phase);
		
		if (p != Game.Controlling)
		{
			if (phase == "Placement" || phase == "Movement")
			{
				Game.AI ().DebugFn = AIDebug;
				Debug.Log ("AI Playing Turn");
				StartCoroutine (AITurn (phase));
			}
		}
	}
}
