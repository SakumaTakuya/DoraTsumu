using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class TsumuCollider : MonoBehaviour
{

	private Tsumu _parent;

	// Use this for initialization
	private void OnEnable ()
	{
		_parent = transform.parent.GetComponent<Tsumu>();
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		var tsumu = other.gameObject.GetComponent<Tsumu>();
		if (!tsumu || _parent.GetType() != tsumu.GetType()) return;
		_parent.Collection.SetNeighbor(_parent.TargetId, _parent.Id, tsumu.Id);
		if(tsumu.State != TsumuState.Normal && _parent.State == TsumuState.Normal) _parent.Collection.Search(_parent.TargetId, tsumu.Id);
		//Neighbors.Add(tsumu);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var tsumu = other.gameObject.GetComponent<Tsumu>();
		if (!tsumu || _parent.GetType() != tsumu.GetType()) return;
		_parent.Collection.ReleaseNeighbor(_parent.TargetId, _parent.Id, tsumu.Id);
		//Neighbors.Remove(tsumu);
	}
}
