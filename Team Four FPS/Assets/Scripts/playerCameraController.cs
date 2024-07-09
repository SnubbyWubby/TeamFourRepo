using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;

public class playerCameraController : MonoBehaviour 
{
    [SerializeField] int cameraSensitivity;
    [SerializeField] int cameraLockVertMin;
    [SerializeField] int cameraLockVertMax;

    [SerializeField] bool cameraInvertY;

    float cameraRotationX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.camEnd = new Vector3(Camera.main.transform.localPosition.x,
                                                  Camera.main.transform.localPosition.y + 5,
                                                  Camera.main.transform.localPosition.z - 7);

    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.stopCameraRotation)
        {
            float xMouse = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            float yMouse = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

            if (cameraInvertY)
            {
                cameraRotationX += yMouse;
            }
            else
            {
                cameraRotationX -= yMouse;
            }

            // Clamp The cameraRotationX On The X-Axis 

            cameraRotationX = Mathf.Clamp(cameraRotationX, cameraLockVertMin, cameraLockVertMax);

            // Rotate The Camera On The X-Axis 

            transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);

            // Rotate The Player On The Y-Axis 

            transform.parent.Rotate(Vector3.up * xMouse);
        }
        
    }
}