using UnityEngine;

public class BombTouch : MonoBehaviour
{
    [System.NonSerialized] public string id;

    private void OnCollisionEnter(Collision other) {
        if (!other.collider.CompareTag("Player"))
            return;
        GameEvents.Bomb(id).instantExplodeSignal?.Invoke();
    }
}
