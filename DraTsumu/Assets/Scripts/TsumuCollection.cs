using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TsumuCollection : MonoBehaviour
{
	private const int Amount = 40;
	
	public Tsumu[,] Pool { get; private set; }
	public int Count;
	public int Combo;
	public int Score;
	public int Level;
	
	[SerializeField] private Tsumu[] _tsumus;//今回使用するツムの種類
	[SerializeField] private int _minCount = 3;
	[SerializeField] private float _comboTime = 1;
	[SerializeField] private float _scoreRate;
	[SerializeField] private int _maxLevel = 99;
	[SerializeField,Range(0f,1f)] private float _levelRate = 0.1f;
	[SerializeField,Range(0f,1f)] private float _fallRate = 0.03f;

	[SerializeField] private Text _comboText;
	[SerializeField] private Text _scoreText;
	[SerializeField] private Text _levelText;
	
	private byte[,,] _adjacencyMatrix;//target, v1, v2
	private Queue<int>[] _nextActive;//現在非アクティブで次にアクティブになるべきツム

	private AudioSource _audio;
	private Action<TsumuState> _release = t => { };
	private readonly Stack<Tsumu> _eraseStack = new Stack<Tsumu>();
	private Tsumu _touchedTsumu;
	
	// Use this for initialization
	private IEnumerator Start ()
	{
		_audio = GetComponent<AudioSource>();
		Pool = new Tsumu[_tsumus.Length, Amount];
		_adjacencyMatrix = new byte[_tsumus.Length, Amount, Amount];
		_nextActive = new Queue<int>[_tsumus.Length];
		
		for (int i = 0; i < _tsumus.Length; i++)
		{
			_tsumus[i].TargetId = i;
			_nextActive[i] = new Queue<int>();
			for (int j = 0; j < Amount; j++)
			{
				Tsumu t = Instantiate(_tsumus[i]);
				t.gameObject.SetActive(false);
				Pool[i, j] = t;
				t.Id = j;
				t.Collection = this;
				_nextActive[i].Enqueue(j);
			}
		}
		
		
		while (true)
		{
			int r = (int)(Mathf.Sqrt((float)Level / _maxLevel) * 3 + 2);
			int i = Random.Range(0, Mathf.Clamp(r, 2, _tsumus.Length));
			Create(i , new Vector2(Random.Range(-3f,3f), Random.Range(12f,20f)));
//			print((int)(Mathf.Sqrt((float)Level / _maxLevel) * 3 + 2) + "," + (_fallRate * Count / (Level + 1) + r * 0.1f));
			yield return new WaitForSeconds(_fallRate * Count / (Level + 1) + r * 0.05f);
		}
	}

	public Tsumu Create(int target,Vector2 position)
	{
		if (_nextActive[target].Count == 0) return null;
		Tsumu t = Pool[target, _nextActive[target].Dequeue()];
		t.gameObject.SetActive(true);
		t.transform.position = position;
		t.tag = "Untagged";//一時的に記述
		Count++;
		return t;
	}

	public void Delete(int target, int id)
	{		
		Pool[target, id].gameObject.SetActive(false);
		_nextActive[target].Enqueue(id);
		Count--;
	}

	public void SetNeighbor(int target, int v1,int v2)
	{
		_adjacencyMatrix[target, v1, v2] = 1;
	}

	public void ReleaseNeighbor(int target, int v1,int v2)
	{
		_adjacencyMatrix[target, v1, v2] = 0;
	}
	
	public void Search(int target, int id)
	{	
		Stack<int> dfsStack = new Stack<int>();//深さ優先探索用のスタック
		dfsStack.Push(id);
		Pool[target, id].ChangeState(TsumuState.Erasable);
		_release += Pool[target, id].ChangeState;
		AddErase(Pool[target, id]);
		DpsSearch(dfsStack, target);
	}

	private void DpsSearch(Stack<int> stack, int target)
	{
		while (stack.Count != 0)
		{
			int id = stack.Peek();
			for (int i = 0; i < Amount; i++)
			{
				if (_adjacencyMatrix[target, id, i] != 1 || Pool[target, i].State != TsumuState.Normal) continue;
				stack.Push(i);
				Pool[target, i].ChangeState(TsumuState.Erasable);
				_release += Pool[target, i].ChangeState;//離したときに実行する
				DpsSearch(stack, target);
				return;
			}
			stack.Pop();
		}
	}

	public void AddErase(Tsumu tsumu)
	{
		if(tsumu.State != TsumuState.Erasable) return;
		if (_touchedTsumu)
		{
			if(_adjacencyMatrix[tsumu.TargetId, tsumu.Id, _touchedTsumu.Id] != 1) return;
			_touchedTsumu.ChangeState(TsumuState.Erase);
		}
		tsumu.ChangeState(TsumuState.Erase);
		_touchedTsumu = tsumu;
		_eraseStack.Push(tsumu);
	}
	
	public IEnumerator Release()
	{
		_release(TsumuState.Normal);
		_touchedTsumu = null;

		//Stack<Tsumu> stack = new Stack<Tsumu>(_eraseStack);
		Tsumu[] array = _eraseStack.ToArray();
		int len = array.Length;
		_eraseStack.Clear();
		if(array.Length < _minCount) yield break;
		for (int i = 0; i < len; i++)
		{
			Tsumu t = array[i];
			Delete(t.TargetId,t.Id);
			CalculateValue(len);
			_audio.pitch = Combo * 0.05f + 1;
			_audio.Play();
			yield return new WaitForSeconds(0.075f);
		}
		//if (stack.Count < _minCount) yield break;
		
		/*while (stack.Count != 0)
		{
			Tsumu t = stack.Pop();
			Delete(t.TargetId,t.Id);
			CalculateValue(++ren);
			_audio.pitch = Combo * 0.05f + 1;
			_audio.Play();
			yield return new WaitForSeconds(0.075f);
		}*/
		//
		StartCoroutine(ComboObserve());
	}

	private void CalculateValue(int ren)
	{
		_comboText.text = Mathf.Clamp(++Combo,0,99).ToString("00");
		
		Score += (int) ((Combo / 10f + ren) * _scoreRate);
		_scoreText.text = "SCORE:" + Mathf.Clamp(Score,0,999999).ToString("000000");

		Level = Mathf.Clamp((int)Mathf.Sqrt(Score * _levelRate), 0, _maxLevel);
		_levelText.text = Level.ToString();
	}

	private IEnumerator ComboObserve()
	{
		float time = 0f;
		int cmb = Combo;
		Color color = _comboText.color;
		while (time < _comboTime)
		{
			if (Combo > cmb)
			{
				_comboText.color = color;
				yield break;
			}
			
			_comboText.color = Color.Lerp(color, Color.clear, time / _comboTime);
			
			yield return null;
			time += Time.deltaTime;
		}
		_comboText.color = color;
		Combo = 0;
		_comboText.text = 0.ToString("00");
	}
}

/*Create
Tsumu t = Pool[target][Count[target]];
t.gameObject.SetActive(true);
t.transform.position = position;
t.ID = Count[target]++;*/

/*Delete
Pool[target, id].gameObject.SetActive(false);
Tsumu tmp = Pool[target, id];
Pool[target,id] = Pool[target,Count[target]];
Pool[target,id].ID = Pool[target,Count[target]].ID;
Pool[target,Count[target]] = tmp;
Pool[target,Count[target]--].ID = tmp.ID;*/
