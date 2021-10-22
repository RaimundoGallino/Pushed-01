using System.Collections.Generic;
using UnityEngine;


public class PlayerPushCollider : MonoBehaviour
{
    [System.NonSerialized] public string id;

    // Lista de objetos impactados a la hora de empujar. Permite no empujar 2 veces o mes al mismo pibe con 1 solo empuje
    private List<GameObject> impactedUnluckyGuys = new List<GameObject>();

    [SerializeField] private MeshCollider myCollider;

    // Parent info
    private PlayerController myPlayerController;
    private PlayerPush myPush;
    private Rigidbody myRb;

    [SerializeField] private GameObject parent;


    private StrenghtPushState currentPushState;

    [Tooltip("A la hora de hacer la resta entre magnitudes de vectores, este valor se sumara para poder handelear mas facilmente los resultados")]
    [SerializeField] private float magnitudDeGracia = 4;

    private void Awake()
    {
        if (!myCollider)
            myCollider = GetComponent<MeshCollider>();
        
        if (!parent)
            parent = transform.parent.transform.parent.gameObject;

        myPlayerController = parent.GetComponent<PlayerController>();
        myPush = parent.GetComponent<PlayerPush>();
        myRb = parent.GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        GameEvents.Player(id).Push.onPush -= OnPush;
        GameEvents.Player(id).Push.onStopPush -= OnStopPush;
    }

    private void OnPush() {
        myCollider.enabled = true;
    }

    private void OnStopPush() {
        myCollider.enabled = false;

        // Notifico a todos los objetivos alcanzados que mi empuje se termino
        foreach (var player in impactedUnluckyGuys) {
            var playerID = player.GetComponent<PlayerController>().id;
        }

        // Limpio la lista
        impactedUnluckyGuys.Clear();
    }

    public void OnEnable()
    {
        if (id != null)
            SetMyEvents();
    }

    public void SetMyEvents()
    {
        GameEvents.Player(id).Push.onPush += OnPush;
        GameEvents.Player(id).Push.onStopPush += OnStopPush;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject impactedOne = other.gameObject;

        // Si soy yo no me calcules piv
        if (impactedOne == gameObject || impactedOne == parent || !other.CompareTag("Player") || impactedUnluckyGuys.Contains(impactedOne))
            return;

        // Aviso a mi empuje que impacte asi se detiene
        GameEvents.Player(id).Push.OnImpactedOne?.Invoke();
        var targetedPlayerId = impactedOne.GetComponent<PlayerController>().id;

        // Get duplicate from the CurrentPushState
        currentPushState = GameEvents.Player(id).Push.GetCurrentPushState?.Invoke().Clone();

        // Get target current push state
        var targetPlayerPushState = GameEvents.Player(targetedPlayerId).Push.GetCurrentPushState?.Invoke();

        var recoilForce = (currentPushState.ForceDirection * -1) * (currentPushState.Strength / myPush.ImpactToStrengthDivisor);

        // Preparo mi estado de fuerza con la info actual
        currentPushState.PrepareStateForSending(parent.transform.forward);

        if (targetPlayerPushState != null)
            TwoPlayersCollisionCase(targetPlayerPushState);

        myRb.AddForce(recoilForce, ForceMode.Impulse);

        // ENVIO LA DATASION DE EMPUJE
        GameEvents.Player(targetedPlayerId).Damage.pushRecieved?.Invoke(currentPushState);

        impactedUnluckyGuys.Add(impactedOne);
    }

    private void TwoPlayersCollisionCase(StrenghtPushState targetPlayerPushState)
    {
        var myStateName = currentPushState.StateName;
        var targetStateName = targetPlayerPushState.StateName;

        // Me stuneo si la fuerza del rival y la mia es strong
        if (myStateName == "Strong" && targetStateName == "Strong") {
            GameEvents.Player(id).Damage.applyEffect?.Invoke(EffectPool.Stun);
            EffectPool.Stun.Behaviour(parent.GetComponent<PlayerMovement>());
        }
        // Aplico la resta de magnitudes entre mi fuerza y la fuerza del rival
        else {
            var magnitudesResult = currentPushState.Strength - targetPlayerPushState.Strength;
            if (magnitudesResult < 0)
                magnitudesResult = 0;
            currentPushState.SetForceVector = currentPushState.ForceDirection * (magnitudesResult + magnitudDeGracia);
        }
    }
}
