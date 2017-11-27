using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour {
	
	
	[SerializeField] private Text _highScoreText;
	private int _highscore;

	// Use this for initialization
	private IEnumerator Start () {
		_highscore = PlayerPrefs.GetInt("high", 0);
		_highScoreText.text = "BEST:" + _highscore.ToString("000000");
		//yield return new WaitForSeconds(0.1f);
		while (Input.touchCount == 0 || Input.touches[0].phase != TouchPhase.Began) yield return null;
		AsyncOperation scene = SceneManager.LoadSceneAsync("Main");
		yield return scene;
		scene.allowSceneActivation = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
