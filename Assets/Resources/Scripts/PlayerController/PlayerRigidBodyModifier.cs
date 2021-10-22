using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRigidBodyModifier : MonoBehaviour
{
    private Rigidbody rb;

    [System.NonSerialized] public string id;

    [SerializeField] private float outOfPlatformDrag = 0.44f;
    private float defaultDrag;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        defaultDrag = rb.drag;
    }

    private void OnEnable() {
        GameEvents.Player(id).Damage.onFreeze += OnFreeze;
        GameEvents.Player(id).onGrounded += OnGrounded;
        GameEvents.Player(id).onNotGrounded += OnNotGrounded;
    }

    private void OnDisable() {
        GameEvents.Player(id).Damage.onFreeze -= OnFreeze;
        GameEvents.Player(id).onGrounded -= OnGrounded;
        GameEvents.Player(id).onNotGrounded -= OnNotGrounded;
    }

    private void OnFreeze(float freezeTime)
    {
        SetDrag(0);
        Invoke("ResetDrag", freezeTime);
    }

    public void ResetDrag()
    {
        rb.drag = defaultDrag;
    }

    public void SetDrag(float value)
    {
        rb.drag = value;
    }

    private void OnGrounded()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.drag = defaultDrag;
    }

    private void OnNotGrounded()
    {
        rb.drag = outOfPlatformDrag;
        rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezeRotation;
    }

    public void GroundConstraints()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    public void NonGroundConstraints()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
