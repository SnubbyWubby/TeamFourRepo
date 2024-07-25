using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;


public class playerCameraController : MonoBehaviour 
{
    [SerializeField] int cameraSensitivity;
    [SerializeField] int cameraLockVertMin;
    [SerializeField] int cameraLockVertMax;

    

    float cameraRotationX;
    float cameraFov;
    float xChange = 0;
    float yChange = 0;
    public float[] sprayPattern = new float[5];
    public int sprayIter;
    public bool invertCamY;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0; i<sprayPattern.Length; i++)
        {
            
            sprayPattern[i] = (-0.05f * Mathf.Log10(i + 1)) + .05f;
        }
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.camEnd = new Vector3(Camera.main.transform.localPosition.x,
                                                  Camera.main.transform.localPosition.y + 5,
                                                  Camera.main.transform.localPosition.z - 7);
        cameraFov = Camera.main.fieldOfView;
        
    }

    // Update is called once per frame
    void Update()
    {
        invertCamY = PlayerPrefs.GetFloat("MasterInvertY") == 1.0f;
        if (!GameManager.Instance.stopCameraRotation)
        {
            float xMouse = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
            float yMouse = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;

            if (GameManager.Instance.playerShot && !GameManager.Instance.isPaused)
            {
                 xChange = xMouse;
                 yChange = yMouse;
                if(!GameManager.Instance.PlayerScript.resetRecoil)
                {
                    sprayIter = 0;
                }
                xMouse += (GameManager.Instance.PlayerScript.gunList[GameManager.Instance.PlayerScript.selectedGun].recoilAmount) + sprayPattern[sprayIter];
                yMouse += (GameManager.Instance.PlayerScript.gunList[GameManager.Instance.PlayerScript.selectedGun].recoilAmount) + sprayPattern[sprayIter];
                if (sprayIter < 4)
                {
                    sprayIter++;
                }
                
            }

            if (invertCamY)
            {
                cameraRotationX += yMouse;
            }
            else
            {
                cameraRotationX -= yMouse;
            }
            aimDown();
        

            

            // Clamp The cameraRotationX On The X-Axis 

            cameraRotationX = Mathf.Clamp(cameraRotationX, cameraLockVertMin, cameraLockVertMax);

            // Rotate The Camera On The X-Axis 

            transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);

            // Rotate The Player On The Y-Axis 

            transform.parent.Rotate(Vector3.up * xMouse);
            //Debug.Log(xMouse);
            //Debug.Log(yChange); 
           
            xChange = 0;
            yChange = 0;    
        }
        
    }
    public void aimDown()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            Camera.main.fieldOfView = 40f;
            playerController speed = GameManager.Instance.PlayerScript;
            speed.speed /= 2;
        }
        else if (Input.GetButtonUp("Fire3"))
        {
            Camera.main.fieldOfView = cameraFov;
            playerController speed = GameManager.Instance.PlayerScript;
            speed.speed *= 2;
        }
        

    }
    
}