using UnityEngine;
using System.Collections;
using PushFightLogic;
using SeeSharpMessenger;

public class TileClickHandler : MonoBehaviour {
	
	public Coords pos;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{

	}

	void OnMouseUp ()
	{
		Messenger<Coords>.Invoke("tile.clicked", pos);
	}
}
