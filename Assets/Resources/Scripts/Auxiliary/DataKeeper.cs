using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataKeeper : MonoBehaviour
{
    public static DataKeeper current;
    private InputDevice[] playerInputs = new InputDevice[4] {null, null, null, null};
    private int[] playerIndexs = new int[4] {10, 10, 10, 10};
    public int amount_players { get; set; }
    //[SerializeField] private GameObject PlayerManager;

    public bool BackToLobby = false;

    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(PlayerManager);
    }

    private void Awake()
    {
        if (current && current != this)
            Destroy(gameObject);
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayerIndex(int idx) 
    {
        if (playerIndexs[idx] == 10)
            playerIndexs[idx] = idx;
    }

    public void AddPlayerInput(InputDevice input, int idx) 
    {
        if (playerInputs[idx] == null)
            playerInputs[idx] = input;
    }

    public int RetrivePlayerIndex(int idx) 
    {
        return (playerIndexs[idx]);
    }

    public InputDevice RetrivePlayerInput(int idx) 
    {
        return (playerInputs[idx]);
    }

    public void RemoveMe(int idx) 
    {
        if (playerInputs[idx] != null)
            playerInputs[idx] = null;

        if (playerIndexs[idx] != 10)
            playerIndexs[idx] = 10;
    }
}
