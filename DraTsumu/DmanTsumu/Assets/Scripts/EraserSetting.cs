using UnityEngine;

[CreateAssetMenu(menuName = "Eraser Setting")]
public class EraserSetting : ScriptableObject {
    
    [Tooltip("ツムを消したときのエフェクト")] public GameObject EraseParticle;
    [Range(0,10),Tooltip("最小で何個のツムを繋げればよいか")] public int MinCount = 3;
    [Range(0.01f,2),Tooltip("攻撃のレート")] public float Rate = 0.5f;
    public AudioClip Atsumori1;
    public AudioClip Atsumori2;
}
