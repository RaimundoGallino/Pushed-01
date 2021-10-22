using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static DreadZitoTools.Utils;

public class PlayerSound : MonoBehaviour
{
    [System.NonSerialized] public string id;

    private void OnEnable() {
        GameEvents.Player(id).Sound.footSteps += RandomFootStep;
        GameEvents.Player(id).Sound.push += RandomPushSound;
        GameEvents.Player(id).Push.OnImpactedOne += ImpactedOne;
        GameEvents.Player(id).Damage.onPushRecieved += SomeonePushedMe;
    }

    private void OnDisable() {
        GameEvents.Player(id).Sound.footSteps -= RandomFootStep;
        GameEvents.Player(id).Sound.push -= RandomPushSound;
        GameEvents.Player(id).Push.OnImpactedOne -= ImpactedOne;
        GameEvents.Player(id).Damage.onPushRecieved -= SomeonePushedMe;
    }

    public void RandomFootStep()
    {
        var currentSceneName = SceneManager.GetActiveScene().name;

        AudioManager.current.PlayRandomClipAtPosition("Player", "FootstepsMetal", transform.position);
    }

    public void RandomPushSound()
    {
        AudioManager.current.PlayRandomClipAtPosition("Player", "Push", transform.position);
    }

    private void ImpactedOne()
    {
        AudioManager.current.PlayRandomClipAtPosition("Player", "Hit", transform.position);
    }

    private void SomeonePushedMe()
    {
        AudioManager.current.PlayRandomClipAtPosition("Player", "Auch", transform.position);
    }

    public void VocalTrigger(InputAction.CallbackContext cntx)
    {
        // This string will contain (Clone) if is a prefab intatiation
        var playerNamePostFix = gameObject.name.GetLast(7);
        if (!cntx.performed || playerNamePostFix != "(Clone)")
            return;

        AudioManager.current.PlayRandomClipAtPosition("Player", "Vocal", transform.position);
    }
}
