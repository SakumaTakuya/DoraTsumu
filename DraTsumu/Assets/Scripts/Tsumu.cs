using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tsumu : MonoBehaviour
{
	public int TargetId;//クラス自体の識別子 
	
	//public HashSet<Tsumu> Neighbors { get; private set; }
	public int Id;// { get; set; }
	public TsumuState State; //{ get; private set; }
	public TsumuCollection Collection;
	
	private SpriteRenderer _renderer;
	[SerializeField]private Rigidbody2D _rigidbody;
	
	// Use this for initialization
	private void OnEnable()
	{
		//Neighbors = new HashSet<Tsumu>();
		_renderer = GetComponent<SpriteRenderer>();
		_rigidbody = GetComponent<Rigidbody2D>();
	}

	public void ChangeState(TsumuState tsumuState)
	{
		switch (tsumuState)
		{
			case TsumuState.Normal:
				transform.localScale = Vector3.one;
				_rigidbody.constraints = RigidbodyConstraints2D.None;
				_renderer.color = Color.white;
				break;
			case TsumuState.Erasable:
				transform.localScale = 1.05f * Vector3.one;
				_rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
				_renderer.color = new Color(0, 0, 0, 0.5f);
				break;
			case TsumuState.Erase:
				transform.localScale = Vector3.one;
				_renderer.color = State == TsumuState.Erase ? Color.black : new Color(1, 1, 1, 0.5f);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
		State = tsumuState;
	}

	/*private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Tsumu"))
		{
			tag = "Tsumu";
		}
	}*/

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Tsumu"))
		{
			tag = "Tsumu";
		}
	}

	private void OnDisable()
	{
		tag = "Untagged";
	}
}

public enum TsumuState
{
	Normal,
	Erasable,
	Erase
}
