using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Emitter : MonoBehaviour
{
    public List<Tsumu> Target;    
    public int ReadyCount { get; set; }

    [SerializeField] private TsumuEraser _eraser;
    [SerializeField] private EmitterSetting _emitSetting;
    [SerializeField] private Image _gauge;

    private const int ReleaceCount = 5;

    private AudioSource[] _audio;
    private ParticleSystem _dropParticle;
    
    private void Awake()
    {
        _audio = GetComponents<AudioSource>();
        _dropParticle = GetComponent<ParticleSystem>();
        _emitSetting = Instantiate(_emitSetting);
        _emitSetting.EmitSpace += transform.position;
        StartCoroutine(ShowGauge());
    }

    private void Start()
    {
        StartCoroutine(IntervalEmit());
        StartCoroutine(Emit(_emitSetting.InitialAmount));
    }

    private IEnumerator ShowGauge()
    {
        var i = 0;
        const int nReleace = ReleaceCount * 10;
        
        while (true)
        {
            if (i < ReadyCount * 10) i++;
            else if (i > ReadyCount * 10) i--;
            
            if (ReadyCount == 0) i = 0;
            
            _gauge.fillAmount = (float)i / nReleace;
            yield return null;
        }   
        // ReSharper disable once FunctionNeverReturns
    }

    public IEnumerator AddReady(int count)
    {
        ReadyCount += count;
        yield return new WaitForSeconds(2);
        if(ReadyCount < ReleaceCount) yield break;
        StartCoroutine(Emit(ReadyCount));
        _audio[0].Play();
        _audio[1].Play();
        _dropParticle.Play();
        ReadyCount = 0;
    }

    public void SetOff(ref int count)
    {
        if (ReadyCount > count)
        {
            ReadyCount -= count;
            
        }
        else
        {
            ReadyCount = 0;
        }
    }

    public IEnumerator Emit(int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var index = Random.Range(0, Target.Count);

            Target[index].Eraser = _eraser;
            Instantiate(Target[index].gameObject, _emitSetting.EmitSpace.Random(), Quaternion.identity);
            yield return null;
        }
    }

    public IEnumerator SpeedUp(float acceleration)
    {
        _emitSetting.Interval *= acceleration;
         yield return new WaitForSeconds(_emitSetting.Duration);
        _emitSetting.Interval /= acceleration;
    }

    public IEnumerator AmountUp(int amount)
    {
        _emitSetting.Amount += amount;
        yield return new WaitForSeconds(_emitSetting.Duration);
        _emitSetting.Amount -= amount;
    }

    public IEnumerator AddColor()
    {
        if(Target.Contains(_emitSetting.AdditionalTsumu)) yield break;
        Target.Add(_emitSetting.AdditionalTsumu);
        yield return new WaitForSeconds(_emitSetting.Duration);
        Target.Remove(_emitSetting.AdditionalTsumu);
    }
    
    private IEnumerator IntervalEmit()
    {
        while (true)
        {
            yield return new WaitForSeconds(_emitSetting.Interval);
            StartCoroutine(Emit(_emitSetting.Amount));
        }
        // ReSharper disable once IteratorNeverReturns
    }
}
