using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraLife : BasePickUp
{
    private bool check = true;

    private void FixedUpdate()
    {
        if (player && check)
        {
            check = !check;
            var targetPlayerID = player.GetComponent<PlayerController>().id;
            GameEvents.Player(targetPlayerID).onExtraLife();
            StartCoroutine(StartDestroyCountDown());
        }
    }
}
