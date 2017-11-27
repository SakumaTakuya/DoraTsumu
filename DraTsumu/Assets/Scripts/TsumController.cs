using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TsumController : MonoBehaviour
{
	[SerializeField] private Camera _camera;
	[SerializeField] private TsumuCollection _collection;
	
	private int _touchId;
	private Tsumu _tsumu;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private void Update () {
		foreach (var touch in Input.touches)
		{
			if (_touchId == -1 || touch.fingerId == _touchId)
			{
				ControlTsumu(touch);
//				print(Input.touchCount);
			}
			
		}
		
	}

	private void ControlTsumu(Touch touch)
	{
		_tsumu = GetTsumuOn(touch.position);
		
		switch (touch.phase)
		{
			case TouchPhase.Began:
				if(!InScreen(touch.position)) return;//-1の時以外は既に追跡を開始している
				_touchId = touch.fingerId;//今回追いかけるタッチがどれか決める
				if(_tsumu)_collection.Search(_tsumu.TargetId, _tsumu.Id);
				break;
			case TouchPhase.Moved:	
				//if(touch.fingerId != _touchId) return;
				if(_tsumu) _collection.AddErase(_tsumu);
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				//if (touch.fingerId != _touchId || !isActiveAndEnabled) return;
//				print("cancel");
				StartCoroutine(_collection.Release());
				_touchId = -1;
				_tsumu = null;
				break;
			case TouchPhase.Stationary:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

	}
	
	private Tsumu GetTsumuOn(Vector2 position)
	{
		var tapPoint = _camera.ScreenToWorldPoint(position);
		var coll2D = Physics2D.OverlapPoint(tapPoint);
		if (!coll2D) return _tsumu;
		var hit = Physics2D.Raycast(tapPoint,-Vector2.up);
//		print(hit.collider.gameObject);
		return !hit ? _tsumu : hit.collider.GetComponent<Tsumu>();
	}

	private bool InScreen(Vector2 touchPos)
	{
		var pos = _camera.pixelRect;
		return touchPos.x >= pos.x && 
		       touchPos.x <= pos.x + pos.width && 
		       touchPos.y >= pos.y && 
		       touchPos.y <= pos.y + pos.height;
	}

}
