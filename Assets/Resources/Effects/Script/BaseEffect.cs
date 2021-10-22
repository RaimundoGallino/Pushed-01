using UnityEngine;

// <summary>
// Base class for any effect
// </summary>
public class BaseEffect
{
    public BaseEffect(EffectScriptableObject effectData)
    {
        duration = effectData.duration;
    }

    public float duration;
}
