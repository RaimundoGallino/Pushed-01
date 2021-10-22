using UnityEngine;

public class BombGroundCheck : MonoBehaviour
{
    private bool isGrounded = false;
    public bool IsGrounded => isGrounded;

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Ground")) {
            isGrounded = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Ground")) {
            isGrounded = false;
        }
    }
}
