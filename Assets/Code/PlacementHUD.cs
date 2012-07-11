
using UnityEngine;
using System.Collections;
using PushFightLogic;
using SeeSharpMessenger;

/// <summary>
/// Displays a help graphic during placment phase showing piece types.
/// Monitors keyboard and changes the piece type being placed accordingly.
/// </summary>
public class PlacementHUD : MonoBehaviour
{
	public Texture PlacementHintImage;
	public Texture PlacementFlash;

	
	public PieceType SelectedPiece { get; private set; }


	public bool IsPlacementPhase = false;
	private bool keyWasPressed = false;
	
	void OnEnable ()
	{
		Messenger<Player, string>.AddListener ("turn.phase", ChangedPhase);
	}

	void OnDisable ()
	{
		Messenger<Player, string>.RemoveListener ("turn.phase", ChangedPhase);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Alpha1))
		{
			SelectedPiece = PieceType.ROUND;
			keyWasPressed = true;
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2))
		{
			SelectedPiece = PieceType.SQUARE;
			keyWasPressed = true;
		}
	}

	
	private float acknowledgeAlpha = 0.0f;


	private IEnumerator FlashAcknowledge (float aValue, float aTime)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
		{
			acknowledgeAlpha = Mathf.Lerp (aValue, 0.0f, t);
			yield return null;
		}
	}

	
	void OnGUI ()
	{
		if (IsPlacementPhase)
		{
			GUI.DrawTexture (new Rect (10, Screen.height - 74, 128, 64), PlacementHintImage);
			GUI.color = new Color (1.0f, 1.0f, 1.0f, acknowledgeAlpha);
			GUI.DrawTexture (new Rect (10, Screen.height - 74, 128, 64), PlacementFlash);
			GUI.color = Color.white;
			
			if (keyWasPressed)
			{
				keyWasPressed = false;
				StartCoroutine (FlashAcknowledge (0.7f, 0.5f));
			}
		}	
	}
	
	// turn.phase
	private void ChangedPhase (Player p, string name)
	{
		if (name.Equals ("Placement"))
		{
			IsPlacementPhase = true;
		}
		else
		{
			IsPlacementPhase = false;
		}
	}

}
