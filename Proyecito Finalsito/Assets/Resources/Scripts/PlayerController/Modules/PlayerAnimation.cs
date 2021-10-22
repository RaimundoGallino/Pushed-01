using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimation : PlayerController
{
    [SerializeField] private GameObject parent;
    private Animator animator;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private PlayerPush playerPush;
    [Header("Movement Parameters")]
    [SerializeField] public float velocity = 0.2f;
    [SerializeField] private float aceleration = 4f;
    [SerializeField] private float desaceleration = 1.5f;

    [Space]
    [SerializeField] private string gameScene;
    [SerializeField] private string lobbyScene;

    [SerializeField] private float animatorSpeed = 5f;
    public float AnimatorSpeed {
        get => animatorSpeed;
        set {
            animatorSpeed = value;
            if (animator)
                animator.speed = animatorSpeed;
        }
    }

    void Start()
    {
        // If not animator found just don't use this script
        if (!animator) {
            try {
                animator = GetComponent<Animator>();
            }
            catch {
                this.enabled = false;
                Debug.LogWarning("El animator esta mal seteado pastrana");
                return;
            }
        }
        // Limite del blend tree para setear los valores de la velocidad entre 0 y 1
        velocity = Mathf.Clamp(velocity, 0, 1);

        if (!parent)
            parent = transform.parent.gameObject;

        rb = parent.GetComponent<Rigidbody>();
        playerMovement = parent.GetComponent<PlayerMovement>();
        playerPush = parent.GetComponent<PlayerPush>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void SetMyEvents()
    {
        GameEvents.Player(id).onMove += OnMove;
        GameEvents.Player(id).onNotMoving += OnStopMove;
        GameEvents.Player(id).Push.onPush += OnPush;

        GameEvents.Player(id).Damage.onFreeze += OnFreeze;
    }

    private void OnDisable()
    {
        GameEvents.Player(id).onMove -= OnMove;
        GameEvents.Player(id).onNotMoving -= OnStopMove;
        GameEvents.Player(id).Push.onPush -= OnPush;

        GameEvents.Player(id).Damage.onFreeze -= OnFreeze;
    }

    void FixedUpdate()
    {
        if (!playerMovement || playerMovement.InputVector == null)
            return;

        if (!playerMovement.IsMoving) {
            OnStopMove();
        }

        animator.SetFloat("speed", velocity);
    }

    private void OnMove()
    {
        if (velocity < 1.0f)
            velocity += Time.deltaTime * aceleration;
    }

    private void OnStopMove()
    {
        if (velocity > 0.0f)
            velocity -= Time.deltaTime * desaceleration;
    }

    private void OnPush()
    {
        animator.SetTrigger("push");
    }

    public void FootStepsSound()
    {
        GameEvents.Player(id).Sound.footSteps?.Invoke();
    }

    public void PushSound()
    {
        GameEvents.Player(id).Sound.push?.Invoke();
    }

    /* ================ Freeze =========== */
    private void OnFreeze(float freezeDuration)
    {
        animator.speed = 0;
        Invoke("OnUnFreeze", freezeDuration);
    }
    private void OnUnFreeze()
    {
        animator.speed = 1;
    }
}
