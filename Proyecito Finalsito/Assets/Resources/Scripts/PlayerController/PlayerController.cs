using System;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // Main Player class
    [System.NonSerialized] public string id;
    private bool hasExtraLife = false;

    private void Awake()
    {
        if (this.GetType().IsSubclassOf(typeof(PlayerController)))
            return;

        id = Guid.NewGuid().ToString();
        GameEvents.ClaimPlayerEvents(id);
        
        var allMyComponents = new dynamic[] {
            GetComponentsInChildren<PlayerAnimation>(),
            GetComponentsInChildren<PlayerDamageHandler>(),
            GetComponentsInChildren<PlayerMovement>(),
            GetComponentsInChildren<PlayerPush>(),
            GetComponentsInChildren<PlayerPushCollider>(),
            GetComponentsInChildren<PlayerGroundCheck>(),
            GetComponentsInChildren<PlayerPushAimBot>(),
            GetComponentsInChildren<PlayerRigidBodyModifier>(),
            GetComponentsInChildren<PlayerCollisionBlocker>(),
            GetComponentsInChildren<PlayerSound>(),
        };

        // Set all ids 
        foreach (var elem in allMyComponents) {
            foreach (var component in elem) {
                component.id = this.id;
                component.enabled = true;
            }
        }
        GameEvents.Player(id).onExtraLife += ExtraLifeCollected;
        GameEvents.Player(id).getExtraLife += HasExtraLife;
        GameEvents.Player(id).onLastLife += LastLife;
    }

    public virtual void OnEnable()
    {
        if (id != null)
            SetMyEvents();
    }

    public virtual void SetMyEvents()
    {
        GameEvents.Player(id).onExtraLife += ExtraLifeCollected;
        GameEvents.Player(id).getExtraLife += HasExtraLife;
        GameEvents.Player(id).onLastLife += LastLife;
    }

    private void ExtraLifeCollected() => hasExtraLife = true;

    private bool HasExtraLife() => hasExtraLife;

    private void LastLife() => hasExtraLife = false;
    private void OnDisable()
    {
        GameEvents.Player(id).onExtraLife -= ExtraLifeCollected;
        GameEvents.Player(id).getExtraLife -= HasExtraLife;
        GameEvents.Player(id).onLastLife -= LastLife;

    }
}
