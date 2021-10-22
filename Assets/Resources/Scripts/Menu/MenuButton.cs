using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class MenuButton : MonoBehaviour
{
    public UnityEvent OnTrigger;
    public UnityEvent OnHover;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private List<GameObject> walls = new List<GameObject>();
    [SerializeField] private VisualEffect robot;


    private int pichu = 0;

    public void ButtonTriggerSound()
    {
        AudioManager.current.Play("LobbyUI", "ReadyButton", "ReadyButtonSound1");
    }

    public void Do_Push() {
        GameObject.Find("MenuSelector").SetActive(false);
        robot.enabled = true;
        //Dissolve robot effect sound
        AudioManager.current.Play("Player", "DisolveEffect", "Disolve3");
        StartCoroutine("DissolveRobot");
        Invoke("Enable_InputManager", 8);
    }

    public void Do_Abort() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif

    }

    public void Do_Settings() 
    {
        pichu++;

        if (pichu > 5) 
        {
            foreach (GameObject wall in walls) 
            {
                Material floor = wall.GetComponent<MeshRenderer>().materials[1];
                Material lines = wall.GetComponent<MeshRenderer>().materials[0];
                floor.color = Color.cyan;
                lines.color = Color.blue;
                wall.GetComponent<MeshRenderer>().materials[1] = floor;
                wall.GetComponent<MeshRenderer>().materials[0] = lines;

            }
        }
    }

    private void Enable_InputManager() {
        inputManager.enabled = true;
    }

    private IEnumerator DissolveRobot() 
    {
        yield return new WaitForSeconds(1.0f);
        animator.Play("Menu_to_lobby_anim");
        //yield return new WaitForSeconds(2);
        robot.Stop();
    }
}
