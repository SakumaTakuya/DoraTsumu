using System;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
	public bool IsLosing { get; set; }
	
	[SerializeField] private Camera _camera;
	[SerializeField] private Emitter _supportEmitter;
	[SerializeField] private Emitter _opponentEmitter;
	[SerializeField] private Image _texture;

	private int _touchId = -1;
	private Tsumu _tsumu;

	public void ControlTsumuby(Touch touch)
	{
		_tsumu = GetTsumuOn(touch.position);
		if(!_tsumu) return;

		switch (touch.phase)
		{
			case TouchPhase.Began:
				if(_touchId != -1 || !InScreen(touch.position)) return;//-1の時以外は既に追跡を開始している
				_touchId = touch.fingerId;//今回追いかけるタッチがどれか決める
				_tsumu.Eraser.SearchErasable(_tsumu);
				_tsumu.Eraser.AddEraseList(_tsumu);
				break;
			case TouchPhase.Moved:	
				if(touch.fingerId != _touchId) return;
				_tsumu.Eraser.AddEraseList(_tsumu);
				break;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				if (touch.fingerId != _touchId || !isActiveAndEnabled) return;
				StartCoroutine(_tsumu.Eraser.Attack(_supportEmitter, _opponentEmitter));
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

	private void OnTriggerEnter2D(Collider2D other)
	{
		other.tag = tag;
	}

	private void OnDisable()
	{
		if(IsLosing) _texture.color = new Color(0,0,0,0.5f);
	}
}
