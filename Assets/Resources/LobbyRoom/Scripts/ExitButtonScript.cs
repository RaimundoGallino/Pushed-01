using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitButtonScript : MonoBehaviour
{

    public UnityEvent holdingExit;
    public UnityEvent getoff;

    //[SerializeField] private Material active;
    //[SerializeField] private Material inactive;
    private Player_Index_DreadZito dreadZito;
    //private ReadyButton Ready_Button;
    //public GameObject player;
    //private List<PlayerController> activePlayers = new List<PlayerController>();

    // Start is called before the first frame update
    void Start()
    {
        dreadZito = FindObjectOfType<Player_Index_DreadZito>();
       //Ready_Button = FindObjectOfType<ReadyButton>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("me estan apretando uwu");
            StartCoroutine(waitingToExit(collider.gameObject));
            //gameObject.GetComponent<MeshRenderer>().materials[1] = inactive;
            Debug.Log(gameObject.GetComponent<MeshRenderer>().materials[1].name);
        }
    }
    void OnTriggerExit (Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            getoff.Invoke();
            StopAllCoroutines();
            //gameObject.GetComponent<MeshRenderer>().materials[1] = active;
        }
    }
    private IEnumerator waitingToExit(GameObject player)
    {
        holdingExit.Invoke();
        yield return new WaitForSeconds(4);
        // logica network
        //player.SetActive(false);
        //Debug.Log("GO:" + player.name);
        ///Remove Player from all data lists and refresh status to start the match
        //var share_data = player.GetComponent<Player_SharedData>();
        //var idx = share_data.index;
        //Destroy(player);
        //data.RemoveMe(idx);
        //Ready_Button.FromExitButton(share_data);
        player.GetComponent<Player_SharedData>().PlayerNuke(new PlayerInput());
        ///
        getoff.Invoke();
        if (dreadZito.Amount_Player() == 1)
            SceneManager.LoadScene("New_lobby");
        Debug.Log("me Fui");
    }
}
