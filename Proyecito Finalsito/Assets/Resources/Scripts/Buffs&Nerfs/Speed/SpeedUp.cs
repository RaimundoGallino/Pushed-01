using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUp : BasePickUp
{
    [SerializeField] private float speedUpTime = 4f;
    [SerializeField] private float speedUpVelocity = 3f;
    private bool check = true;
    
    private void FixedUpdate()
    {
        if (player && check)
        {
            check = !check;
            var targetPlayerID = player.GetComponent<PlayerController>().id;
            GameEvents.Player(targetPlayerID).Damage.onSpeedUp(speedUpTime, speedUpVelocity);
            StartCoroutine(StartDestroyCountDown());
        }
    }
}
