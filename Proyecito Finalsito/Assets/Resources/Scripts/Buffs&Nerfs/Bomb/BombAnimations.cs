using UnityEngine;

public class BombAnimations : MonoBehaviour
{
    [System.NonSerialized] public string id;

    [SerializeField] private Animator animator;

    private void Awake() {
        if (!animator)
            animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        if (id != null)
            SetMyEvents();
    }

    private void OnDisable() {
        GameEvents.Bomb(id).onPatrol -= Walk;
        GameEvents.Bomb(id).onReachPatrolPoint -= Idle;
        GameEvents.Bomb(id).onChase -= Run;
    }

    private void SetMyEvents()
    {
        GameEvents.Bomb(id).onPatrol += Walk;
        GameEvents.Bomb(id).onReachPatrolPoint += Idle;
        GameEvents.Bomb(id).onChase += Run;
    }

    private void Idle() {
        animator.Play("Idle");
    }

    private void Walk() {
        animator.Play("Walk");
    }

    private void Run() {
        animator.Play("Run");
    }
}
