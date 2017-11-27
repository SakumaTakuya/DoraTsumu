using UnityEngine;

public class TOrange : Tsumu
{
    public override void Affect(Emitter supporter,Emitter target, int power)
    {
        base.Affect(supporter ,target, power);
        target.StartCoroutine(target.AddColor());
    }
}
