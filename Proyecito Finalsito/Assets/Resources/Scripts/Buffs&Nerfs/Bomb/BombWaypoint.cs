using UnityEngine;

public class BombWaypoint : MonoBehaviour
{
    public bool isGrounded = false;

    private void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }
}
