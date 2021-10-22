using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class Game_CoreLogic : MonoBehaviour
{
    [Header("CountDowns")]
    [SerializeField] private GameObject Initial_Countdown;
    [SerializeField] private GameObject Final_Countdown;

    [SerializeField] private TextMeshProUGUI Win_Text;

    private DataKeeper data;
    private int players_alive;

    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<DataKeeper>();
        players_alive = data.amount_players;
        //Initial_Countdown.GetComponent<CountDown>().Start_InitialCountdown();
        StartCoroutine("HoldPlayersInStart");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator HoldPlayersInStart() 
    {
        yield return new WaitForSeconds(4);
        Initial_Countdown.GetComponent<CountDown>().Start_InitialCountdown();
    }

    public void PlayerFall(Collider col) 
    {
        if (col.gameObject.CompareTag("Player")) 
        {
            var playerId = col.GetComponent<PlayerController>().id;
            if (GameEvents.Player(playerId).getExtraLife() == true)
                return;

            FindObjectOfType<CinemachineTargetGroup>().RemoveMember(col.transform);
            players_alive--;
            Destroy(col.gameObject);
            CheckWinner(col.gameObject);
        }
    }


    private void CheckWinner(GameObject player) 
    {
        if (players_alive == 1)
        {
            Win_Text.enabled = true;
            Final_Countdown.SetActive(true);
            Final_Countdown.GetComponent<CountDown>().Start_FinalCountdown();
        }
        
    }
}
