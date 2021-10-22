using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using static DreadZitoTools.Utils;
using System;

public class PlayerDamageHandler : PlayerController
{
    [SerializeField] private Rigidbody rb;

    /*=============== Bomb explotion ============ */
    [SerializeField] private List<StrengthExplotionState> recievedExplotions = new List<StrengthExplotionState>();
    /* ====================================== */

    /* ================ PlayerEffects =================== */
    [System.Serializable]
    class EffectInfo {
        public float startDurationTime;
        public BaseEffect effect;
    }
    [SerializeField] private List<EffectInfo> effects = new List<EffectInfo>();
    private bool anEffectIsActive = false;
    /* =================================== */

    /*=================== Push Forces ==============*/
    [SerializeField] private List<StrenghtPushState> recievedPushes = new List<StrenghtPushState>();
    /*============================================*/

    private void Start()
    {
        if (!rb)
            rb = GetComponent<Rigidbody>();
    }

    public override void OnEnable() {
        base.OnEnable();
    }

    private void OnDisable()
    {
        GameEvents.Player(id).Damage.pushRecieved -= RecievePush;
        GameEvents.Player(id).Damage.recieveExplotion -= RecieveExplotion;
        GameEvents.Player(id).Damage.GetActiveEffects -= GetActiveEffects;
        GameEvents.Player(id).Damage.GetActivePushesStates -= GetCurrentPushesStates;
        GameEvents.Player(id).Damage.GetActiveExplotionStates -= GetCurrentExplotionStates;
        GameEvents.Player(id).Damage.applyEffect -= AddEffect;
    }

    public override void SetMyEvents()
    {
        GameEvents.Player(id).Damage.pushRecieved += RecievePush;
        GameEvents.Player(id).Damage.recieveExplotion += RecieveExplotion;
        GameEvents.Player(id).Damage.GetActiveEffects += GetActiveEffects;
        GameEvents.Player(id).Damage.GetActivePushesStates += GetCurrentPushesStates;
        GameEvents.Player(id).Damage.GetActiveExplotionStates += GetCurrentExplotionStates;
        GameEvents.Player(id).Damage.applyEffect += AddEffect;
    }

    public void AddEffect(BaseEffect effect)
    {
        effects.Add(new EffectInfo() {
            startDurationTime = effect.duration + Time.time,
            effect = effect
        });

        anEffectIsActive = true;
    }

    private BaseEffect[] GetActiveEffects()
    {
        var activeEffects = new List<BaseEffect>();

        foreach (var effect in effects) {
            activeEffects.Add(effect.effect);
        }

        return activeEffects.ToArray();
    }

    /**
    * RecievePush - Recive la informacion de empuje de un oponente
    * @state: el estado de empuje del adversario
    * ---------------------------------
    */
    private void RecievePush(StrenghtPushState enemyPushState) {
        // Lo agrego a mi lista
        recievedPushes.Add(enemyPushState);

        // Doy aviso que se activo un empuje en contra
        GameEvents.Player(id).Damage.onPushRecieved?.Invoke();

        // Inicio la corrutina para eliminar el empuje enemigo de la lista
        StartCoroutine(RemoveRecievedPushState(enemyPushState));
    }
    private IEnumerator RemoveRecievedPushState(StrenghtPushState state)
    {
        yield return new WaitForSeconds(state.Duration);

        recievedPushes.Remove(state);
        if (recievedPushes.Count == 0)
            GameEvents.Player(id).Damage.hasNoPushesAffecting?.Invoke();
    }
    private StrenghtPushState[] GetCurrentPushesStates() => recievedPushes.ToArray();

    /**
    * RecieveExplotion - Recive la explosion de una bomba
    * @explotionState: el estado de la explosion recivido
    * --------------------------------------------
    */
    private void RecieveExplotion(StrengthExplotionState explotionState)
    {
        // Lo agrego a mi lista
        recievedExplotions.Add(explotionState);

        // Doy aviso que se activo una explosion
        GameEvents.Player(id).Damage.onExplotionRecieved?.Invoke();

        // Inicio la corrutina para eliminar la explosion de la lista
        StartCoroutine(RemoveRecievedExplotionState(explotionState));
    }
    private IEnumerator RemoveRecievedExplotionState(StrengthExplotionState state)
    {
        yield return new WaitForSeconds(state.Duration);

        recievedExplotions.Remove(state);
        if (recievedPushes.Count == 0)
            GameEvents.Player(id).Damage.hasNoExplotionsAffecting?.Invoke();
    }
    private StrengthExplotionState[] GetCurrentExplotionStates() => recievedExplotions.ToArray();

    private void Update()
    {
        if (!anEffectIsActive)
            return;
        // Itero por la lista de efectos
        foreach (var effect in effects) {
            // SI aun no se cumplio la duracion del efecto, continuo
            if (Time.time < effect.startDurationTime)
                continue;

            // Si la duracion se cumplio lo descarto de la lista e itero en el siguiente frame
            effects.Remove(effect);
            break;
        }

        // SI se vacio la lista de efectos detengo el update
        if (effects.Count == 0)
            anEffectIsActive = false; 
    }
}
