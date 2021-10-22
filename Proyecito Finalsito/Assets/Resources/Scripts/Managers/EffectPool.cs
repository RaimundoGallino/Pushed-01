
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    private static EffectPool current;

    [SerializeField] private EffectScriptableObject StunScriptableObject;
    private Stun stun;
    public static Stun Stun => current.stun;

    private void Awake()
    {
        current = this;

        if (!StunScriptableObject)
            StunScriptableObject = (EffectScriptableObject) Resources.Load("Effects/Stun");
        stun = new Stun(StunScriptableObject);
    }
}
