using UnityEngine;
using System.Collections;

public enum GameMode {
	SOLITAIRE, AI, MULTIPLAYER	
}

public static class MenuChoice
{
	public static GameMode Mode;
}

public class ManageMenu : MonoBehaviour {
	public GameMode ChosenMode {get; private set;}
	bool showMenu = true;
	
	void Awake () {
    	DontDestroyOnLoad (transform.gameObject);
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI () {
		if (!showMenu) return;
		int button_width = 150;
		int centre_xpos = (Screen.width - button_width ) / 2;
		
		if (GUI.Button (new Rect (centre_xpos,10,button_width,100), "Solitaire")) {
				MenuChoice.Mode = GameMode.SOLITAIRE;
				Application.LoadLevel("PushFight");
				showMenu = false;
			}
		if (GUI.Button (new Rect (centre_xpos,110,button_width,100), "AI")) {
				MenuChoice.Mode = GameMode.AI;
				Application.LoadLevel("PushFight");
				showMenu = false;
			}
	}
}
