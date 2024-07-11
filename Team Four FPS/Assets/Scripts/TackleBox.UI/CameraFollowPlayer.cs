using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform plrTarget;
    
    [SerializeField] private float offSetX;
    [SerializeField] private float offSetZ; 
    [SerializeField] private float spdLerp;

    private void CameraUpdate() 
    {
        transform.position = Vector3.Lerp(transform.position, 
                             new Vector3(plrTarget.position.x + offSetX, 
                             transform.position.y,plrTarget.position.z + offSetZ), spdLerp); 
    }
}
