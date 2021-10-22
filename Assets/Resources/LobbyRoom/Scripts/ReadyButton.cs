using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private List<GameObject> Lights = new List<GameObject>();
    public Material Ready_Material;
    public Material NotReady_Material;
    public ParticleSystem pressReadyPraticles;
    public VisualEffect buttonEffect;
    private bool ready = false;
    private bool start = false;
    public GameObject notReadyText;
    public UnityEvent pressReady;
    public UnityEvent pressNotReady;

    //Ready Status Check Related
    private GameObject player;
    private int Total_Players = 0;
    private List<Player_SharedData> Players_Ready = new List<Player_SharedData>();
    [SerializeField] private UnityEngine.Object GamePlay_Scene;

    //Countdown Related Things
    [SerializeField] private GameObject Countdown;

    //DataKeeper Related
    [SerializeField] private DataKeeper data;

    //Animation Related
    [SerializeField] private Animator animator;
    private VisualEffect circulitos;

    private void Start()
    {
        data = FindObjectOfType<DataKeeper>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            player = collision.gameObject;
            
        if (collision.gameObject.name == "Lower_limit")
        {

            if (player.GetComponent<Player_SharedData>().ready == false) 
            {
                player.GetComponent<Player_SharedData>().ready = true;
                int idx = player.GetComponent<Player_SharedData>().index;
                Lights[idx].GetComponent<MeshRenderer>().material = Ready_Material;
            }
            else
            {
                player.GetComponent<Player_SharedData>().ready = false;
                int idx = player.GetComponent<Player_SharedData>().index;
                Lights[idx].GetComponent<MeshRenderer>().material = NotReady_Material;
            }

            AudioManager.current.Play("LobbyUI", "ReadyButton", "ReadyButtonSound1");

            Update_StatusPlayers();
            Check_Ready();

            if (ready)
            {
                //ready = !ready;
                Ready(player);
            }
            else
            {
                //ready = !ready;
                notReady(player);
            }

            //Save index and control scheme of every player that hit the button
            data.AddPlayerIndex(player.GetComponent<Player_SharedData>().index);
            data.AddPlayerInput(player.GetComponent<PlayerInput>().devices[0], player.GetComponent<Player_SharedData>().index);
            data.amount_players = Total_Players;
        }
    }

    void Ready(GameObject player)
    {
        buttonEffect.Play();
        pressReadyPraticles.Play();
        pressReady.Invoke();
    }
    void notReady(GameObject player)
    {
        buttonEffect.Play();
        pressReadyPraticles.Play();
        pressNotReady.Invoke();
    }

    public void Update_TotalPlayers(int num)
    {
        Total_Players = num;
    }


    private void Update_StatusPlayers() 
    {   
        var All_Players = FindObjectsOfType<Player_SharedData>();
        foreach (Player_SharedData player in All_Players) {
            if (!Players_Ready.Contains(player))
                Players_Ready.Add(player);
        }
    }


    private void Check_Ready() 
    {
        int pichu = 0;

        foreach (Player_SharedData player in Players_Ready) {
            if (player.ready)
                pichu++;
        }

        if (pichu == Total_Players && Total_Players > 1)
        {
            ready = true;
            start = true;
        }
        else
        {
            if (start)
            {
                StopCoroutine("ToGameScene");
                Play_Player_Anim("Stop");
                Countdown.GetComponent<CountDown>().Stop_Countdown();
            }
            ready = false;
            start = false;
            
            
        }

        if (start)
        {
            Countdown.GetComponent<CountDown>().Start_Countdown();
            //Invoke("ToGameScene", 4);
            StartCoroutine("ToGameScene");
            
        }
    }

    private IEnumerator ToGameScene() {
        //animator = player.GetComponentInChildren<Animator>();
        //circulitos = player.GetComponentInChildren<VisualEffect>();
        //circulitos.enabled = true;
        //animator.Play("Dissolveanimation");
        float time = Countdown.GetComponent<CountDown>().Time_Countdown();
        yield return new WaitForSeconds(time);
        Countdown.GetComponent<CountDown>().Stop_Countdown();
        Play_Player_Anim("Dematerialize");
        yield return new WaitForSeconds(3);
        //GameEvents.Game.OnEnterGame();
        AudioManager.current.Stop("Game", "Music", "Lobby");
        SceneManager.LoadScene("Game_Scene");
    }


    public void FromExitButton(Player_SharedData share_data) 
    {
        Lights[share_data.index].GetComponent<MeshRenderer>().material = NotReady_Material;
        Players_Ready.Remove(share_data);
        if (start)
        {
            StopCoroutine("ToGameScene");
            Countdown.GetComponent<CountDown>().Stop_Countdown();
        }
        Total_Players--;
        if (Total_Players > 0) 
        {
            //Update_StatusPlayers();
            Check_Ready();

            if (ready)
            {
                //ready = !ready;
                Ready(player);
            }
            else
            {
                //ready = !ready;
                notReady(player);
            }
        }
        else
            notReady(player);
    }


    private void Play_Player_Anim(string anim) 
    {
        foreach (Player_SharedData player in Players_Ready) 
        {
            if (anim == "Materialize") 
            {
                player.MaterializeMe();
            }

            if (anim == "Dematerialize")
            {
                player.DematerializeMe();
            }

            if (anim == "Stop")
            {
                player.StopAnims();
            }
        }
    }


}