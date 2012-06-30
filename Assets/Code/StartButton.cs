
using UnityEngine;
using System.Collections;
using PushFightLogic;
using SeeSharpMessenger;

public class StartButton : MonoBehaviour
{
	public bool GameStarted = false;
	private Player CurrentPlayer;
	
	// Use this for initialization
	void Start ()
	{
	}
	
	
	void OnEnable ()
	{
		
		Messenger<Player>.AddListener ("turn.begin", TurnBegin);
		Messenger<Player>.AddListener ("game.over", GameOver);
	}

	
	void OnDisable ()
	{
		
		Messenger<Player>.RemoveListener ("turn.begin", TurnBegin);
		Messenger<Player>.RemoveListener ("game.over", GameOver);
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	
	void OnGUI ()
	{
		GameMaster master = Game.Master ();
		
		if (GameStarted)
		{
			GUI.TextField (new Rect (10, 10, 150, 50), "Player: " + CurrentPlayer.ToString ());
			GUI.TextField (new Rect (160, 10, 150, 50), "Round: " + master.round);
			GUI.TextField (new Rect (10, 60, 150, 50), master.Turn.Phase ());
		}
		else
		{
			if (GUI.Button (new Rect (10, 10, 150, 100), "Start"))
			{
				master.Reset ();
				
				foreach (GameObject piece in GameObject.FindGameObjectsWithTag("Piece"))
				{
					Destroy (piece);	
				}
			}
		}
	}

	#region GameListener implementation
	public void GameOver (Player winner)
	{
		GameStarted = false;
	}
	#endregion

	#region TurnListener implementation
	public void ChangedPhase (string name)
	{
	}


	public void TurnBegin (Player player)
	{
		CurrentPlayer = player;
		GameStarted = true;
	}


	public void TurnOver ()
	{
		
	}
	#endregion
}
