
using UnityEngine;
using System.Collections;
using SeeSharpMessenger;
using PushFightLogic;
using System.Collections.Generic;

public abstract class UnityGameState
{
	protected GameStateManager Context { get; private set; }

	
	public abstract void Clicked (Coords location);

	
	public virtual void Skipped ()
	{
		Game.Master ().Turn.Control ().Skip ();
	}


	public abstract void Enter ();


	public abstract void Exit ();

	
	public UnityGameState (GameStateManager context)
	{
		Context = context;	
	}
}


[ExecuteInEditMode()]
public class GameStateManager : MonoBehaviour
{
	private UnityGameState State { get; set; }


	private List<TileClickHandler> Tiles = new List<TileClickHandler> ();

	
	public void OnEnable ()
	{	
		State = null;
		
		Messenger<Coords>.AddListener ("tile.clicked", Clicked);
		Messenger<Player,string>.AddListener ("turn.phase", MonitorPhases);
		
		Game.NewStateMan (this);
	}

	
	public void OnDisable ()
	{
		Messenger<Coords>.RemoveListener ("tile.clicked", Clicked);
		Messenger<Player,string>.RemoveListener ("turn.phase", MonitorPhases);	
	}

	
	public void Attach (TileClickHandler tile)
	{
		Tiles.Add (tile);	
	}

	
	public void SwapState (UnityGameState newState)
	{
		if (State != null)
		{
			State.Exit ();
		}
		if (newState != null)
		{
			newState.Enter ();
		}
		State = newState;
		
		if (State == null)
		{
			Debug.Log ("StateMan cleared state");
		}
		else
		{
			Debug.Log ("StateMan changed state " + State.GetType ().ToString ());
		}
	}
	
	//turn.phase
	private void MonitorPhases (Player p, string phase)
	{
		if (phase.Equals ("Placement"))
		{
			SwapState (new PlacementState (this));
		}
		else if (phase.Equals ("Movement"))
		{
			SwapState (new SelectPieceToMoveState (this));
		}
		else if (phase.Equals ("Pushing"))
		{
			SwapState (new SelectPieceToPushState (this));
		}
		else if (phase.Equals ("Ended"))
		{
			SwapState (null);
		}
	}
	
	//tile.clicked
	private void Clicked (Coords location)
	{
		if (State == null)
		{
			return;
		}
		if (MenuChoice.Mode == GameMode.AI && Game.Controlling != Game.Master ().Turn.TurnPlayer)
		{
			return;
		}
		State.Clicked (location);	
	}

	
	private void Update ()
	{
		if (State == null)
		{
			return;
		}
		if (MenuChoice.Mode == GameMode.AI && Game.Controlling != Game.Master ().Turn.TurnPlayer)
		{
			return;
		}
		if (Input.GetKeyDown (KeyCode.Space))
		{
			State.Skipped ();
		}	
	}
	
	public class PlacementState : UnityGameState
	{
		public PlacementState (GameStateManager context) : base(context)
		{
		}
		
		#region implemented abstract members of UnityGameState
		public override void Clicked (Coords location)
		{
			PieceType piece = GameObject.Find ("TheGame").GetComponent<PlacementHUD> ().SelectedPiece;
				
			bool success = Game.Master ().Turn.Control ().Place (piece, location);
			
			if (!success)
			{
				Debug.Log ("Placement failed for some reason");	
			}
		}

	
		public override void Enter ()
		{
			
		}

	
		public override void Exit ()
		{
		}
		#endregion
		
	}

	
	public class SelectPieceToMoveState : UnityGameState
	{
		public SelectPieceToMoveState (GameStateManager context) : base(context)
		{
		}
	
		#region implemented abstract members of UnityGameState
		public override void Clicked (Coords location)
		{
			List<Coords> valid = Game.Master ().Turn.ValidMoves (location);
			
			if (valid.Count == 0)
			{
				return;
			}
			
			Context.SwapState (new MovePieceState (Context, location));
		}

	
		public override void Enter ()
		{
		}

	
		public override void Exit ()
		{
		}
		#endregion
	}

	
	public class MovePieceState : UnityGameState
	{
		private Coords LocationToMove;

		
		public MovePieceState (GameStateManager context, Coords location) : base(context)
		{
			LocationToMove = location;
		}

		#region implemented abstract members of UnityGameState
		public override void Clicked (Coords location)
		{
			Game.Master ().Turn.Control ().Move (LocationToMove, location);
			
			if (Game.Master ().Turn.Phase ().Equals ("Movement"))
			{
				Context.SwapState (new SelectPieceToMoveState (Context));
			}
		}

		
		public override void Enter ()
		{
			List<Coords> valid = Game.Master ().Turn.ValidMoves (LocationToMove);
			var validTiles = Context.Tiles.FindAll (t => valid.Contains (t.pos));
			validTiles.ForEach (t => t.renderer.material.color = Color.green);
		}


		public override void Exit ()
		{
			Context.Tiles.ForEach (t => t.renderer.material.color = Color.white);
		}
		
		#endregion
	}

	
	public class SelectPieceToPushState : UnityGameState
	{
		public SelectPieceToPushState (GameStateManager context) : base(context)
		{
		}
		
		#region implemented abstract members of UnityGameState
		public override void Clicked (Coords location)
		{
			List<Coords> valid = Game.Master ().Turn.ValidPushes (location);
			
			if (valid.Count == 0)
			{
				return;
			}
			
			Context.SwapState (new PushPieceState (Context, location));
		}


		public override void Enter ()
		{
		}


		public override void Exit ()
		{
		}
		#endregion
		
	}

	
	public class PushPieceState : UnityGameState
	{
		private Coords PieceToPush;

		
		public PushPieceState (GameStateManager context, Coords location) : base(context)
		{
			PieceToPush = location;
		}

		#region implemented abstract members of UnityGameState
		public override void Clicked (Coords location)
		{			
			Game.Master ().Turn.Control ().Push (PieceToPush, location);
			
			if (Game.Master ().Turn.Phase ().Equals ("Pushing"))
			{
				Context.SwapState (new SelectPieceToPushState (Context));
			}
		}


		public override void Enter ()
		{
			List<Coords> valid = Game.Master ().Turn.ValidPushes (PieceToPush);
			var validTiles = Context.Tiles.FindAll (t => valid.Contains (t.pos));
			validTiles.ForEach (t => t.renderer.material.color = Color.green);
		}


		public override void Exit ()
		{
			Context.Tiles.ForEach (t => t.renderer.material.color = Color.white);
		}
		#endregion
	}
}
