using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BombMovement : BasePickUp
{
    [SerializeField] private Rigidbody ourRigidbody;

    [SerializeField] private GameObject parent;

    private BombBrain bombBrain;
    [SerializeField] private BombGroundCheck bombGroundCheck;

    private float selectNewPatrolTargetTime = 3f;
    private float speed;

    private bool patroling = false;
    private bool playerChase = false;

    private Transform selectedPatrolTarget;

    // Lista que contiene todos los puntos para recorrer
    [SerializeField] private List<GameObject> Points;

    private float patrullaSpeed;

    private void Awake()
    {
        reproducePickUpSound = false;

        if (!parent)
            parent = transform.parent.gameObject;

        bombBrain = parent.GetComponent<BombBrain>();

        if (Points == null || Points.Count == 0)
        {
            Points = new List<GameObject>();

            var parent = transform.parent;
            for (int i = 0; i < parent.childCount; i++) {
                var currentChild = parent.GetChild(i).gameObject;

                if (currentChild == gameObject || currentChild.GetComponent<TextMeshPro>() != null)
                    continue;
                Points.Add(currentChild);
            }
        }

        if (!bombGroundCheck)
            bombGroundCheck = GetComponentInChildren<BombGroundCheck>();
    }

    private void OnEnable() {
        if (id != null)
            SetMyEvents();
    }

    private void OnDisable() {
        base.onPlayerSpotted -= PlayerTargeted;
    }

    private void SetMyEvents()
    {
        base.onPlayerSpotted += PlayerTargeted;
    }

    void Start()
    {
        if (!ourRigidbody)
            ourRigidbody = transform.parent.GetComponent<Rigidbody>();

        patrullaSpeed = bombBrain.patrolSpeed;
        speed = bombBrain.chasePlayerSpeed;
        selectNewPatrolTargetTime = bombBrain.selectNewPatrolTargetTime;

        StartPatrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bombGroundCheck.IsGrounded)
            return;

        // Encontró al player y va directo hacia él
        if (playerChase)
        {
            if (transform.position != player.transform.position)
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            transform.LookAt(player.transform.position);
        }
        // Mientras no llega a destino
        else if (patroling)
        {
            if (!selectedPatrolTarget)
                return;

            var pos = transform.position;
            var targetPos = selectedPatrolTarget.position;

            if (Vector3.Distance(pos, targetPos) >= 0.2f) {
                transform.position = Vector3.MoveTowards(pos, targetPos, patrullaSpeed * Time.deltaTime);
            }
            else {
                GameEvents.Bomb(id).onReachPatrolPoint?.Invoke();
                Invoke("StartPatrol", selectNewPatrolTargetTime);
                patroling = false;
            }
        }
    }

    // Selecciona un destino random para ir 
    IEnumerator Select()
    {
        int randomIndx = -1;

        if (selectedPatrolTarget) {
            var indexes = new List<int>();
            var startIndex = 0;

            foreach (var item in Points) {
                if (item != selectedPatrolTarget.gameObject)
                    indexes.Add(startIndex);
                startIndex++;
            }

            var randIdx = UnityEngine.Random.Range(0, indexes.Count);
            randomIndx = indexes[randIdx];
        }
        else {
            randomIndx = UnityEngine.Random.Range(0, Points.Count);
        }

        while (selectedPatrolTarget != null && selectedPatrolTarget == Points[randomIndx].transform ||
            !Points[randomIndx].GetComponent<BombWaypoint>().isGrounded)
        {
            randomIndx = UnityEngine.Random.Range(0, Points.Count);
            yield return null;
        }

        selectedPatrolTarget = Points[randomIndx].transform;
        transform.LookAt(selectedPatrolTarget);
    }

    private void StartPatrol()
    {
        StartCoroutine(Select());
        patroling = true;
        playerChase = false;

        GameEvents.Bomb(id).onPatrol?.Invoke();
    }

    private void PlayerTargeted()
    {
        CancelInvoke();
        patroling = false;
        playerChase = true;

        GameEvents.Bomb(id).startExplodeTimer?.Invoke();
        GameEvents.Bomb(id).onChase?.Invoke();
    }
}
