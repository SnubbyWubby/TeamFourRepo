using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FramesPerSeconds : MonoBehaviour
{
    [Header("<=====FRAMES_PER_SECONDS_DISPLAY=====>")]

    public TextMeshProUGUI textFPS;

    [SerializeField] GameObject fpsText; 

    [Range(0.1f, 2.0f)] public float pollTime;  

    private float usrTime;
    private int countFrames; 

    // Update is called once per frame
    void Update() 
    {
        ShowGameFPS();
    }

    public void ShowGameFPS() 
    {
        if (Input.GetButton("TextFPS"))
        {
            // Press & Hold The TAB Button To View The Game's Frames Per Seconds

            fpsText.SetActive(true);

            usrTime += Time.deltaTime;

            countFrames++;

            if (usrTime >= pollTime)
            {
                int frRate = Mathf.RoundToInt(countFrames / usrTime);

                textFPS.text = frRate.ToString() + " FPS";

                usrTime -= pollTime;

                countFrames = 0;
            }
        }
        else if (Input.GetButtonUp("TextFPS"))
        {
            fpsText.SetActive(false);
        }
    }
}