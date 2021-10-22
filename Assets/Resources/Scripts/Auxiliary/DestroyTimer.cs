using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    [SerializeField] private float time;

    void Start() {
        Invoke("Ablamo", time);
    }

    private void Ablamo() {
        Destroy(gameObject);
    }
}
