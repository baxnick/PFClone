
using UnityEngine;
using System.Collections;
using PushFightLogic;

[ExecuteInEditMode()]
/// <summary>
/// Composes the game board graphics and scripts using a prototype representation of the board.
/// </summary>
/// <remarks>
/// Executes in edit mode to make it easy to be able to place 
/// camera etc correctly during development.
/// </remarks>
public class SetupGameBoard : MonoBehaviour
{
	public GameObject tileGameObject;
	public GameObject railGameObject;
	// Use this for initialization
	void Start ()
	{
		if (GameObject.Find ("GameBoard") != null)
		{
			DestroyImmediate (GameObject.Find ("GameBoard"));
		}
		GameObject emptyTileContainer = new GameObject ("GameBoard");
		
		Board board = Board.Create ();
		foreach (BoardSquare tile in board.Squares)
		{
			Vector3 pos = transform.position;
			pos.x += tile.Pos.x * 1.01f;
			if (tile.Pos.x >= board.Squares.GetLength (0) / 2)
			{
				pos.x += 0.05f;
			}
			pos.z += tile.Pos.y * 1.01f;
			
			GameObject newHolder = null;
			if (tile.Type == BoardSquareType.RAIL)
			{
				newHolder = (GameObject)Instantiate (railGameObject, pos, transform.rotation);
			}
			else if (tile.Type == BoardSquareType.NORMAL)
			{
				newHolder = (GameObject)Instantiate (tileGameObject, pos, transform.rotation);
				var newTile = newHolder.transform.Find ("TileMesh").gameObject;
				var clickScript = (TileClickHandler)newTile.AddComponent (typeof(TileClickHandler));
				clickScript.pos = new Coords () {x = tile.Pos.x, y = tile.Pos.y};
				Game.StateMan ().Attach (clickScript);
			}
			
			if (newHolder != null)
			{
				newHolder.transform.parent = emptyTileContainer.transform;
			}
		}
	}
}
