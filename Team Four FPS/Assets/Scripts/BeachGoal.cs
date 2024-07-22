using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.SaveSystem;
using UnityEngine;

public class BeachGoal : MonoBehaviour, ILevelGoal
{
    public bool updateGameGoal(int enemies)
    {
        //Debug.Log(enemies);
        if (enemies <= 0)
        {
            StartCoroutine(GameManager.Instance.levelTrans());
            return true;
        }

        return false;
    }
}
