using UnityEngine;
using System.Collections;
using UnityEngine.VFX;


[RequireComponent(typeof(Rigidbody))]
public class MapObjectBrain : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private int time = 4;
    [SerializeField] private VisualEffect dropEffect;
    [SerializeField] private ParticleSystem spreadEffect;

    private void Awake() {
        if (!meshCollider)
            meshCollider = GetComponent<MeshCollider>();
    }

    // Hacer que el objeto se caiga
    public void DropMe() {
        if (dropEffect)
            dropEffect.Play();
        if (spreadEffect)
            spreadEffect.Play();
        StartCoroutine(DropCoroutine());
    }

    private IEnumerator DropCoroutine()
    {
        yield return new WaitForSeconds(time);
        if (dropEffect)
            dropEffect.Stop();
        if (spreadEffect)
            spreadEffect.Stop();
        //Destroy(meshCollider);
        meshCollider.convex = true;
        Rigidbody my_rb = GetComponent<Rigidbody>();
        my_rb.useGravity = true;
        my_rb.isKinematic = false;
        my_rb.constraints = RigidbodyConstraints.None;
    }

    // Destruir objeto cuando ninguna camara le ve
    private void OnBecameInvisible() {

        GameEvents.PlayableMap.hasDestroyed?.Invoke();
        Destroy(gameObject);
    }
}
