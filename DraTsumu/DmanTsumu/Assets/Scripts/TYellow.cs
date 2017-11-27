using UnityEngine;

public class TYellow : Tsumu 
{
    [SerializeField, Range(0, 4)] private int _amount = 2;
    
    public override void Affect(Emitter supporter , Emitter target, int power)
    {
        base.Affect(supporter, target, power);
        target.StartCoroutine(target.AmountUp(_amount));
    }
}
