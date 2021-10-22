using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_To_Lobby : MonoBehaviour
{
    [SerializeField] private Object Menu_Scene;
    private DataKeeper data;
    private PlayerInputManager Input_manager;
    private ReadyButton ready;

    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<DataKeeper>();

        if (SceneManager.GetActiveScene().name == "New_lobby" && data.BackToLobby)
        {
            Input_manager = FindObjectOfType<PlayerInputManager>();
            ready = FindObjectOfType<ReadyButton>();

            if (data.BackToLobby) 
            {
                data.BackToLobby = false;
                GameObject.Find("menuCamera").SetActive(false);
                GameObject.Find("MenuSelector").SetActive(false);
                Input_manager.enabled = true;
                Input_manager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
                SpawnPlayers();


            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void BackToLobby() 
    {
        data = FindObjectOfType<DataKeeper>();
        data.BackToLobby = true;
        AudioManager.current.Stop("Game", "Music", "Game");
        SceneManager.LoadScene("New_lobby");
    }


    private void SpawnPlayers()
    {
        //var input2 = data.RetrivePlayerInput(0);
        //Debug.Log("Device:" + input2);

        int players_num = data.amount_players;
        //Player_Index_DreadZito dreadZito = FindObjectOfType<Player_Index_DreadZito>();

        for (int i = 0; i < players_num; i++)
        {
            var input = data.RetrivePlayerInput(i);
            var player = Input_manager.JoinPlayer(i, -1, null, input);
            player.gameObject.GetComponent<Player_SharedData>().MaterializeMe();
            //dreadZito.Players_FromGame(i);
        }

        Input_manager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        Invoke("Fix_Wea", 2);
    }

    private void Fix_Wea()
    {
        int players_num = data.amount_players;
        Player_Index_DreadZito dreadZito = FindObjectOfType<Player_Index_DreadZito>();

        for (int i = 0; i < players_num; i++)
            dreadZito.Players_FromGame(i);
        ready.Update_TotalPlayers(players_num);
    }
}
