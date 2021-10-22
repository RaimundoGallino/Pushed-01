using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class PlayerMovement : PlayerController
{
    // ========= Particle effect =================
    [SerializeField] private ParticleSystem effect;
    private ParticleSystem.EmissionModule effectParticleEmission;
    private void ActiveEffect() => effectParticleEmission.enabled = true;
    private void UnActiveEffect() => effectParticleEmission.enabled = false;
    // ==========================================

    [SerializeField] private PlayerGroundCheck groundCheck;

    // ================== Movement fields ==================
    private Rigidbody rb;
    [SerializeField] private float movementForce = 100f;
    private float defaultMovementForce;

    private bool allowLookAt = true;

    [SerializeField] private float minMovementForce = 0.3f;
    private float maxMovementForce;
    [SerializeField] private float movementForceGainSpeed = 0.5f;

    [SerializeField] private bool canMove = true;

    private Vector3 force = Vector3.zero;
    private Vector2 inputVector;
    public Vector2 InputVector => inputVector;

    private Coroutine activeIncreasingMoveForceCoroutine;
    // =============================

    private bool isMoving = false;
    public bool IsMoving => isMoving;
    private bool stoppedMoving = false;

    private PlayerPush myPush;

#region Explotion Case Affects
    // ========== Explotions affects ==================
    private bool recievingExplotionDamage = false;
    private float forceCounterExplotion;
    [SerializeField] private float recoverCounterForceRate = .4f;

    private StrengthExplotionState[] activeExplotionStates;
    private List<Coroutine> affectedExplotionsCoroutines = new List<Coroutine>();
    // =========================================
#endregion

#region My Push Case Affects
    // ======= Push Affects ==============
    private PlayerPush.ExhaustState currentExhaustState;
    private bool isPushExhausted;
    // ===========================
#endregion

#region Other Players Push Affects
    // ======= Enemy Pushes Affects ==============
    private StrenghtPushState[] activePushStates;
    private List<Coroutine> affectedPushesCoroutines = new List<Coroutine>();
    // ===========================
#endregion

    [SerializeField] private ForceMode forceMode = ForceMode.Impulse;
    private ForceMode defaultForceMode;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private GameObject myCamObj;

	private void Awake() {
        rb = GetComponent<Rigidbody>();
        
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (!effect)
            effect = GetComponentInChildren<ParticleSystem>();
        effectParticleEmission = effect.emission;
        UnActiveEffect();

        defaultForceMode = forceMode;

        groundCheck = GetComponentInChildren<PlayerGroundCheck>();

        // Setting default values
        maxMovementForce = movementForce;
        defaultMovementForce = movementForce;

        myPush = GetComponent<PlayerPush>();
    }

    private void OnDisable()
    {
        GameEvents.Player(id).onMove -= ActiveEffect;
        GameEvents.Player(id).onNotMoving -= UnActiveEffect;

        // My Push Affections
        GameEvents.Player(id).Push.onPush -= OnPush;
        GameEvents.Player(id).Push.onStopPush -= OnStopPush;
        GameEvents.Player(id).Push.onReachedExhaustState -= ExhaustForce;

        // Explotion Affections
        GameEvents.Player(id).Damage.onExplotionRecieved -= OnExplotionRecieved;
        GameEvents.Player(id).Damage.hasNoExplotionsAffecting -= NoMoreExplotionsAffecting;
        
        // Other Player Pushes Affections
        GameEvents.Player(id).Damage.onPushRecieved -= OnPushRecieved;
        GameEvents.Player(id).Damage.hasNoPushesAffecting -= NoMorePushesAffecting;
        
        GameEvents.Player(id).Damage.onFreeze -= onFreeze;
        GameEvents.Player(id).Damage.onSpeedUp -= onSpeedUp;
    }

    public override void SetMyEvents()
    {
        base.SetMyEvents();
        GameEvents.Player(id).onMove += ActiveEffect;
        GameEvents.Player(id).onNotMoving += UnActiveEffect;

        // My Push Affections
        GameEvents.Player(id).Push.onPush += OnPush;
        GameEvents.Player(id).Push.onStopPush += OnStopPush;
        GameEvents.Player(id).Push.onReachedExhaustState += ExhaustForce;

        // Explotion Affections
        GameEvents.Player(id).Damage.onExplotionRecieved += OnExplotionRecieved;
        GameEvents.Player(id).Damage.hasNoExplotionsAffecting += NoMoreExplotionsAffecting;
        
        // Other Player Pushes Affections
        GameEvents.Player(id).Damage.onPushRecieved += OnPushRecieved;
        GameEvents.Player(id).Damage.hasNoPushesAffecting += NoMorePushesAffecting;
    
        GameEvents.Player(id).Damage.onFreeze += onFreeze;
        GameEvents.Player(id).Damage.onSpeedUp += onSpeedUp;
    }

    private void FixedUpdate()
    {
        // Solo moverse cuando esta tocando el suelo
        if (groundCheck.IsGrounded && canMove)
            Movement();
    }

#region  Movement Itself
    /* =============================== MOVEMENT ================================== */

    /// <summary> Detiene la ejecucion de movimiento </summary>
    private void Stop(bool hardStop = false)
    {
        StopAllCoroutines();
        CancelInvoke();

        if (!hardStop)
            OnStoppedMoving();
    }

    private void ResetMovementForce()
    {
        if (isPushExhausted)
            return;

        StopIncreasingMovementForce();

        movementForce = minMovementForce;
    }
    private void StartIncreasingMovementForce()
    {
        if (isPushExhausted)
            return;

        if (activeIncreasingMoveForceCoroutine == null)
            activeIncreasingMoveForceCoroutine = StartCoroutine("IncrementMovementForce");
    }

    private void StopIncreasingMovementForce()
    {
        if (activeIncreasingMoveForceCoroutine != null) {
            StopCoroutine(activeIncreasingMoveForceCoroutine);
            activeIncreasingMoveForceCoroutine = null;
        }
    }

    private IEnumerator IncrementMovementForce()
    {
        while (movementForce < maxMovementForce)
        {
            movementForce += Time.deltaTime * movementForceGainSpeed;
            yield return new WaitForFixedUpdate();
        }

        movementForce = maxMovementForce;
    }

    private void LookAt()
    {
        Vector3 direction = rb.velocity;
        direction.y = 0;

        if (InputVector.sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;

        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;

        return right.normalized;
    }

    public void OnMoveKeysPress(InputAction.CallbackContext cntx) {
        inputVector = cntx.ReadValue<Vector2>();
    }

    private void Movement()
    {
        var input_vec = inputVector.normalized;
        force = Vector3.zero;

        // Si el vector de movimiento es nulo, significa que no me estoy moviendo
        // por lo tanto no agreges fuerzas
        if (input_vec == Vector2.zero) {
            if (!stoppedMoving) {
                OnStoppedMoving();
                GameEvents.Player(id).onNotMoving?.Invoke();
                stoppedMoving = true;
            }
            return;
        }
        stoppedMoving = false;

        isMoving = true;

        if (allowLookAt)
            LookAt();

        force += input_vec.x * GetCameraRight(playerCamera);
        force += input_vec.y * GetCameraForward(playerCamera);
        force.y = 0;
        force *= movementForce;

        //StartIncreasingMovementForce();
        GameEvents.Player(id).onMove?.Invoke();

        // Si me estan explotando
        if (recievingExplotionDamage)
            return;

        rb.AddForce(force, forceMode);
    }

    public void CanMoveFalse() {
        canMove = false;
    }
    public void CanMoveTrue() {
        // Lo siento, pero no te muevas aun si tenes stun
        var activeEffects = GameEvents.Player(id).Damage.GetActiveEffects?.Invoke();
        if (activeEffects.Contains(EffectPool.Stun))
            return;

        canMove = true;
    }

    private void OnStoppedMoving()
    {
        isMoving = false;
        ResetMovementForce();
    }

    /* =========================================================== */
#endregion

private IEnumerator LookAtBlock(float recoverDelay)
{
    LookAt();
    allowLookAt = false;

    // Pequeño delay para volver a usar el LookAt
    yield return new WaitForSeconds(recoverDelay);

    allowLookAt = true;
}

#region MY PUSH REACTIONS
/* =================== PUSHING REACTIONS ================= */
    private void OnPush()
    {
        canMove = false;
    }

    private void OnStopPush()
    {
        // Lo siento, pero no te muevas aun si tenes stun
        var activeEffects = GameEvents.Player(id).Damage.GetActiveEffects?.Invoke();
        if (activeEffects.Contains(EffectPool.Stun))
            return;

        canMove = true;
    }

    // Se llama cuando el jugador alcanzó la etapa final del push
    private void ExhaustForce(PlayerPush.ExhaustState exhaustStateInfo)
    {
        currentExhaustState = exhaustStateInfo;
        isPushExhausted = true;
        movementForce = currentExhaustState.movementForceDecrease;
        StopAllCoroutines();
        StartCoroutine("ExitExhaustState");
    }
    private IEnumerator ExitExhaustState()
	{
        yield return new WaitForSeconds(currentExhaustState.stateDuration);
        isPushExhausted = false;
        currentExhaustState = null;
        ResetMovementForce();
    }
#endregion

#region Explotion Case
    private void OnExplotionRecieved()
    {
        ResetMovementForce();

        // Si ya tenia estados en ejecucion, los anoto
        var previousList = new List<StrengthExplotionState>();
        forceCounterExplotion = 0;
        if (activeExplotionStates != null)
            previousList.AddRange(activeExplotionStates);

        // Obtengo los estados de explosion recividos
        activeExplotionStates = GameEvents.Player(id).Damage.GetActiveExplotionStates?.Invoke();
        recievingExplotionDamage = true;

        foreach (var state in activeExplotionStates) {
            // Si mi anterior lista contiene este estado, no ejecutes otra coroutina
            if (previousList.Contains(state))
                continue;

            // Mando una corutina que sirva como Update que actualice la fuerza de explosion del estado recivido
            dynamic obj = new ExpandoObject();
            var coro = ExplotionForceBehaviour(state, obj);

            // Guardo la corutina en mi array de corutinas
            obj.Coroutine = StartCoroutine(coro);
            affectedExplotionsCoroutines.Add(obj.Coroutine);
        }
    }
    private IEnumerator ExplotionForceBehaviour(StrengthExplotionState state, dynamic obj)
    {
        var frames = 0;

        while (frames < state.DurationFrames)
        {
            var bombForce = state.ForceDirection;
            if (isMoving)
                forceCounterExplotion += Time.deltaTime * recoverCounterForceRate;

            if (force.magnitude != 0 && forceCounterExplotion >= 1) {
                bombForce *= state.Strength / (force.magnitude * forceCounterExplotion);
            }
            else {
                bombForce *= state.Strength;
            }

            rb.AddForce(bombForce, state.ForceMode);

            frames++;

            yield return new WaitForFixedUpdate();
        }

        affectedExplotionsCoroutines.Remove(obj.Coroutine);
    }
    private void NoMoreExplotionsAffecting()
    {
        activeExplotionStates = null;
        recievingExplotionDamage = false;
    }
#endregion

    // ============== Effect Reactions ==================
    public void OnStun(float stunDuration)
    {
        Stop();

        Invoke("CanMoveTrue", stunDuration);
    }

#region OTHER PLAYERS PUSH
    /* ============== OTHER PLAYERS PUSH =========== */
    private void OnPushRecieved()
    {
        ResetMovementForce();

        // Si ya tenia estados en ejecucion, los anoto
        var previousList = new List<StrenghtPushState>();
        if (activePushStates != null)
            previousList.AddRange(activePushStates);

        // Obtengo los estados de empuje recividos
        activePushStates = GameEvents.Player(id).Damage.GetActivePushesStates?.Invoke();

        foreach (var state in activePushStates) {
            // Si mi anterior lista contiene este estado, no ejecutes otra coroutina
            if (previousList.Contains(state))
                continue;

            // Mando una corutina que sirva como Update que actualice la fuerza de empuje del estado recivido
            dynamic obj = new ExpandoObject();
            var coro = EnemyPushUpdate(state, obj);

            obj.Coroutine = StartCoroutine(coro);
            // Guardo la corutina en mi array de corutinas
            affectedPushesCoroutines.Add(obj.Coroutine);
        }
    }

    private void NoMorePushesAffecting()
    {
        activePushStates = null;
    }

    /// <summary>
    /// Aplica fuerza con un estado recivido durante los frames definidos en el estado <br/>
    /// <param name="state">- State: El estado recivido </param> <br/>
    /// <param name="obj">- Obj: Objeto que contiene la instancia de la coruitna actual para cuando termine poder quitarla de la lista </param>
    /// </summary>
    private IEnumerator EnemyPushUpdate(StrenghtPushState state, dynamic obj)
    {
        var frames = 0;
        var originalMass = rb.mass;

        // Does the push
        while (frames < state.DurationFrames)
        {
            var pushForce = state.ForceVector;

            rb.mass += state.Strength * Time.fixedDeltaTime * 2;

            if (force != Vector3.zero) {
                pushForce -= force;
            }

            pushForce.y = 0;
            rb.AddForce(pushForce, state.ForceMode);

            frames++;

            yield return new WaitForFixedUpdate();
        }

        // Restore the mass
        while (originalMass < rb.mass) {
            rb.mass -= state.Strength * Time.fixedDeltaTime * 2;
            yield return new WaitForFixedUpdate();
        }

        rb.mass = originalMass;

        affectedPushesCoroutines.Remove(obj.Coroutine);
    }
#endregion

    // ============== Freeze effect ==================
    private void onFreeze(float freezeDuration)
    {
        CanMoveFalse();
        Invoke("OnStopFreeze", freezeDuration);
    }
    private void OnStopFreeze()
    {
        CanMoveTrue();
    }

    // ============== SpeedUp effect ==================
    public void onSpeedUp(float speedUpTime, float speedUpVelocity)
    {
        movementForce += speedUpVelocity;
        Invoke("onNormalSpeed", speedUpTime);
    }

    public void onNormalSpeed()
    {
        movementForce = defaultMovementForce;
    }

}