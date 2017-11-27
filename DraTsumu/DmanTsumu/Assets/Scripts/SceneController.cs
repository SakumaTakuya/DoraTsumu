using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{

	[SerializeField] private int _index = 0;
	[SerializeField] private Button _button;

	private void OnEnable()
	{
		
		if(!_button) return;
		_button.gameObject.SetActive(true);
		_button.onClick.AddListener(ChangeScene);
	}

	private void OnDisable()
	{
		if(!_button) return;
		_button.gameObject.SetActive(false);
	}

	public void ChangeScene()
	{
		StartCoroutine(Change());
	}

	private IEnumerator Change()
	{
		SceneManager.LoadScene(_index);
		yield return null;
	}
}
