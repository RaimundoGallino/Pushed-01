[System.Serializable]
public class Stun : BaseEffect
{
    public Stun(EffectScriptableObject effectData) : base(effectData) {
    }

    public void Behaviour(PlayerMovement target) {
        target.OnStun(duration);
    }
}
