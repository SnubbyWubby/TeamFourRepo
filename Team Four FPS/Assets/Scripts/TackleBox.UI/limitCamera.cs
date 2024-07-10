using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class limitCamera : MonoBehaviour
{
    [Header("<=====MINI_MAP_POV=====>")]

    public GameObject plrView;

    [Range(10, 50)] public int yAxis;  

    private void ViewUpdate() 
    {
        transform.position = new Vector3(plrView.transform.position.x, yAxis, plrView.transform.position.z); 
    }
}
