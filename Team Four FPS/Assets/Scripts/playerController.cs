using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class playerController : MonoBehaviour, IDamage
{
    [Header("<=====COMPONENTS=====>")]

    [SerializeField] CharacterController controller;
    [SerializeField] Transform throwPos;
    [SerializeField] Transform feetPos;
    [SerializeField] AudioSource plrAudio;

    [Header("<=====PLAYER_STATS=====>")] 

    [SerializeField] int HP;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [Range(0.1f, 0.5f)] public float audioDamageTimer; 

    [Header("<=====PLAYER_GUNS=====>")]

    [SerializeField] List<gunStats> gunList = new List<gunStats>();

    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject flashMuzzle;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    [Header("<=====PLAYER_AUDIO=====>")]

    [SerializeField] AudioClip[] jumpAudio;
    [SerializeField] float jumpVolume;

    [SerializeField] AudioClip[] damageAudio; 
    [SerializeField] float damageVolume;

    [SerializeField] AudioClip[] movementAudio; 
    [SerializeField] float movementVolume;

    [SerializeField] AudioClip[] grenadeAudio;
    [SerializeField] float grenadeVolume;

    [Header("<=====PLAYER_MOVEMENT=====>")]

    [SerializeField] float speed;
    [SerializeField] float sprintModifier;

    [SerializeField] float slideDuration;
    [SerializeField] float slideModifier;

    [SerializeField] float crouchCenterYOffset;
    [SerializeField] float standingCenterYOffset;

    [SerializeField] float wallCheckDistance;
    [SerializeField] float wallDescendModifier;

    [Range(0.2f, 0.5f)] public float audioWalkTimer;
    [Range(0.01f, 0.1f)]public float audioRunTimer; 

    public bool truWallRun;
    public float wallRunSpeed;

    public StateMovement plrStateMoving;

    [Header("<=====PLAYER_GRENADES=====>")]

    [SerializeField] GameObject grenade;

    [SerializeField] int grenadeCount;
    [SerializeField] float grenadeReloadTime;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    bool isShooting;
    bool isCrouching;
    bool isSprinting;
    bool isJumping;
    bool isWallRunning;
    bool isStraight;
    bool isWalking;
    bool isRunning;
    bool isDamageHit; 

    int originalHP;
    int jumpCount;
    int selectedGun;

    float playerHeight;
    float crouchHeight;
    float plrWallRunTimer;

    public enum StateMovement { plrWallRunning }

    // Start is called before the first frame update
    void Start()
    {
        originalHP = HP;
        isStraight = true;

        updatePlayerUI();

        playerHeight = controller.height;
        crouchHeight = controller.height / 2;

        GameManager.Instance.updateGrenadeCount(grenadeCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isPaused)
        {
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);

            movement();

            if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].ammoCurr > 0 && !isShooting)
                StartCoroutine(shoot());

            if (Input.GetButtonDown("Grenade") && grenadeCount > 0)
                StartCoroutine(throwGrenade());

            if (Input.GetButtonDown("Crouch") && !isSprinting) // Crouch
                crouch();

            if (Input.GetButtonDown("Crouch") && isSprinting) // Slide
                StartCoroutine(slide());

            /*if (Input.GetButtonDown("Jump") && !isWallRunning)
                wallRun();*/

            selectGun();
        }

        sprint();

        // if (Input.GetButtonDown("Jump") && truWallRun) // Wall Run
            // StartCoroutine(PlayerWallRun());

        //wallRun();
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;

        updatePlayerUI();

        Debug.Log("Gun Added");

        shootDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootDistance = gun.shootDistance;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)
        {
            selectedGun++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
        {
            selectedGun--;
            changeGun();
        }
    }

    void changeGun()
    {
        updatePlayerUI();

        shootDamage = gunList[selectedGun].shootDamage;
        shootRate = gunList[selectedGun].shootRate;
        shootDistance = gunList[selectedGun].shootDistance;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].GetComponent<MeshRenderer>().sharedMaterial;
    }

    void movement()
    {
        // Check if player is touching the ground
        if (controller.isGrounded)
        {
            isJumping = false;
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        // Get input from player, create vector, then move player.
        moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            isJumping = true;

            plrAudio.PlayOneShot(jumpAudio[Random.Range(0, jumpAudio.Length)], jumpVolume); 

            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (controller.isGrounded && moveDirection.magnitude > audioWalkTimer && !isWalking) 
        { StartCoroutine(PlayMovementAudio()); } 
    }

    void sprint()
    {
        // Sprint = left shift
        if (Input.GetButtonDown("Sprint"))
        {
            // Break out of crouch if crouching
            if (isCrouching)
            {
                crouch();
            }

            speed *= sprintModifier;
            isSprinting = true;

            isRunning = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintModifier;
            isSprinting = false;

            isRunning = false;
        }
    }

    IEnumerator PlayMovementAudio() 
    {
        isWalking = true;

        plrAudio.PlayOneShot(movementAudio[Random.Range(0, movementAudio.Length)], movementVolume); 

        if (!isRunning)
        {
            yield return new WaitForSeconds(audioWalkTimer);
        }
        else 
        { 
            yield return new WaitForSeconds(audioRunTimer);
        }

        isWalking = false;  
    }

    IEnumerator shoot()
    {
        isShooting = true;

        plrAudio.PlayOneShot(gunList[selectedGun].shootSound, gunList[selectedGun].audioVolume); 

        gunList[selectedGun].ammoCurr--;

        updatePlayerUI();

        StartCoroutine(MuzzleFlash());

        RaycastHit hit;

        // Use raycast and get gameobject that is hit
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance))
        {
            Debug.Log(hit.transform.name);
            // Get IDamage component if gameobject is damageable
            IDamage damageable = hit.collider.GetComponent<IDamage>();

            if (hit.transform != transform && damageable != null)
            {
                damageable.takeDamage(shootDamage);
            }
            else
            {
                Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        if (!isDamageHit) { StartCoroutine(PlayDamageHitSounds()); }

        updatePlayerUI();

        if (HP <= 0)
        {
            GameManager.Instance.GameLoss();
        }
    }

    IEnumerator PlayDamageHitSounds() 
    {
        isDamageHit = true;

        plrAudio.PlayOneShot(damageAudio[Random.Range(0, damageAudio.Length)], damageVolume); 

        yield return new WaitForSeconds(audioDamageTimer);  

        isDamageHit = false; 
    }

    void updatePlayerUI()
    {
        float healthPercentage = (float)HP / originalHP;
        GameManager.Instance.playerHPBar.fillAmount = healthPercentage;

        if (healthPercentage > 0.75f)
            // Health is green
            GameManager.Instance.playerHPBar.color = new Color(0.22f, 0.82f, 0);
        else if (healthPercentage <= 0.75f && healthPercentage > 0.5f)
            // Health is yellow
            GameManager.Instance.playerHPBar.color = new Color(1f, 1f, 0);
        else if (healthPercentage <= 0.5f && healthPercentage > 0.25f)
            // Health is orange
            GameManager.Instance.playerHPBar.color = new Color(0.92f, 0.63f, 0.06f);
        else
            // Health is red
            GameManager.Instance.playerHPBar.color = new Color(1f, 0.25f, 0.25f);

        if (gunList.Count > 0)
        {
            GameManager.Instance.ammoCurrent.text = gunList[selectedGun].ammoCurr.ToString("F0");
            GameManager.Instance.ammoMaximum.text = gunList[selectedGun].ammoMax.ToString("F0");
        }
    }

    IEnumerator MuzzleFlash()
    {
        flashMuzzle.SetActive(true);

        yield return new WaitForSeconds(0.125f);

        flashMuzzle.SetActive(false);
    }

    public void HealthPack(int amount)
    {
        HP += amount;

        // Player Will Regenerate Full Health When Walking Or Running Towards The Blue Health Sphere

        GameManager.Instance.playerHPBar.fillAmount = (float)HP * originalHP;

        float healthPercentage = (float)HP * originalHP;

        GameManager.Instance.playerHPBar.fillAmount = healthPercentage;

        if (healthPercentage > 0.75f)
        {
            // Player's Health Bar Will Regenerate Back To Green Fill Color 

            GameManager.Instance.playerHPBar.color = new Color(0.22f, 0.82f, 0);
        }
    }

    public void ArmorShield(int amount)
    {
        HP += amount;

        // Player Will Regenerate Full Armor When Walking Or Running Towards The Green Armor Shield 

        GameManager.Instance.playerHPBar.fillAmount = (float)HP * originalHP;

        float healthPercentage = (float)HP * originalHP;

        GameManager.Instance.playerHPBar.fillAmount = healthPercentage;

        if (healthPercentage > 0.75f)
        {
            // Player's Health Bar Will Regenerate Back To Green Fill Color 

            GameManager.Instance.playerHPBar.color = new Color(0.22f, 0.82f, 0);
        }
    }

    void crouch()
    {
        if (!isCrouching)
        {
            Debug.Log("crouching");
            isCrouching = true;
            controller.height = crouchHeight;
            controller.center.Set(0f, crouchCenterYOffset, 0f);
        }
        else
        {
            Debug.Log("standing");
            isCrouching = false;
            controller.height = playerHeight;
            controller.center.Set(0f, standingCenterYOffset, 0f);
        }
    }

    IEnumerator PlayerWallRun()
    {
        // User Input (Manager) Button Either "Q" Or "E" To Activate Wall Run

        truWallRun = true;

        if (truWallRun)
        {
            plrStateMoving = StateMovement.plrWallRunning;

            speed = wallRunSpeed;

            Debug.Log("Wall-Running!");
        }

        yield return new WaitForSeconds(plrWallRunTimer);

        truWallRun = false;
    }

    IEnumerator slide()
    {
        Debug.Log("Weeeee!");
        float slideTime = slideDuration;

        // Go into crouch position, may need to change if we add animations
        crouch();

        speed *= slideModifier;
        playerVelocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);
        controller.Move(playerVelocity * Time.deltaTime);

        yield return new WaitForSeconds(slideTime);

        // Stand back up and remove slideModifier
        crouch();
        speed /= slideModifier;
    }

    IEnumerator throwGrenade()
    {
        // TODO: Write code to create better arc.
        Instantiate(grenade, throwPos.position, transform.rotation);

        yield return new WaitForSeconds(grenadeReloadTime);

        plrAudio.PlayOneShot(grenadeAudio[Random.Range(0, grenadeAudio.Length)], grenadeVolume);

        grenadeCount--;

        GameManager.Instance.updateGrenadeCount(-1);
    }

    public void wallRun()
    {
        if (controller.isGrounded && !isStraight)
        {
            Debug.Log(transform.eulerAngles.z);
            if (transform.eulerAngles.z != 0)
            {
                transform.Rotate(Vector3.forward * -transform.eulerAngles.z);
            }

                isStraight = true;
        }
        //RaycastHit hitWall;
        Debug.DrawRay(feetPos.transform.position, -transform.right * 2, Color.red);
        Debug.DrawRay(feetPos.transform.position, transform.right * 2, Color.blue);

        if (isJumping)
        {
            Collider[] hits = Physics.OverlapSphere(feetPos.transform.position, 2f);
            foreach (Collider hit in hits)
            {
                if (hit.transform != transform && hit.CompareTag("Wallrunnable"))
                {
                    if (gameObject.transform.position.y >= 4.5f)
                    {
                        playerVelocity.y = 0f;
                        
                        if (hit.transform.position.x > gameObject.transform.position.x && isStraight)
                        {
                            isStraight = false;
                            transform.Rotate(Vector3.forward * -30f);
                        }
                        else if (hit.transform.position.x < gameObject.transform.position.x && isStraight)
                        {
                            isStraight = false;
                            transform.Rotate(Vector3.forward * 30f);
                        }
                    }

                    playerVelocity.y -= gravity * wallDescendModifier * Time.deltaTime;
                }
            }
            /* 
            if (Physics.Raycast(feetPos.transform.position, -transform.right, out leftWall, wallCheckDistance))
            {
                if (leftWall.collider.CompareTag("Wallrunnable"))
                {
                    if (gameObject.transform.position.y >= 4.5f)
                    {
                        playerVelocity.y = 0f;
                    }

                    playerVelocity.y -= gravity * wallDescendModifier * Time.deltaTime;
                }
            }
            else if (Physics.Raycast(feetPos.transform.position, transform.right, out rightWall, wallCheckDistance))
            {
                if (rightWall.collider.CompareTag("Wallrunnable"))
                {
                    if (gameObject.transform.position.y >= 4.5f)
                    {
                        playerVelocity.y = 0f;
                        transform.Rotate(Vector3.forward * 30f);
                    }

                    playerVelocity.y -= gravity * wallDescendModifier * Time.deltaTime;
                }
            }
            */
        }
    }
}