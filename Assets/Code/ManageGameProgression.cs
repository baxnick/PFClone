
using UnityEngine;
using System;
using PushFightLogic;
using SeeSharpMessenger;

/// <summary>
/// Singleton that gives access to critical portions of the
/// game to all behaviours wihout having to constantly use FindObject.
/// </summary>
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

/// <summary>
/// Initialises the singleton when the scene swaps
/// </summary>
public class ManageGameProgression : MonoBehaviour
{
	// Use this for initialization
	void OnEnable ()
	{
		Game.NewMaster ();
	}
}
