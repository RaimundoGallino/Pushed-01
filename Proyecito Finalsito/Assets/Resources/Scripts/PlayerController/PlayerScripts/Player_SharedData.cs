using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using UnityEngine;

public class Player_SharedData : MonoBehaviour
{
    public int index = 10;
    public bool ready = false;

    private bool hold = false;

    private Player_Index_DreadZito IndexMaster;
    private SpawnPoint_System Spawn;

    private DataKeeper data;
    private ReadyButton Ready_Button;

    //This related to appear and disapperar animations
    [SerializeField] private Animator ani;
    [SerializeField] private VisualEffect vfx_materialize;
    [SerializeField] private VisualEffect vfx_dematerialize;
    [SerializeField] private GameObject ojos;
    [SerializeField] private GameObject cuerpo;
    [SerializeField] private GameObject diente;

    //SkinList
    [SerializeField] private List<Material> Skins = new List<Material>();


    //private void Awake()
    //{
    //    GameEvents.Game.OnEnterGame += RemovePlayerNuke;
    //}


    // Start is called before the first frame update
    void Start()
    {

        //DontDestroyOnLoad(gameObject);
        if (SceneManager.GetActiveScene().name == "New_lobby")
        {
            gameObject.GetComponent<PlayerPush>().CanPush = false;
            Request_ID();
            PutMeOnMySpawn();
            SetMySkin(index);
            MaterializeMe();
        }

        if (SceneManager.GetActiveScene().name == "Game_Scene") 
        {
            gameObject.GetComponent<PlayerPush>().CanPush = false;
            PutMeOnMySpawnInGame();
            hold = true;
        }
    }

    private void OnDestroy()
    {

        if (SceneManager.GetActiveScene().name == "New_lobby")
            Free_ID();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Request_ID() {
        IndexMaster = FindObjectOfType<Player_Index_DreadZito>();
        index = IndexMaster.GiveMeAnIndex();
    }

    public void Free_ID() {
        IndexMaster = FindObjectOfType<Player_Index_DreadZito>();
        if (IndexMaster != null)
            IndexMaster.ClearPlayerSlot(index);
    }


    public bool Toogle_Ready() {
        ready = !ready;
        return ready;
    }

    private void PutMeOnMySpawn() 
    {
        Spawn = FindObjectOfType<SpawnPoint_System>();
        gameObject.transform.position = Spawn.RetriveSpawnPoint(index);
    }


    private void PutMeOnMySpawnInGame()
    {
        Spawn = FindObjectOfType<SpawnPoint_System>();
        var idx = gameObject.GetComponent<PlayerInput>().playerIndex;
        gameObject.transform.position = Spawn.RetriveSpawnPoint(idx);
    }


    public void SetMySkin(int idx)
    {
        //Debug.Log(Skins[index]);
        //Debug.Log(gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true));
        //var mm = gameObject.GetComponentInChildren<SkinnedMeshRenderer>(true).materials;
        //foreach (Material m in mm)
        //Debug.Log(gameObject.transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>());
        var materials = gameObject.transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>().materials;
        materials[0] = Skins[idx];
        materials[1] = Skins[idx];
        gameObject.transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>().materials = materials;
        gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

    }

    //public void GiveMeMyControl(int idx)
    //{
    //    data = FindObjectOfType<DataKeeper>();
    //    //var man = FindObjectOfType<PlayerInputManager>();
    //    var input = data.RetrivePlayerInput(idx);
    //    //man.JoinPlayer(idx, -1, input.currentControlScheme, input.devices[0]);
    //    //gameObject.GetComponent<PlayerInput>().SwitchCurrentControlScheme(input.devices[0]);
    //}

    //public void GiveMeMyID(int idx)
    //{
    //    data = FindObjectOfType<DataKeeper>();
    //    index = data.RetrivePlayerIndex(idx);

    //}


    public void StopAnims() 
    {
        StopCoroutine("MaterializeMe");
        StopCoroutine("DematerializeMe");
        gameObject.GetComponent<PlayerMovement>().CanMoveTrue();
    }

    public void MaterializeMe() 
    {
        vfx_materialize.enabled = true;
        AudioManager.current.Play("Player", "DisolveEffect", "Disolve3");
        StartCoroutine("Materialize_Anim");
    }

    private IEnumerator Materialize_Anim()
    {
        yield return new WaitForSeconds(1.6f);
        ojos.SetActive(true);
        cuerpo.SetActive(true);
        diente.SetActive(true);
        ani.Play("SpawnAnimation");
        vfx_materialize.Stop();
        yield return new WaitForSeconds(2.2f);
        ani.Play("Blend Tree");
        if (hold)
            yield return new WaitForSeconds(4);

        gameObject.GetComponent<PlayerMovement>().CanMoveTrue();
        gameObject.GetComponent<PlayerPush>().CanPush = true;
    }

    public void DematerializeMe() 
    {
        //gameObject.GetComponent<PlayerController>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().CanMoveFalse();
        gameObject.GetComponent<PlayerPush>().CanPush = false;
        vfx_dematerialize.enabled = true;
        AudioManager.current.Play("Player", "Spawn", "SpawnSound1");
        StartCoroutine("Dematerialize_Anim");
    }

    private IEnumerator Dematerialize_Anim()
    {
        yield return new WaitForSeconds(1);
        ani.Play("Dissolveanimation");
        vfx_dematerialize.Stop();
    }


    public void PlayerNuke(PlayerInput input) 
    {
        var share_data = gameObject.GetComponent<Player_SharedData>();
        var idx = share_data.index;
        data = FindObjectOfType<DataKeeper>();
        Ready_Button = FindObjectOfType<ReadyButton>();
        Destroy(gameObject);
        data.RemoveMe(idx);
        Ready_Button.FromExitButton(share_data);
    }

    public void RemovePlayerNuke() 
    {
        gameObject.GetComponent<PlayerInput>().onDeviceLost -= PlayerNuke;
    }
}
