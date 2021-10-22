using UnityEngine;

public class PlayerPushAimBot : MonoBehaviour
{
    [System.NonSerialized] public string id;
    [SerializeField] private MeshCollider myCollider;

    [Header("Parent Info")]
    [SerializeField] private GameObject parent;
    private GameObject targetedPlayer;
    private GameObject GetGameObjectOfTarget() => targetedPlayer;

    private Vector3 starterScale;

    private void Awake()
    {
        if (!myCollider)
            myCollider = GetComponent<MeshCollider>();

        if (!parent)
            parent = transform.parent.transform.parent.gameObject;

        starterScale = transform.localScale;
    }

    private void OnEnable()
    {
        // Getter event
        GameEvents.Player(id).AimBot.GetGameObjectOfTarget += GetGameObjectOfTarget;
    }

    private void OnDisable()
    {
        // Getter event
        GameEvents.Player(id).AimBot.GetGameObjectOfTarget -= GetGameObjectOfTarget;
    }


    /// <summary> Escala el detector dependiendo de que tan cansado nos encontremos </summary>
    public void ScaleDetector(int pushStagesCount, int currentIndex)
    {
        if (currentIndex == -1)
            return;

        // // Esto me va a evitar dividir por indices, sino por el numero de elemento, ejemplo: el elemento 1 seria index 0, ¿Dividirias entre 0 querido camarada?
        // var scaleDivisor = currentIndex + 1;

        // var newScale = starterScale;
        // newScale.x /= scaleDivisor;
        // newScale.z /= scaleDivisor;

        // transform.localScale = newScale;
    }



    /* =========================== DETECTION ================================ */
    private void OnTriggerEnter(Collider other)
    {
        // Break cases
        if (!other.CompareTag("Player") || other.gameObject == gameObject || other.gameObject == parent || targetedPlayer)
            return;

        // Sos pollo
        targetedPlayer = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        // Break cases
        if (!other.CompareTag("Player") || !targetedPlayer)
            return;

        // Si el jugador target salio del rango
        if (other.gameObject == targetedPlayer)
            targetedPlayer = null;
    }
}
