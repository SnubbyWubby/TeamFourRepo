using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutofBounds : MonoBehaviour
{
    [Header("<=====OUT_OF_BOUNDS_STATS=====>")]

    [SerializeField] int maxDuration;

    float timeRemaining = 0f;
    bool timerIsRunning = false;

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            GameManager.Instance.updateOutofBoundsDisplay(timeRemaining);
            if (timeRemaining > 0)
                timeRemaining -= Time.deltaTime;
            else
            {
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
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OutofBoundsToggle(false);
            timerIsRunning = false;
            timeRemaining = maxDuration;
        }
    }
}