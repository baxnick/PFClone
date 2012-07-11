
using UnityEngine;
using System;
using System.Collections;
using PushFightLogic;
using System.Collections.Generic;
using SeeSharpMessenger;

/// <summary>
/// Monitors piece placement events and spawns a new graphical representation when one is detected.
/// </summary>
public class PieceManager : MonoBehaviour
{
	public GameObject SquareToken;
	public GameObject RoundToken;
	public Material blackMat;
	public Material whiteMat;
	
	void OnEnable ()
	{
		Messenger<GameToken>.AddListener ("piece.placed", OnPiecePlaced);
	}

	
	void OnDisable ()
	{
		Messenger<GameToken>.RemoveListener ("piece.placed", OnPiecePlaced);
	}

	
	private void OnPiecePlaced (GameToken token)
	{
		GameObject holder;
		if (token.type == PieceType.ROUND)
		{
			holder = (GameObject)Instantiate (RoundToken);
		}
		else if (token.type == PieceType.SQUARE)
		{
			holder = (GameObject)Instantiate (SquareToken);
		}
		else
		{
			throw new InvalidOperationException ();
		}
		
		Transform mesh = holder.transform.FindChild ("Mesh");
		if (token.owner == Player.P1)
		{
			mesh.renderer.material = whiteMat;
		}
		else if (token.owner == Player.P2)
		{
			mesh.renderer.material = blackMat;
		}
		else
		{
			throw new InvalidOperationException ();
		}
		
		holder.transform.parent = GameObject.Find ("GameBoard").transform;
		holder.transform.position = new Vector3 (token.location.x, 0, token.location.y);
		mesh.GetComponent<PieceHandler> ().ID = token.id;
		mesh.GetComponent<PieceHandler> ().OnSpawned ();
	}
}
