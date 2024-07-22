using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.SaveSystem;
using UnityEngine;

public class TownGoal : MonoBehaviour, ILevelGoal
{
    public bool updateGameGoal(int enemies)
    {
        if (enemies <= 0)
        {
            StartCoroutine(GameManager.Instance.levelTrans());
            return true;
        }

        return false;
    }
}