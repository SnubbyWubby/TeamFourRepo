using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;

public class InvertCam : MonoBehaviour
{


    public void InvertCamY()
    {
       GameManager.Instance.invertCam = !GameManager.Instance.invertCam;
    }
}
