using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	[SerializeField] private TsumuCollection _collection;
//	[SerializeField] private GameObject _waitStart;
	[SerializeField] private GameObject _gameOver;
	[SerializeField] private Text _highScoreText;
	[SerializeField] private Text _scoreResultText;
	[SerializeField] private GameObject _newRecord;

	private AudioSource _audioSource;
	
	private int _highscore;
	private const string HighscoreAdress = "high";

	// Use this for initialization
	private void Start ()
	{
		_audioSource = GetComponent<AudioSource>();
		_highscore = PlayerPrefs.GetInt(HighscoreAdress, 0);
		_highScoreText.text = "BEST:" + _highscore.ToString("000000");
		//_waitStart.SetActive(false);
		_audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void GameOver()
	{
		_collection.StopAllCoroutines();
		_gameOver.SetActive(true);
		_scoreResultText.text = "SCORE:" + _collection.Score;
		CompareHighScore(_collection.Score);
		StartCoroutine(WaitRetry());
	}

	private static IEnumerator WaitRetry()
	{
		yield return new WaitForSeconds(2);
		while (Input.touchCount == 0|| Input.touches[0].phase != TouchPhase.Began) yield return null;
		AsyncOperation scene = SceneManager.LoadSceneAsync("Start");
		yield return scene;
		scene.allowSceneActivation = true;
	}

	private void CompareHighScore(int score)
	{
		if (score > _highscore)
		{
			_newRecord.SetActive(true);
			PlayerPrefs.SetInt(HighscoreAdress, score);
		}
	}
	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Tsumu"))
		{
			GameOver();
			other.GetComponent<Rigidbody2D>().simulated = false;
		}
	}
}
