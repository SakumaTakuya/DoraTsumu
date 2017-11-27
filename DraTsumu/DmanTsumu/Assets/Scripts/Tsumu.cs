using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tsumu : MonoBehaviour
{
	public HashSet<Tsumu> Neighbors { get; private set; }
	public SpriteRenderer Renderer { get; private set; }
	public TsumuState State { get; set; }
	public TsumuEraser Eraser; // { get; set; }にするとなぜかうまくいかない

	// Use this for initialization
	private void Start()
	{
		Neighbors = new HashSet<Tsumu>();
		Renderer = GetComponent<SpriteRenderer>();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		var tsumu = other.gameObject.GetComponent<Tsumu>();
		if (!tsumu || Neighbors.Contains(tsumu) || GetType() != tsumu.GetType()) return;
		Neighbors.Add(tsumu);
		if (State != TsumuState.Searched && State != TsumuState.Erase) return;
		Eraser.SearchErasable(tsumu);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		var tsumu = other.gameObject.GetComponent<Tsumu>();
		if (!tsumu || !Neighbors.Contains(tsumu) || GetType() != tsumu.GetType()) return;
		StartCoroutine(Check(tsumu));
	}

	private IEnumerator Check(Tsumu tsumu)
	{
		while (tsumu && tsumu.State != State) yield return null;
		Neighbors.Remove(tsumu);
	}

private void OnCollisionEnter2D(Collision2D other)
	{
		if (CompareTag("Untagged")) tag = other.collider.tag;
	}

	public virtual void Affect(Emitter supporter,Emitter target, int power)
	{
		supporter.SetOff(ref power);
		target.StartCoroutine(target.AddReady(power));
	}
}

public enum TsumuState
{
	Normal,
	Searched,
	Erase
}