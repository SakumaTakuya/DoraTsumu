using UnityEngine;

public class TBlue : Tsumu
{
    [SerializeField,Range(0,1)] private float _acceleration = 1;
    
    public override void Affect(Emitter supporter ,Emitter target, int power)
    {
        base.Affect(supporter, target, power);
        target.StartCoroutine(target.SpeedUp(_acceleration));
    }
}
