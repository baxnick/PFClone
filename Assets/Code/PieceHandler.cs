
using UnityEngine;
using System.Collections;
using PushFightLogic;
using SeeSharpMessenger;
using System.Collections.Generic;

/// <summary>
/// Monitors events relating to the piece it represents and animates accordingly.
/// </summary>
public class PieceHandler : MonoBehaviour
{
	public int ID;
	private Quaternion StartingOrientation;
	
	// Use this for initialization
	void Start ()
	{
		StartCoroutine (AnimationLoop (1.0f, 0.3f));
	}
	
	void OnEnable ()
	{
		
		Messenger<GameToken,Coords>.AddListener ("piece.moving", MoveEvent);
		Messenger<GameToken,Coords>.AddListener ("piece.displacing", DisplaceEvent);
	}

	
	void OnDisable ()
	{
		Messenger<GameToken,Coords>.RemoveListener ("piece.moving", MoveEvent);
		Messenger<GameToken,Coords>.RemoveListener ("piece.displacing", DisplaceEvent);
	}

	
	private Queue<Coords> animationQueue = new Queue<Coords> ();


	private IEnumerator AnimationLoop (float animTime, float snapTime)
	{
		while (true)
		{
			while (animationQueue.Count == 0)
			{
				yield return null;
			}
			
			Coords target = animationQueue.Dequeue ();
			Vector3 originTransform = transform.parent.position;
			Vector3 targetTransform = new Vector3 (target.x, 0, target.y);
			
			transform.GetComponent<Rigidbody> ().isKinematic = false;
			transform.rigidbody.AddForce (Vector3.up * 2.0f, ForceMode.Impulse);
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / animTime)
			{
				transform.rigidbody.AddForce (Vector3.up * 1.0f);
				transform.parent.position = Vector3.Slerp (originTransform, targetTransform, t);
				yield return null;	
			}
			
			if (Game.Master ().Winner () == null)
			{
				transform.GetComponent<Rigidbody> ().isKinematic = true;
				
				yield return StartCoroutine(SnapBack(snapTime));
			}
		}
	}

	
	private IEnumerator DropperLoop (float fallTime, float animTime)
	{
		transform.localPosition = new Vector3 (0.0f, 3.0f, 0.0f);
		transform.GetComponent<Rigidbody> ().isKinematic = false;
		rigidbody.AddTorque (Vector3.up * Random.Range (-5.0f, 5.0f), ForceMode.Impulse);
		rigidbody.AddTorque (Vector3.left * Random.Range (-5.0f, 5.0f), ForceMode.Impulse);
		rigidbody.AddTorque (Vector3.forward * Random.Range (-5.0f, 5.0f), ForceMode.Impulse);
		
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fallTime)
		{
			yield return null;	
		}
		
		transform.GetComponent<Rigidbody> ().isKinematic = true;
		yield return StartCoroutine(SnapBack(animTime));
	}

	
	private IEnumerator SnapBack (float time)
	{
		Vector3 originTransform = transform.localPosition;
		Vector3 targetTransform = new Vector3 (0f, 0f, 0f);
		
		Quaternion originRotation = transform.localRotation;
		
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time)
		{
			transform.localPosition = Vector3.Slerp (originTransform, targetTransform, t);
			transform.localRotation = Quaternion.Slerp (originRotation, StartingOrientation, t);
			yield return null;	
		}
		
		transform.localPosition = targetTransform;
		transform.localRotation = StartingOrientation;
	}

	
	public void OnSpawned ()
	{
		StartingOrientation = transform.localRotation;
		StartCoroutine (DropperLoop (1.0f, 0.5f));
	}

	
	private void MoveEvent (GameToken origin, Coords toLoc)
	{
		if (origin.id != ID)
		{
			return;
		}
		
		animationQueue.Enqueue (toLoc);
	}

	
	private void DisplaceEvent (GameToken origin, Coords toLoc)
	{
		if (origin.id != ID)
		{
			return;
		}
		
		animationQueue.Enqueue (toLoc);
	}
}
