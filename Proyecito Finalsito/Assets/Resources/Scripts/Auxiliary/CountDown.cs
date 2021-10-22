using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{

    [SerializeField] private GameObject Countdown;
    [SerializeField] private TextMeshProUGUI Countdown_Text;
    [SerializeField] private int Time_To_Countdown;
    [SerializeField] private PlayerInputManager manager;


    private int timeRemaining;
    private bool final = false;
    private bool start = false;

    private IEnumerator CountdownTimer() 
    {
        while (timeRemaining > 0) 
        {
            Countdown_Text.text = timeRemaining.ToString();
            yield return new WaitForSeconds(1);
            timeRemaining--;
        }

        if (start)
        {
            start = false;
            Countdown_Text.text = "GO!";
            GameEvents.Game.onGameReady?.Invoke();
            Invoke("Stop_Countdown", 1);
        }
        else 
        {
            Countdown_Text.text = timeRemaining.ToString();
        }

        if (final) 
        {
            final = false;
            SceneManager.LoadScene("Game_Scene");
        }

    }


    public void Start_Countdown() 
    {
        Countdown.SetActive(true);
        timeRemaining = Time_To_Countdown;
        AudioManager.current.Play("Countdown", "Countdown", "Countdown");
        StartCoroutine("CountdownTimer");
        if (manager != null)
            manager.DisableJoining();
    }

    public void Stop_Countdown() 
    {
        Countdown.SetActive(false);
        timeRemaining = Time_To_Countdown;
        AudioManager.current.Stop("Countdown", "Countdown", "Countdown");
        if (manager != null)
            manager.EnableJoining();
    }


    public void Start_FinalCountdown()
    {
        Countdown.SetActive(true);
        timeRemaining = Time_To_Countdown;
        StartCoroutine("CountdownTimer");
        final = true;
    }

    public void Start_InitialCountdown()
    {
        Countdown.SetActive(true);
        timeRemaining = Time_To_Countdown;
        AudioManager.current.Play("Countdown", "Countdown", "Countdown");
        StartCoroutine("CountdownTimer");
        start = true;
    }

    public float Time_Countdown()
    {
        return Time_To_Countdown;
    }

}
