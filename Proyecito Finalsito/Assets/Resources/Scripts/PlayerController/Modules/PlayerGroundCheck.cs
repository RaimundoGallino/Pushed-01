using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [System.NonSerialized] public string id;

    [SerializeField] private bool isGrounded = false;
    public bool IsGrounded => isGrounded;

    // ================== GROUND CHECK =======================
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ground")) {
            GameEvents.Player(id).onGrounded?.Invoke();
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground")) {
            GameEvents.Player(id).onNotGrounded?.Invoke();
            isGrounded = false;
        }
    }
}