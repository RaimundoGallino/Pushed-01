using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Index_DreadZito : MonoBehaviour
{
    //[SerializeField] private List<GameObject> Spawns = new List<GameObject>();
    //private List<bool> Players = new List<bool>() { };
    //Quitar Serialize
    [SerializeField] private bool[] Players = new bool[4] { false, false, false, false};
    private int Total_Players = 0;

    [SerializeField] private ReadyButton readyButton;


    //private int index = -1;


    public int GiveMeAnIndex() {
        int i = 0;
        while (i < 4) {
            if (Players[i] == false) {
                Players[i] = true;
                Total_Players++;
                Update_TotalPlayers_OnButton();
                return i;
            }
            i++;
        }
        return 15;
    }


    public void Players_FromGame(int idx) 
    {
        Players[idx] = true;
        //Total_Players++;
        Update_TotalPlayers_OnButton();
    }

    public void ClearPlayerSlot(int index) {
        Players[index] = false;
        Total_Players--;
        Update_TotalPlayers_OnButton();
    }

    public void Update_TotalPlayers_OnButton()
    {
        readyButton.Update_TotalPlayers(Total_Players);
    }

    public int Amount_Player() 
    {
        return Total_Players;
    }
}
