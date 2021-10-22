using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BombBrain : BasePickUp
{
    [Header("Speed")]
    public float chasePlayerSpeed = 5f;
    public float patrolSpeed = 2.5f;

    [Header("Patrol")]
    public float selectNewPatrolTargetTime = 3f;
    [SerializeField] private float explodeCountdown = 3f;

    [Space]
    [SerializeField] private GameObject bombModel;

    [Header("Explosion")]
    [SerializeField] private List<StrengthExplotionState> explotionStates = new List<StrengthExplotionState>();
    [SerializeField] private TextMeshPro tmpText;
    [SerializeField] private bool showForceStatusText = false;
    [SerializeField] float explotionRadio = 6f;
    [SerializeField] float force = 20f;

    private StrengthExplotionState currentExplotionState;

    // For gizmos
    private float currentExplotionStateRadio;

    private void Awake()
    {
        reproducePickUpSound = false;

        id = Guid.NewGuid().ToString();

        GameEvents.ClaimBombEvents(id);

        var comps = new dynamic[] {
            GetComponentsInChildren<BombTouch>(),
            GetComponentsInChildren<BombAnimations>(),
            GetComponentsInChildren<BombMovement>()
        };
        foreach (var arr in comps) {
            foreach (var item in arr) {
                item.id = id;
                item.enabled = true;
            }
        }

        if (!bombModel)
            bombModel = GetComponentInChildren<BombMovement>().gameObject;

        if (!tmpText)
            tmpText = bombModel.transform.GetComponentInChildren<TextMeshPro>();

        foreach (var item in explotionStates) {
            item.InitializeState(force);
        }

        SetMyEvents();
    }

    private void OnEnable() {
        if (id != null)
            SetMyEvents();
    }

    private void OnDisable() {
        GameEvents.Bomb(id).instantExplodeSignal -= Explode;
        GameEvents.Bomb(id).startExplodeTimer -= StartExplodeCountdown;
    }

    private void SetMyEvents()
    {
        GameEvents.Bomb(id).instantExplodeSignal += Explode;
        GameEvents.Bomb(id).startExplodeTimer += StartExplodeCountdown;
    }

    private void StartExplodeCountdown() => Invoke("Explode", explodeCountdown);

    private void Explode()
    {
        AudioManager.current.PlayRandomClipAtPosition("Bomb", "Explotion", transform.position);
        Vector3 bombPos = bombModel.transform.position;
        float dividedRadio = explotionRadio / explotionStates.Count; // el radio se divide segun la cantidad de estados que hay
        float currentRadio = 0;
        List<Collider> targetedColliders = new List<Collider>();

        Instantiate(effect, bombModel.transform.position, Quaternion.identity);
        bombModel.SetActive(false);

        foreach (var state in explotionStates)
        {
            currentRadio += dividedRadio;

            currentExplotionStateRadio = currentRadio;

            Collider[] colliders = Physics.OverlapSphere(bombPos, currentRadio);

            foreach (Collider nearbyObject in colliders)
            {
                if (!nearbyObject.CompareTag("Player") || targetedColliders.Contains(nearbyObject))
                    continue;

                var playerId = nearbyObject.GetComponent<PlayerController>().id;
                var forceDirection = (nearbyObject.transform.position - bombPos).normalized;

                state.PrepareStateForSending(forceDirection);

                GameEvents.Player(playerId).Damage.recieveExplotion?.Invoke(state);
                targetedColliders.Add(nearbyObject);
            }
        }

        Destroy(gameObject);
    }

    private void ForceStatusText(string text, Color color) {
        if (!showForceStatusText)
            return;
        tmpText.text = text;
        tmpText.color = color;
    }

    private void OnDrawGizmosSelected() {
        if (currentExplotionState == null)
            Gizmos.DrawWireSphere(bombModel.transform.position, explotionRadio);
        else
            Gizmos.DrawWireSphere(bombModel.transform.position, currentExplotionStateRadio);
    }
}
