using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassMyTrigger : MonoBehaviour
{
    [SerializeField] private Game_CoreLogic game;

    private void OnTriggerEnter(Collider other)
    {
        game.PlayerFall(other);
        Debug.Log(other);
    }
}
