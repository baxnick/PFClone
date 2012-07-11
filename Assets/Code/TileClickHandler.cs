
using UnityEngine;
using System.Collections;
using PushFightLogic;
using SeeSharpMessenger;

/// <summary>
/// An individual click handler attached to each tile, 
/// it rebroadcasts the message so it can be handled in a central location. 
/// </summary>
public class TileClickHandler : MonoBehaviour
{
	
	public Coords pos;

	void OnMouseUp ()
	{
		Messenger<Coords>.Invoke ("tile.clicked", pos);
	}
}
