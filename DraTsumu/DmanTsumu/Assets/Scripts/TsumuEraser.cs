using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TsumuEraser : MonoBehaviour
{

	[SerializeField] private EraserSetting _setting;
	[SerializeField] private Text _comboText;
	[SerializeField] private Vector3 _gauge;
	[SerializeField] private GameObject _particle;

	private AudioSource _comboAudio;
	private AudioSource _atsumoriAudio;
	private Coroutine _now;
	
	private readonly Stack<Tsumu> _eraseSet = new Stack<Tsumu>();
	private readonly HashSet<Tsumu> _erasableSet = new HashSet<Tsumu>();
	
	private bool _isSearchingErasable;
	private int _combo;

	private void Awake()
	{
		_comboAudio = GetComponents<AudioSource>()[0];
		_atsumoriAudio = GetComponents<AudioSource>()[1];
	}

	public void SearchErasable(Tsumu tsumu)
	{
		if(_isSearchingErasable || _erasableSet.Count > 0 && _erasableSet.First().GetType() != tsumu.GetType()) return;
		_isSearchingErasable = true;
		SearchToErase(tsumu);
		_isSearchingErasable = false;
	}

	public void SearchToErase(Tsumu tsumu)
	{
		_erasableSet.Add(tsumu);
		if (tsumu.State != TsumuState.Normal) return;
		
		tsumu.Renderer.color = Color.gray;
		tsumu.State = TsumuState.Searched;
		
		var　array = tsumu.Neighbors.Where(t => t.State != TsumuState.Searched).ToArray();
		foreach (var t in array)
		{
			SearchToErase(t);
		}
	}

	public void SearchDiserasable(Tsumu tsumu)
	{
		if(HaveErase(tsumu)) return;
		SearchToNormal(tsumu);
	}

	private static void SearchToNormal(Tsumu tsumu)
	{
		if(tsumu.State != TsumuState.Searched) return;
		
		tsumu.State = TsumuState.Normal;
		tsumu.Renderer.color = Color.white;
		
		var　array = tsumu.Neighbors.Where(t => t.State == TsumuState.Searched).ToArray();
		foreach (var t in array)
		{
			SearchToNormal(t);
		}
	}

	private static bool HaveErase(Tsumu tsumu)
	{
		return GetErase(tsumu, tsumu.Neighbors, new HashSet<Tsumu>{tsumu});
	}

	/*private void GetNeihnors(Tsumu tsumu, ISet<Tsumu> set)
	{
		foreach (var t in tsumu.Neighbors.Where(t => !set.Contains(t)))
		{
			set.Add(t);
			GetNeihnors(t, set);
		}
	}*/

	private static Tsumu GetErase(Tsumu tsumu, IEnumerable<Tsumu> set, ICollection<Tsumu> searched)
	{
		
		Tsumu erase = null;
		foreach (var t in set)
		{
			searched.Add(t);
			if (t.State == TsumuState.Erase)
			{
				erase = t;
				break;
			}
			erase = GetErase(t, t.Neighbors.Where(ts => !searched.Contains(ts)), searched);
		}
		return erase;
	}

	public void AddEraseList(Tsumu tsumu)
	{
		if(tsumu.State != TsumuState.Searched) return;
		if (_eraseSet.Count > 0)
		{
			var peek = _eraseSet.Peek();
			if (!peek.Neighbors.Contains(tsumu)) return; //前のツムに隣接していないものは消せない
			if (tsumu != peek) peek.Renderer.color = new Color(0, 0, 0, 1); //先ほどまで触っていたツムの色を暗くする
		}
		
		tsumu.State = TsumuState.Erase;
		tsumu.Renderer.color = new Color(0, 0, 0, 0.5f);
		_eraseSet.Push(tsumu);
		
	}

	public IEnumerator Attack(Emitter supporter ,Emitter target)
	{
		Undo();
		if (_eraseSet.Count < _setting.MinCount)
		{
			_eraseSet.Clear();
			yield break;
		}
	
		if(_now != null) StopCoroutine(_now);
		_comboText.color = new Color(1,1,1,0.390625f);
		var array = _eraseSet.ToArray();
		foreach (var tsumu in array)
		{
			if(!tsumu) continue;
			Instantiate(_setting.EraseParticle, tsumu.transform.position, Quaternion.identity);
			_comboAudio.pitch *= 1.2f;
			_comboAudio.Play();
			//Destroy(tsumu.gameObject);
			StartCoroutine(Charge(tsumu.transform));
			_comboText.text = (++_combo).ToString();
			yield return new WaitForSeconds(0.1f);
		}
		
		array[0].Affect(supporter ,target, Power(_eraseSet.Count));
		_now = StartCoroutine(EndAttack());
	}

	private IEnumerator EndAttack()
	{
		_comboAudio.pitch = 1;
		var atsumori = _combo > 14 ? _setting.Atsumori2 : _setting.Atsumori1;
		_atsumoriAudio.PlayOneShot(atsumori);
		_eraseSet.Clear();

		var time = 0.0f;
		while (time < 1.5f)
		{
			_comboText.color -= new Color(0,0,0,0.005f);
			yield return null;
			time += Time.deltaTime;
		}
		_comboText.text = "0";
		_combo = 0;
		_comboText.color = new Color(1,1,1,0.390625f);
	}

	public void Undo()
	{
		foreach (var tsumu in _erasableSet)
		{
			if(!tsumu) continue;
			tsumu.State = TsumuState.Normal;
			tsumu.Renderer.color = Color.white;
		}
		_erasableSet.Clear();
	}

	private int Power(int count)
	{
		var bonus = 8;
		if      (count < 4 ) bonus = 0;
		else if (count < 6 ) bonus = 1;
		else if (count < 8 ) bonus = 3;
		else if (count < 10) bonus = 5;
		return (int)((count + bonus + _combo * 0.2f) * _setting.Rate);
	}

	private IEnumerator Charge(Transform target)
	{
		Destroy(target.gameObject);
		var tsu = Instantiate(_particle, target.position, target.rotation);
		while (Vector3.Distance(tsu.transform.position, _gauge) > 1)
		{
			tsu.transform.position = Vector3.Lerp(tsu.transform.position, _gauge, Time.deltaTime * 3);
			yield return null;
		}
		Destroy(tsu);
	}
	
}

//

//private Tsumu _touchedTsumu;
	
/**/

/*public void ToErase(Tsumu tsumu)
{
	if(tsumu.State != TsumuState.Erasable || _touchedTsumu == tsumu ) return;
	if (_touchedTsumu)
	{
		_touchedTsumu.Renderer.color = Color.black;
		if(!_touchedTsumu.Neighbors.Contains(tsumu)) return;
	}
	
	tsumu.State = TsumuState.Erase;
	tsumu.Renderer.color = new Color(1,1,1,0.5f);
	
	_touchedTsumu = tsumu;
}

public IEnumerator Attack(Emitter target)
{
	var tmp = _erasableSet.Where(t => t.State == TsumuState.Erase);
	var eraseSet = tmp as Tsumu[] ?? tmp.ToArray();
	
	Undo();
	if(eraseSet.Length < _minCount) yield break;
	eraseSet[0].Affect(target, Power(eraseSet.Length));
	foreach (var tsumu in eraseSet)
	{
		Instantiate(_eraseParticle, tsumu.transform.position, Quaternion.identity);
		Destroy(tsumu.gameObject);
		yield return new WaitForSeconds(0.1f);
	}	
}

private void Undo()
{
	foreach (var tsumu in _erasableSet)
	{
		if(!tsumu) continue;
		tsumu.Renderer.color = Color.white;
		tsumu.State = TsumuState.Nomal;
	}
	_erasableSet.Clear();
}*/
