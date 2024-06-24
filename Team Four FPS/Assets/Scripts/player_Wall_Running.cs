using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_Wall_Running : MonoBehaviour
{
    // ||******************************[ First Script Of The Player's Wall Running Ability ]******************************|| 

    /*public bool truWallRun = true;
    public bool truWallRunning = false;

    public bool plrOnGround;

    [Range(1.0f, 15.0f)] public float wallRunSpeed;
    [Range(1.0f, 10f)] public float jumpForce;

    [Range(60, 180)] public int cameraView;

    public Rigidbody rbPlayer;

    public LayerMask groundFloor;

    public Transform plrCamera;

    // Start is called before the first frame update
    void Start()   
    {
        rbPlayer = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false; 
    }

    // Update Is Called Once Per Frame
    void Update()
    {
        if (truWallRunning && plrOnGround) 
        { 
            truWallRunning = false;
        }

        if (truWallRunning && rbPlayer.velocity.magnitude <= 30) 
        {
            rbPlayer.velocity += transform.forward * 100 * Time.deltaTime + transform.up * -0.1f;
        }

        PlayerWallJump();

        PlayerPointOfView(); 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Mathf.Abs(Vector3.Dot(collision.GetContact(0).normal, Vector3.up)) < 0.1f && truWallRun) 
        {
            // Player Will Start Wall Running

            Vector3 plrPosition = GameManager.Instance.Player.transform.position;  

            rbPlayer.velocity = new Vector3(plrPosition.x, plrPosition.y + 1, plrPosition.z) + transform.forward * 50; 

            truWallRunning = true;  
        }
    }

    private void PlayerWallJump() 
    {
        if (plrOnGround) 
        { 
            rbPlayer.velocity = new Vector3(rbPlayer.velocity.x, jumpForce, rbPlayer.velocity.z); 
        }

        // Player Will Jump Off Between Walls & Ground Floor 

        if (truWallRunning) 
        {
            if (Physics.Raycast(transform.position, transform.right, 1.0f, groundFloor)) 
            { 
                rbPlayer.velocity = new Vector3(0, jumpForce / 2, 0) + transform.right * -40; 
            }

            if (Physics.Raycast(transform.position, -transform.right, 1.0f, groundFloor))
            {
                rbPlayer.velocity = new Vector3(0, jumpForce / 2, 0) + transform.right * 40;
            }

            if (Physics.Raycast(transform.position, transform.forward, 1.0f, groundFloor))
            {
                rbPlayer.velocity = new Vector3(0, jumpForce / 2, 0) + transform.forward * -40;
            }
        }
    }

    void PlayerPointOfView()  
    {
        // Player Point Of View During Wall Running 

        if (truWallRunning)
        {
            plrCamera.GetComponent<Camera>().fieldOfView = cameraView;

            if (Physics.Raycast(transform.position, transform.right, 1.0f, groundFloor))
            {
                rbPlayer.velocity += transform.right * 0.1f;

                if (plrCamera.localEulerAngles.z < 15f || plrCamera.localEulerAngles.z > 345f)
                {
                    plrCamera.localEulerAngles += new Vector3(0, 0, 100f * Time.deltaTime);
                }
            }

            if (Physics.Raycast(transform.position, -transform.right, 1.0f, groundFloor))
            {
                if (plrCamera.localEulerAngles.z > 345f)
                {
                    plrCamera.localEulerAngles += new Vector3(0, 0, -100f * Time.deltaTime);
                }

                plrCamera.localEulerAngles += new Vector3(0, 0, -10f * Time.deltaTime);

                rbPlayer.velocity += transform.right * -0.1f;
            }
        }
        else 
        {
            plrCamera.GetComponent<Camera>().fieldOfView = cameraView;
        }
    }

    IEnumerator WallRunCoolDown() 
    {
        truWallRun = true;

        yield return new WaitForSeconds(wallRunSpeed); 

        truWallRun = false;
    }

    private void OnCollisionExit(Collision collision) 
    {
        truWallRunning = false; 

        StartCoroutine(WallRunCoolDown());
    }*/

    // ||******************************[ Second Script Of The Player's Wall Running Ability ]******************************|| 

    public LayerMask plrOnWall; 
    public LayerMask plrOnGround;

    public float wallRunForce;
    public float wallRunMaxTime; 
    public float wallRunTimer;
    public float wallClimbSpeed; 

    public KeyCode upRunKey = KeyCode.UpArrow;  
    public KeyCode downRunKey = KeyCode.DownArrow; 

    private bool upRunning;
    private bool downRunning;

    private float inputHorizontal;
    private float inputVertical;

    public float checkWallDistance; 
    public float minJumpHeight; 

    private RaycastHit hitWallLeft; 
    private RaycastHit hitWallRight; 

    private bool leftWall;
    private bool rightWall;

    public Transform orientation;

    public playerController plrMovement;

    public Rigidbody rgPlayer;

    // Start is called before the first frame update
    void Start()   
    {
        rgPlayer = GetComponent<Rigidbody>();

        plrMovement = GetComponent<playerController>(); 
    }

    // Update Is Called Once Per Frame
    void Update()
    {
        WallCheckDistance();

        StateUserMachine(); 
    }

    private void FixedUpdate()
    {
        if (plrMovement.truWallRun) { WallRunMovement(); }
    }

    private void WallCheckDistance() 
    {
        // Allows The Physics.Raycast() Method To Detect Any Walls Closer To The User/Player 

        rightWall = Physics.Raycast(transform.position, orientation.right, out hitWallRight, checkWallDistance, plrOnWall);

        leftWall = Physics.Raycast(transform.position, -orientation.right, out hitWallLeft, checkWallDistance, plrOnWall);
    }

    private bool CheckAboveGround() 
    {
        // Allows The Physics.Raycast() Method To Detect The User/Player Is Above Ground-Floor  

        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, plrOnGround);
    }

    private void StateUserMachine()  
    {
        // Entering The Horizontal & Vertical Inputs 

        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        upRunning = Input.GetKey(upRunKey);
        downRunning = Input.GetKey(downRunKey); 

        // Phase 1 - Player Wall-Running 

        if (leftWall || rightWall && inputVertical > 0 && CheckAboveGround())
        {
            // Player Starts Wall-Running 

            if (!plrMovement.truWallRun) { StartWallRun(); } 
        }
        else 
        {
            // Player Finishes Wall-Running 

            if (plrMovement.truWallRun) { StopWallRun(); } 
        }
    }

    private void StartWallRun() 
    {
        plrMovement.truWallRun = true;     
    }

    private void WallRunMovement() 
    {
        rgPlayer.useGravity = false; 

        rgPlayer.velocity = new Vector3(rgPlayer.velocity.x, 0f, rgPlayer.velocity.z);

        Vector3 normalWall = rightWall ? hitWallRight.normal : hitWallLeft.normal;

        Vector3 forwardWall = Vector3.Cross(normalWall, transform.up);

        if ((orientation.forward - forwardWall).magnitude > (orientation.forward - -forwardWall).magnitude) { forwardWall = -forwardWall; }

        // Forward Force Movement 

        rgPlayer.AddForce(forwardWall * wallRunForce, ForceMode.Force);

        // Up Or Down Forces Against The Walls

        if (upRunning) 
        {
            rgPlayer.velocity = new Vector3(rgPlayer.velocity.x, wallClimbSpeed, rgPlayer.velocity.z); 
        }

        if (downRunning)
        {
            rgPlayer.velocity = new Vector3(rgPlayer.velocity.x, -wallClimbSpeed, rgPlayer.velocity.z); 
        }

        // Pushing The Force Against The Walls 

        if (!(leftWall && inputHorizontal > 0) && !(rightWall && inputHorizontal < 0)) 
        {
            rgPlayer.AddForce(-normalWall * 100, ForceMode.Force); 
        }
    }

    private void StopWallRun() 
    {
        plrMovement.truWallRun = false;    
    }
}