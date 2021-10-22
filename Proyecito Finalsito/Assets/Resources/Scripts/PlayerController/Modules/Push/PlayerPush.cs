using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;

#region README
/*
⠄⠄⠄⢰⣧⣼⣯⠄⣸⣠⣶⣶⣦⣾⠄⠄⠄⠄⡀⠄⢀⣿⣿⠄⠄⠄⢸⡇⠄⠄
⠄⠄⠄⣾⣿⠿⠿⠶⠿⢿⣿⣿⣿⣿⣦⣤⣄⢀⡅⢠⣾⣛⡉⠄⠄⠄⠸⢀⣿⠄
⠄⠄⢀⡋⣡⣴⣶⣶⡀⠄⠄⠙⢿⣿⣿⣿⣿⣿⣴⣿⣿⣿⢃⣤⣄⣀⣥⣿⣿⠄
⠄⠄⢸⣇⠻⣿⣿⣿⣧⣀⢀⣠⡌⢻⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⠿⠿⣿⣿⣿⠄
⠄⢀⢸⣿⣷⣤⣤⣤⣬⣙⣛⢿⣿⣿⣿⣿⣿⣿⡿⣿⣿⡍⠄⠄⢀⣤⣄⠉⠋⣰
⠄⣼⣖⣿⣿⣿⣿⣿⣿⣿⣿⣿⢿⣿⣿⣿⣿⣿⢇⣿⣿⡷⠶⠶⢿⣿⣿⠇⢀⣤
⠘⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣽⣿⣿⣿⡇⣿⣿⣿⣿⣿⣿⣷⣶⣥⣴⣿⡗
⢀⠈⢿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠄
⢸⣿⣦⣌⣛⣻⣿⣿⣧⠙⠛⠛⡭⠅⠒⠦⠭⣭⡻⣿⣿⣿⣿⣿⣿⣿⣿⡿⠃⠄
⠘⣿⣿⣿⣿⣿⣿⣿⣿⡆⠄⠄⠄⠄⠄⠄⠄⠄⠹⠈⢋⣽⣿⣿⣿⣿⣵⣾⠃⠄
⠄⠘⣿⣿⣿⣿⣿⣿⣿⣿⠄⣴⣿⣶⣄⠄⣴⣶⠄⢀⣾⣿⣿⣿⣿⣿⣿⠃⠄⠄
⠄⠄⠈⠻⣿⣿⣿⣿⣿⣿⡄⢻⣿⣿⣿⠄⣿⣿⡀⣾⣿⣿⣿⣿⣛⠛⠁⠄⠄⠄
⠄⠄⠄⠄⠈⠛⢿⣿⣿⣿⠁⠞⢿⣿⣿⡄⢿⣿⡇⣸⣿⣿⠿⠛⠁⠄⠄⠄⠄⠄
⠄⠄⠄⠄⠄⠄⠄⠉⠻⣿⣿⣾⣦⡙⠻⣷⣾⣿⠃⠿⠋⠁⠄⠄⠄⠄⠄⢀⣠⣴
⣿⣿⣿⣶⣶⣮⣥⣒⠲⢮⣝⡿⣿⣿⡆⣿⡿⠃⠄⠄⠄⠄⠄⠄⠄⣠⣴⣿⣿⣿
*/
#endregion

[RequireComponent(typeof(Rigidbody))]
public class PlayerPush : PlayerController
{
    private Rigidbody rb;
    [SerializeField] private PlayerGroundCheck groundCheck;

    // Trail effect ===========
    [SerializeField] private TrailRenderer trailEffect;
    private void ActiveTrailEffect() => trailEffect.enabled = true;
    private void UnActiveTrailEffect() => trailEffect.enabled = false;
    //========================

    public bool CanPush = true;
    private Vector3 pushVector;
    private Vector3 force;
    [Tooltip("Fuerza de empuje base para cada estado")]
    [SerializeField] private float basePushForce = 10f;
    [Tooltip("Maneja cuanto se desplaza el jugador en cada estado")]
    [SerializeField] private float baseImpulseForce = 10f;
    private Coroutine activePushingCoroutine;
    [Tooltip("Es el divisor de la fuerza cuando se aplica el recoil al impactar con otro player")]
    [SerializeField] private float impactToStrengthDivisor = 2;
    public float ImpactToStrengthDivisor => impactToStrengthDivisor;

    private bool _isPushing = false;
    public bool isPushing=> _isPushing;
    private void IsPushingTrue() => _isPushing = true;
    private void IsPushingFalse() => _isPushing = false;

    [Header("Testing")]
    [Tooltip("Activo: Muestra un texto sobre la cabeza del jugador indicando los diferentes estados de empuje")]
    [SerializeField] private TextMeshPro pushStateTmpText;
    [SerializeField] private bool showPushStateText;
    [SerializeField] private TextMeshPro spamStageTmpText;
    [SerializeField] private bool showPushStageIndexText;

    private bool inCooldown = false;

    private int spamCounter = -1;
    [Tooltip("Indica cuantos spameos maximos tendra el jugador, cada nivel de spam reduce las capacidades de un empuje basandose en su antecesor")]
    [SerializeField] private int maxSpams = 5;
    [Tooltip("Cooldown entre spameos del empuje, no aplica para cuando alcanza el estado mas cansado")]
    [SerializeField] private float cooldown = .2f;

    // ========== Force states =============
    [Tooltip("Los diferentes estados que sufre un empuje")]
    [SerializeField] private List<StrenghtPushState> PushStates = new List<StrenghtPushState>();
    // Lista de etapas de empuje
    [SerializeField] private List<List<StrenghtPushState>> spamLevel = new List<List<StrenghtPushState>>();
    [SerializeField] private List<Color> spamLevelColors = new List<Color>() {Color.green, Color.yellow, Color.magenta, Color.red };
    private List<StrenghtPushState> currentPushStage = new List<StrenghtPushState>();
    private StrenghtPushState currentPushState;
    public StrenghtPushState GetCurrentPushState() => currentPushState;
    // ====================================

    [SerializeField] private PlayerPushAimBot pushAimBot;

    [Tooltip("Tiempo que demora en recuperarse la fuerza de empuje")]
    [SerializeField] private float pushPowerRecoverTime = 1.5f;
    //========================

    [System.Serializable]
    public class ExhaustState
	{
        public float movementForceDecrease = 10;
        public float stateDuration = 3.5f;
	}

    [Tooltip("Objeto que tiene la informacion acerca del estado mas cansado del empuje. Tambieen afecta el movimiento en PlayerMovement")]
    [SerializeField] private ExhaustState exhaustStateInfo;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (pushStateTmpText == null)
            pushStateTmpText = GetComponentsInChildren<TextMeshPro>().Where(elem => elem.name.Equals("ForceStatus")).First();
        if (spamStageTmpText == null)
            spamStageTmpText = GetComponentsInChildren<TextMeshPro>(true).Where(elem => elem.name.Equals("SpamStageInfo")).First();

        if (!pushAimBot)
            pushAimBot = GetComponentInChildren<PlayerPushAimBot>();

        if (showPushStageIndexText)
            spamStageTmpText.gameObject.SetActive(true);

        if (!groundCheck)
            groundCheck = GetComponent<PlayerGroundCheck>();

        /* Setear la fuerza base por cada PUSH STATE */
        foreach (var item in PushStates) {
            item.InitializeState(basePushForce, baseImpulseForce);
        }

        /* Setear estructura de spamLevel */
        List<StrenghtPushState> auxPushStates = new List<StrenghtPushState>();
        auxPushStates.AddRange(ReplicateList(PushStates));

        for (int i = 0; i < maxSpams; i++)
        {
            // Si es 0 toda su potencia debe quedar intacta
            if (i != 0) {
                foreach (var item in auxPushStates) {
                    item.LossPower();
                }
            }

            spamLevel.Add(ReplicateList(auxPushStates));
        }

        auxPushStates.Clear();
    }

    public override void SetMyEvents()
    {
        GameEvents.Player(id).Push.onPush += ActiveTrailEffect;
        GameEvents.Player(id).Push.onStopPush += UnActiveTrailEffect;
        GameEvents.Player(id).Push.OnImpactedOne += OnImpactedOne;

        // Getter event
        GameEvents.Player(id).Push.GetCurrentPushState += GetCurrentPushState;

        // Freeze
        GameEvents.Player(id).Damage.onFreeze += OnFreeze;
    }

    private void OnDisable()
    {
        GameEvents.Player(id).Push.onPush -= ActiveTrailEffect;
        GameEvents.Player(id).Push.onStopPush -= UnActiveTrailEffect;
        GameEvents.Player(id).Push.OnImpactedOne -= OnImpactedOne;

        // Getter event
        GameEvents.Player(id).Push.GetCurrentPushState -= GetCurrentPushState;

        // Freeze
        GameEvents.Player(id).Damage.onFreeze -= OnFreeze;
    }

    // ====== Cooldown ============
    private void StartCooldown() {
        CancelInvoke();
        StopAllCoroutines();
        inCooldown = true;
    }
    private void StartCooldownEndTimer() {
        float usedCooldown = cooldown;

        // Cuando el jugador esta exausto
        if (spamCounter == maxSpams - 1) {
            usedCooldown = exhaustStateInfo.stateDuration;
            GameEvents.Player(id).Push.onReachedExhaustState?.Invoke(exhaustStateInfo);
        }

        Invoke("CooldownEnd", usedCooldown);
    }
    private void CooldownEnd()
    {
        PushSpamHandler();
        inCooldown = false;
    }

    public void OnPushKeyPress(InputAction.CallbackContext cntx)
    {
        if (inCooldown || !this.groundCheck.IsGrounded || !CanPush)
            return;

        // Increment the spam counter
        if (spamCounter != spamLevel.Count - 1)
            spamCounter ++;

        // Ajusto el aimbot detector segun mi etapa de empuje
        if (pushAimBot)
            pushAimBot.ScaleDetector(spamLevel.Count, spamCounter);

        if (showPushStageIndexText)
            StatusTextSet(spamStageTmpText, $"Spam Stage: {spamCounter}", Color.clear);

        currentPushStage = spamLevel[spamCounter];

        StartCooldown();
        _isPushing = true;
        GameEvents.Player(id).Push.onPush?.Invoke();

        activePushingCoroutine = StartCoroutine(Pushing());
    }

    private IEnumerator Pushing()
    {
        // Moverse entre los diferentes estados de la etapa actual
        for (int i = 0; i < currentPushStage.Count; i++)
        {
            var elem = currentPushStage[i];
            var pos = transform.position;
            pos.y = 0;

            if (showPushStateText)
                StatusTextSet(pushStateTmpText, elem.StateName, elem.Color);

            currentPushState = elem;

            // Consigo la posicion del target alcanzado por el aimbot
            var aimBotTarget = GameEvents.Player(id).AimBot.GetGameObjectOfTarget?.Invoke();
            var forceDirection = transform.forward;

            // Si el aimbot encontro a un target, dirige la direccion hacia el
            if (aimBotTarget) {
                forceDirection = (aimBotTarget.transform.position - transform.position).normalized;
                transform.LookAt(aimBotTarget.transform);
            }

            forceDirection.y = 0;

            var force = forceDirection * elem.ImpulseForce;

            rb.AddForce(force, ForceMode.Impulse);

            yield return new WaitForSeconds(elem.Duration);
        }
        Stop();
    }

    private void Stop()
    {
        currentPushState = null;

        if (showPushStateText)
            StatusTextSet(pushStateTmpText, "", Color.white);

        activePushingCoroutine = null;
        rb.velocity = Vector3.zero;
        _isPushing = false;
        StartCooldownEndTimer();
        GameEvents.Player(id).Push.onStopPush?.Invoke();
    }
    
    private void StatusTextSet(TextMeshPro tmpText, string text, Color color) {
        tmpText.text = text;
        if (color != Color.clear)
            tmpText.color = color;
    }

    private void PushSpamHandler()
    {
        var restorePushTime = pushPowerRecoverTime;

        // Se me agotó la stamina ;c (llego al ultimo indice de spameo)
        if (spamCounter == maxSpams - 1) {
            restorePushTime = exhaustStateInfo.stateDuration;
        }

        // Iniciar cuenta atras para recuperarse
        StartCoroutine(StartRegeneratingPower(restorePushTime));
    }

    private IEnumerator StartRegeneratingPower(float cooldownTime)
    {
        // Wait for cooldownTime
        yield return new WaitForSeconds(cooldownTime);

        spamCounter--;

        // Ajusto el aimbot detector segun mi etapa de empuje
        if (pushAimBot)
            pushAimBot.ScaleDetector(spamLevel.Count, spamCounter);

        if (showPushStageIndexText)
            StatusTextSet(spamStageTmpText, $"Spam Stage: {spamCounter}", Color.clear);

        // Call it again if is not the first stage
        if (spamCounter != -1) {
            StartCoroutine(StartRegeneratingPower(pushPowerRecoverTime));
        }
        else {
            // Ya se recupero totalmente
            StatusTextSet(spamStageTmpText, "", Color.clear);
        }

        yield break;
    }

    private void OnImpactedOne()
    {
        if (activePushingCoroutine != null)
            StopCoroutine(activePushingCoroutine);

        //rb.velocity = Vector3.zero;

        StartCoroutine("EffectCheck");
    }

    // Comprueba si no esta ningun efecto malisioso antes de frenar el estado empuje
    private IEnumerator EffectCheck()
    {
        // Wait to end of frame
        yield return null;

        var stopDelayTime = 0f;
        var effectList = GameEvents.Player(id).Damage.GetActiveEffects?.Invoke();

        // Wait until the stun effect end
        if (effectList.Contains(EffectPool.Stun))
            stopDelayTime = EffectPool.Stun.duration;

        Invoke("Stop", stopDelayTime);
    }
 
    /* ============= Freeze */
    private void OnFreeze(float freezeDuration)
    {
        CanPush = false;
        Invoke("OnStopFreeze", freezeDuration);
    }
    private void OnStopFreeze()
    {
        CanPush = true;
    }


    /* ======================= */

    private List<StrenghtPushState> ReplicateList(List<StrenghtPushState> target)
    {
        var newList = new List<StrenghtPushState>();

        foreach (var item in target) {
            newList.Add(item.Clone());
        }

        return newList;
    }
}
