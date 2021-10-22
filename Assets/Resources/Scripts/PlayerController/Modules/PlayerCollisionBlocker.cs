using UnityEngine;

public class PlayerCollisionBlocker : MonoBehaviour
{
    [System.NonSerialized] public string id;

    [SerializeField] private Collider myMainCollider;
    [SerializeField] private Collider myCollissionBlocker;

    void Start()
    {
        if (!myMainCollider)
            myMainCollider = transform.parent.GetComponent<Collider>();
        if (!myCollissionBlocker)
            myCollissionBlocker = GetComponent<Collider>();

        Physics.IgnoreCollision(myMainCollider, myCollissionBlocker, true);
    }
}
