
using UnityEngine;
using System.Collections;
using SeeSharpMessenger;
using PushFightLogic;

/// <summary>
/// This behaviour monitors anchor related events and 
/// updates the position and visibility of the game object accordingly.
/// </summary>
public class AnchorHandler : MonoBehaviour
{
	private GameObject Sphere;
	
	// Use this for initialization
	void Start ()
	{
		Sphere = transform.FindChild ("Sphere").gameObject;
		
	}

	
	void OnEnable ()
	{
		Messenger.AddListener ("game.begin", OnGameBegin);
		Messenger<Coords>.AddListener ("piece.anchored", OnPieceAnchored);
		Messenger.AddListener ("piece.unanchored", OnPieceUnAnchored);
	}

		
	void OnDisable ()
	{
		Messenger.RemoveListener ("game.begin", OnGameBegin);
		Messenger<Coords>.RemoveListener ("piece.anchored", OnPieceAnchored);
		Messenger.RemoveListener ("piece.unanchored", OnPieceUnAnchored);	
	}
	
	void OnGameBegin ()
	{
		Sphere.renderer.enabled = false;
	}

	
	IEnumerator MoveLoop (Coords target, float apparateTime)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / apparateTime)
		{
			float alpha = Mathf.Lerp (1.0f, 0.0f, t);
			var c = Sphere.renderer.material.color;
			Sphere.renderer.material.color = new Color (c.r, c.g, c.b, alpha);
			yield return null;
		}
		
		transform.position = new Vector3 (target.x, 0, target.y);
		Sphere.renderer.enabled = true;
		
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / apparateTime)
		{
			float alpha = Mathf.Lerp (0.0f, 1.0f, t);
			var c = Sphere.renderer.material.color;
			Sphere.renderer.material.color = new Color (c.r, c.g, c.b, alpha);
			yield return null;
		}
	}

	
	IEnumerator FadeLoop (float apparateTime)
	{
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / apparateTime)
		{
			float alpha = Mathf.Lerp (1.0f, 0.0f, t);
			var c = Sphere.renderer.material.color;
			Sphere.renderer.material.color = new Color (c.r, c.g, c.b, alpha);
			yield return null;
		}
		
		Sphere.renderer.enabled = false;
	}

	
	void OnPieceAnchored (Coords location)
	{
		StartCoroutine (MoveLoop (location, 2.0f));
	}

	
	void OnPieceUnAnchored ()
	{
		StartCoroutine (FadeLoop (2.0f));
		Sphere.renderer.enabled = false;
	}
}
