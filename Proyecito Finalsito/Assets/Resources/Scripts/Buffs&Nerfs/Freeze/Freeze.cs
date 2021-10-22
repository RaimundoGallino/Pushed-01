using UnityEngine;

public class Freeze : BasePickUp
{
    [SerializeField] private float freezeTime = 4f;
    private bool check = true;

    private void FixedUpdate()
    {
        if (player && check)
        {   
            check = !check;
            var targetPlayerID = player.GetComponent<PlayerController>().id;
            GameEvents.Player(targetPlayerID).Damage.onFreeze(freezeTime);
            StartCoroutine(StartDestroyCountDown());
        }
    }
}
