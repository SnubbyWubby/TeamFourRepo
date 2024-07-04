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
    [SerializeField] int maxArmorHP; 
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

    [SerializeField] AudioClip[] healthAudio;
    [SerializeField] float healthVolume;

    [SerializeField] AudioClip[] armorAudio;
    [SerializeField] float armorVolume;

    [SerializeField] AudioClip[] weaponAudio;
    [SerializeField] float weaponVolume;

    [SerializeField] AudioClip reloadAudio;
    [SerializeField] float reloadVolume;

    [Header("<=====PLAYER_MOVEMENT=====>")]

    [SerializeField] float speed;
    [SerializeField] float sprintModifier;

    [SerializeField] float slideDuration;
    [SerializeField] float slideModifier;

    [SerializeField] float crouchCenterYOffset;
    [SerializeField] float standingCenterYOffset;

    [Range(0.2f, 0.5f)] public float audioWalkTimer;
    [Range(0.01f, 0.1f)]public float audioRunTimer; 

    [Header("<=====PLAYER_GRENADES=====>")]

    [SerializeField] GameObject grenade;

    [SerializeField] int grenadeCount;
    [SerializeField] float grenadeReloadTime;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    bool isShooting;
    bool isCrouching;
    bool isSprinting;
    bool isSliding;
    bool isJumping;
    bool isStraight;
    bool isWalking;
    bool isRunning;
    bool isDamageHit;

    int originalHP;
    int armHP;
    int jumpCount;
    int selectedGun;

    float playerHeight;
    float crouchHeight; 

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

            if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].ammoCurr > 0 && !isShooting) // Shoot Weapons 
                StartCoroutine(shoot());

            if (Input.GetButtonDown("Grenade") && grenadeCount > 0) // Throw Grenade
                StartCoroutine(throwGrenade());

            if (Input.GetButtonDown("Crouch") && (!isSprinting || isSliding)) // Crouch
                crouch();

            if (Input.GetButtonDown("Crouch") && isSprinting && !isSliding) // Slide
                StartCoroutine(slide());

            if (Input.GetButtonDown("Reload") && gunList.Count > 0 && gunList[selectedGun].ammoCurr < gunList[selectedGun].ammoMax) // Reload Guns
            {
                plrAudio.PlayOneShot(reloadAudio, reloadVolume);
                gunList[selectedGun].ammoCurr = gunList[selectedGun].ammoMax;
                updatePlayerUI();
            }

            selectGun();
        }

        sprint();
    }

    #region Player Movement Functionality

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

    void crouch()
    {
        if (!isCrouching)
        {
            isCrouching = true;
            controller.height = crouchHeight;
            controller.center.Set(0f, crouchCenterYOffset, 0f);
        }
        else
        {
            isCrouching = false;
            controller.height = playerHeight;
            controller.center.Set(0f, standingCenterYOffset, 0f);
        }
    }

    IEnumerator slide()
    {
        float slideTime = slideDuration;

        // Go into crouch position, may need to change if we add animations
        crouch();

        isSliding = true;
        speed *= slideModifier;
        playerVelocity = new Vector3(moveDirection.x * speed, moveDirection.y, moveDirection.z * speed);
        controller.Move(playerVelocity * Time.deltaTime);

        yield return new WaitForSeconds(slideTime);

        // Stand back up and remove slideModifier
        //crouch();
        speed /= slideModifier;
        isSliding = false;
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

    #endregion

    #region Player Weapon Functionality
    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;

        updatePlayerUI();

        plrAudio.PlayOneShot(weaponAudio[Random.Range(0, weaponAudio.Length)], weaponVolume);

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

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
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

    IEnumerator throwGrenade()
    {
        // TODO: Write code to create better arc.

        Instantiate(grenade, throwPos.position, transform.rotation);

        yield return new WaitForSeconds(grenadeReloadTime);

        plrAudio.PlayOneShot(grenadeAudio[Random.Range(0, grenadeAudio.Length)], grenadeVolume);

        grenadeCount--;

        GameManager.Instance.updateGrenadeCount(-1);
    }
    #endregion

    #region Player Damage Functionality
    public void takeDamage(int amount)
    {
        if (armHP > 0)
        {
            int dmgRemainder = 0;

            if (amount > armHP)
            {
                dmgRemainder = amount % armHP;
            }

            armHP -= amount;
            HP -= dmgRemainder;
        }
        else
        {
            HP -= amount;
        }

        if (!isDamageHit) { StartCoroutine(PlayDamageHitSounds()); }

        updatePlayerUI();

        if (HP <= 0 && armHP <= 0)
        {
            GameManager.Instance.GameLoss("You Are Dead!");
        }
    }

    IEnumerator PlayDamageHitSounds() 
    {
        isDamageHit = true;

        plrAudio.PlayOneShot(damageAudio[Random.Range(0, damageAudio.Length)], damageVolume); 

        yield return new WaitForSeconds(audioDamageTimer);  

        isDamageHit = false; 
    }
    #endregion

    #region Player UI Functionality
    void updatePlayerUI()
    {
        float healthPercentage = (float)HP / originalHP;
        GameManager.Instance.playerHPBar.fillAmount = healthPercentage;

        if (healthPercentage > 1f)
            // Health is blue
            GameManager.Instance.playerHPBar.color = new Color(0.16f, 0.76f, 1f);
        else if (healthPercentage <= 1f && healthPercentage > 0.75f)
            // Health is green
            GameManager.Instance.playerHPBar.color = new Color(0.22f, 0.82f, 0f);
        else if (healthPercentage <= 0.75f && healthPercentage > 0.5f)
            // Health is yellow
            GameManager.Instance.playerHPBar.color = new Color(1f, 1f, 0f);
        else if (healthPercentage <= 0.5f && healthPercentage > 0.25f)
            // Health is orange
            GameManager.Instance.playerHPBar.color = new Color(0.92f, 0.63f, 0.06f);
        else
            // Health is red
            GameManager.Instance.playerHPBar.color = new Color(1f, 0.25f, 0.25f);


        if (armHP > 0)
        {
            float armorPercentage = (float)armHP / maxArmorHP;
            GameManager.Instance.plrArmorHPBar.fillAmount = armorPercentage;
        }
        else
        {
            GameManager.Instance.plrArmorHPBack.SetActive(false);
        }

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
    #endregion

    #region Player Pickup Functionality
    public void HealthPack(int amount)
    {
        HP += amount;

        if (HP > originalHP)
        {
            HP = originalHP;
        }

        // Player Will Regenerate Full Health When Walking Or Running Towards The Blue Health Sphere

        updatePlayerUI();

        plrAudio.PlayOneShot(healthAudio[Random.Range(0, healthAudio.Length)], healthVolume); 
    }

    public void ArmorShield(int amount)
    {
        armHP += amount;

        if (armHP > maxArmorHP)
        {
            armHP = maxArmorHP;
        }

        GameManager.Instance.plrArmorHPBack.SetActive(true);

        // Player Will Regenerate Full Armor When Walking Or Running Towards The Green Armor Shield 

        updatePlayerUI();

        plrAudio.PlayOneShot(armorAudio[Random.Range(0, armorAudio.Length)], armorVolume); 
    }
    #endregion
}