using System;
using UnityEngine;

public class EmitterSetting : ScriptableObject
{
    public Space EmitSpace;
    [Range(5, 100), Tooltip("始めのツムの数")] public int InitialAmount = 5;
    [Range(0.01f,30),Tooltip("ツムを降らす間隔")]public float Interval = 1;
    [Range(0,30),Tooltip("一度に降らす量")]public int Amount = 1;
    [Range(0.01f,30),Tooltip("ツムの攻撃の効果時間")]public float Duration = 5;

    public Tsumu AdditionalTsumu;
    

    private void OnEnable()
    {
        name = "EmitterSetting";
    }
}