using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;

public class OutofBounds : MonoBehaviour
{
    [Header("<=====OUT_OF_BOUNDS_AUDIO=====>")]

    [SerializeField] AudioSource otbAudio;

    [SerializeField] AudioClip[] outBoundAudio;
    [SerializeField] float outBoundVolume;

    [Header("<=====OUT_OF_BOUNDS_STATS=====>")]

    [SerializeField] int maxDuration;

    float timeRemaining = 0f;
    bool timerIsRunning;

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            GameManager.Instance.updateOutofBoundsDisplay(timeRemaining);

            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;

                otbAudio.PlayOneShot(outBoundAudio[Random.Range(0, outBoundAudio.Length)], outBoundVolume);
            }
            else
            {
                otbAudio.Stop(); 

                timeRemaining = 0;
                timerIsRunning = false;
                GameManager.Instance.GameLoss("You left the battle field!");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OutofBoundsToggle(true);
            timeRemaining = maxDuration;
            timerIsRunning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        otbAudio.Stop();

        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OutofBoundsToggle(false);
            timerIsRunning = false;
            timeRemaining = maxDuration;
        }
    }
}