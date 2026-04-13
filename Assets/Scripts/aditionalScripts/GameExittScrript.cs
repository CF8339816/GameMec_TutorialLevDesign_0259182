using System.Collections;
using TMPro;
using UnityEngine;


public class GameOverManager : MonoBehaviour
{
   // public float ExitDelay = 5f;
    public TextMeshProUGUI ExitText;
    //private float Countdown;
    private bool isExiting = false;


    void Awake()
    {
        //Countdown = ExitDelay;

        if (ExitText != null)
        {
            ExitText.text = "";
        }
    }

    //void SetExitCountdownText()
    //{
    //    exitCountdownText.text = "Game Exit : " + Countdown.ToString() ; // sets count to output to string
    //}


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isExiting)
        {

            isExiting = true;
            StartCoroutine(StartExitCountdown());

        }
            
           
    }
    IEnumerator StartExitCountdown()
    {
        float timer = 5f;
        

        while (timer > 0)
        {
            if (ExitText != null)
            {
                ExitText.text = "Exiting in: " + Mathf.Ceil(timer).ToString();
            }
            else
            {
                Debug.LogWarning("UI Text is missing from GameOverManager Inspector! Still counting down...");
            }

            timer -= Time.deltaTime;
            yield return null; 
        }
        //while (Countdown > 0)
        //{
        //    ExitText.text = "Game Exit in: " + Mathf.Ceil(Countdown).ToString();// displays the countdown output in an always rounded up to whole int
        //    yield return null;

        //    Countdown -= Time.deltaTime;
        //}

        //ExitText.text = "Exiting...";
        ExitGame();
    }



    public void ExitGame()
    {
        Debug.Log("Exiting Game...");

        // Works in a built application
        Application.Quit();

        // Works inside the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
