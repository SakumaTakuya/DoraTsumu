using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private Player[] _player;
	[SerializeField] private Text _stateText;
	[SerializeField] private Text _timeText;
	
	[SerializeField] private TouchController _touchController;
	[SerializeField] private SceneController _sceneController;

	private Action _state;

    private float _time;
	private bool _isGameSet;
	
	// Use this for initialization
	private void Awake ()
	{
		_state = Ready;
	}
	
	// Update is called once per frame
	private void Update ()
	{
		_state();
	}

	private void Ready()
	{
		_stateText.text = "Ready";
		SetActiveAll(false);
		_sceneController.enabled = false;

		if (Input.touchCount < 4) return;
		{
			_state = Play;
			SetActiveAll(true);
		}
	}

	private void Play()
	{
		_time += Time.deltaTime;
		_stateText.text = "Play";
		_timeText.text = string.Format("{0:F1}s", _time);
	}

	private void GameSet()
	{
		_isGameSet = true;
		var winner = _player.First(p => !p.IsLosing).name;
		_stateText.text = winner + " win !";
		
		SetActiveAll(false);
		_sceneController.enabled = true;
	}

	private void JudgeGameSet(string loser)
	{
		if (_isGameSet) return;
		foreach (var player in _player)
		{
			if (!player.CompareTag(loser)) continue;
			player.IsLosing = true;
		}
		
		if(_player.Count(p => p.IsLosing) == _player.Length - 1) _state = GameSet;
	}

	private void SetActiveAll(bool active)
	{
		foreach (var player in _player)
		{
			player.gameObject.SetActive(active);
		}
		_touchController.enabled = active;
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		JudgeGameSet(other.tag);
	}
}
