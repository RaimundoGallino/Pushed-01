using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;

public class InitPlayers : MonoBehaviour
{
    private PlayerInputManager manager;
    private DataKeeper data;
    private int players_num;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<PlayerInputManager>();
        data = FindObjectOfType<DataKeeper>();
        players_num = data.amount_players;
        AudioManager.current.Play("Game", "Music", "Game");
        SpawnPlayers();
    }

    private void SpawnPlayers() 
    {
        for (int i = 0; i < players_num; i++)
        {
            var input = data.RetrivePlayerInput(i);
            var player = manager.JoinPlayer(i, -1, null, input);
            player.gameObject.GetComponent<Player_SharedData>().MaterializeMe();
            player.gameObject.GetComponent<Player_SharedData>().SetMySkin(i);
            FindObjectOfType<CinemachineTargetGroup>().AddMember(player.transform, 1, 2);
        }
    }

}
