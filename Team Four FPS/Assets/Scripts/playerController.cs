using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TackleBox;
using TackleBox.Audio;
using TackleBox.Guns;
using Unity.VisualScripting;

using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;


public class playerController : MonoBehaviour, IDamage
{
    [Header("<=====COMPONENTS=====>")]

    [SerializeField] CharacterController controller;
    [SerializeField] Transform throwPos;
    [SerializeField] Transform feetPos;
    [SerializeField] public AudioSource plrAudio;

    [Header("<=====PLAYER_STATS=====>")] 

    [SerializeField] public int HP;
    [SerializeField] int maxArmorHP; 
    [SerializeField] int jumpMax;
    [SerializeField] int jumpHighSpeed;
    [SerializeField] int jumpLowSpeed;
    [SerializeField] int gravity;

    [Range(0.1f, 0.5f)] public float audioDamageTimer; 

    [Header("<=====PLAYER_GUNS=====>")]

    [SerializeField] public List<gunStats> gunList = new List<gunStats>();

    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject flashMuzzle;
    [SerializeField] GameObject hitMarker;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;
    bool isReloading;
    public bool resetRecoil;

    [Header("<=====PLAYER_MOVEMENT=====>")]

    [SerializeField] public float speed;
    [SerializeField] float sprintModifier;

    [SerializeField] float slideDuration;
    [SerializeField] float slideModifier;

    [SerializeField] float crouchCenterYOffset;
    [SerializeField] float standingCenterYOffset;

    [Range(0.2f, 0.5f)] public float audioWalkTimer;
    [Range(0.01f, 0.1f)]public float audioRunTimer; 

    [Header("<=====PLAYER_GRENADES=====>")]

    [SerializeField] GameObject grenade;

    [SerializeField] public int grenadeCount;
    [SerializeField] float grenadeReloadTime;
    

    Vector3 moveDirection;
    Vector3 playerVelocity;

    public bool isSprinting { get; private set; }
    public bool isWalking { get; private set; }

    bool isShooting;
    bool isCrouching;
    bool isSliding;
    bool isJumping;
    bool isStraight;
    bool isDamageHit;

    int originalHP;
    int armHP;
    int jumpCount;
    public int selectedGun;

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
            if (!GameManager.Instance.diedOnce)
            {
                if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[selectedGun].ammoCurr > 0 && !isShooting) // Shoot Weapons 
                    StartCoroutine(shoot());

                if (Input.GetButtonDown("Grenade") && grenadeCount > 0) // Throw Grenade
                    StartCoroutine(throwGrenade());

                if (Input.GetButtonDown("Crouch") && (!isSprinting || isSliding)) // Crouch
                    crouch();

                if (Input.GetButtonDown("Crouch") && isSprinting && !isSliding) // Slide
                    StartCoroutine(slide());

                if (Input.GetButtonDown("Reload") && gunList.Count > 0 && gunList[selectedGun].ammoCurr < gunList[selectedGun].ammoMax && !isReloading) // Reload Guns
                {
                    StartCoroutine(StopShootingAndReloading());
                    AudioManager soundManager = AudioManager.Instance;
                    Audio Reload = soundManager.GetSoundByID("Reloads");
                    Reload.PlayOneShot(plrAudio);
                    if (gunList[selectedGun].ammoMax - (gunList[selectedGun].clipSize - gunList[selectedGun].ammoCurr) < 0)
                    {
                        gunList[selectedGun].ammoCurr += gunList[selectedGun].ammoMax;
                        gunList[selectedGun].ammoMax = 0;
                    }
                    else
                    {
                        Debug.Log("Hit it!");
                        gunList[selectedGun].ammoMax -= (gunList[selectedGun].clipSize - gunList[selectedGun].ammoCurr);
                        gunList[selectedGun].ammoCurr = gunList[selectedGun].clipSize;
                    }
                    
                    updatePlayerUI();
                }
            }
            if (GameManager.Instance.diedOnce)
            {
                //Meme Code - Blake Farrar
                gunModel.transform.position = new Vector3(1000, 1000, 1000);

            }

            selectGun();
        }

        sprint();
    }

    #region Player Movement Functionality

    void movement()
    {
        // Check if player is touching the ground
        if (!GameManager.Instance.ignoreMovement)
        {
            if (controller.isGrounded && !Input.GetButton("Jump"))
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

                AudioManager soundManager = AudioManager.Instance;
                Audio Jump = soundManager.GetSoundByID("Jumps");
                Jump.PlayOneShot(plrAudio);

                jumpCount++;
                playerVelocity.y = jumpHighSpeed;
            }

            if (Input.GetButtonUp("Jump") && playerVelocity.y > jumpCount)
            {
                playerVelocity.y = jumpLowSpeed;
            }

            playerVelocity.y -= gravity * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);

            if (controller.isGrounded && moveDirection.magnitude > audioWalkTimer && !isWalking)
            { StartCoroutine(PlayMovementAudio()); }
        }
          
        
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
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintModifier;
            isSprinting = false;
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

        //For audiomanager testing
        AudioManager soundManager = AudioManager.Instance;
        Audio Footstep = soundManager.GetSoundByID("Footstep");
        Footstep.PlayOneShot(plrAudio);

        if (!isSprinting)
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

        AudioManager soundManager = AudioManager.Instance;
        Audio Weapon = soundManager.GetSoundByID("Weapons");
        Weapon.PlayOneShot(plrAudio); 

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
        if (!isReloading)
        {
            isShooting = true;
            StartCoroutine(StartRecoil());
            StartCoroutine(ResetRecoil());
            

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
                    StartCoroutine(EnableHitMarker());
                }
                else if(hit.transform != transform && hit.collider.CompareTag("Head"))
                {
                    Transform t = hit.collider.gameObject.transform;
                    while (!t.CompareTag("Enemy"))
                    {
                        t = t.parent;
                    }
                    t.GetComponent<IDamage>().takeDamage(2*shootDamage);
                    StartCoroutine(EnableHitMarker());
                }
                else
                {
                    Instantiate(gunList[selectedGun].hitEffect, hit.point, Quaternion.identity);
                }
            }


            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    IEnumerator throwGrenade()
    {
        // TODO: Write code to create better arc.

        Instantiate(grenade, throwPos.position, transform.rotation);

        yield return new WaitForSeconds(grenadeReloadTime);

        AudioManager soundManager = AudioManager.Instance;
        Audio Grenade = soundManager.GetSoundByID("Grenades"); 
        Grenade.PlayOneShot(plrAudio);

        grenadeCount--;

        GameManager.Instance.updateGrenadeCount(-1);
    }
    IEnumerator ResetRecoil()
    {
        resetRecoil = true;
        yield return new WaitForSeconds(2f);
        resetRecoil = false;
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

        if (!isDamageHit && !GameManager.Instance.diedOnce)
        { StartCoroutine(PlayDamageHitSounds()); }
        

        updatePlayerUI();

        if (HP <= 0 && armHP <= 0)
        {
            GameManager.Instance.GameLoss("You Are Dead!");
        }
    }

    IEnumerator PlayDamageHitSounds() 
    {
        isDamageHit = true;

        AudioManager soundManager = AudioManager.Instance;
        Audio Damage = soundManager.GetSoundByID("DamageHits"); 
        Damage.PlayOneShot(plrAudio); 

        yield return new WaitForSeconds(audioDamageTimer);  

        isDamageHit = false; 
    }
    #endregion

    #region Player UI Functionality
    public void updatePlayerUI()
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

            if (armorPercentage <= 0.75f && armorPercentage > 0.25f)
                // Armor Is Blue
                GameManager.Instance.plrArmorHPBar.color = new Color(0.15f, 0.75f, 1f);
            else if (armorPercentage <= 0.25f)
                // Armor Is Red
                GameManager.Instance.plrArmorHPBar.color = new Color(1f, 0.25f, 0.25f);
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
    IEnumerator StopShootingAndReloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunList[selectedGun].reloadTime);
        isReloading = false;
    }
    IEnumerator EnableHitMarker()
    {
        hitMarker.SetActive(true);
        yield return new WaitForSeconds(.15f);
        hitMarker.SetActive(false);
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

        AudioManager soundManager = AudioManager.Instance;
        Audio Health = soundManager.GetSoundByID("HealthPickUp");
        Health.PlayOneShot(plrAudio);
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

        AudioManager soundManager = AudioManager.Instance;
        Audio Armor = soundManager.GetSoundByID("ArmorPickUp");
        Armor.PlayOneShot(plrAudio);
    }
    IEnumerator StartRecoil()
    {
        GameManager.Instance.playerShot = true;
        yield return new WaitForSeconds(.1f);
        GameManager.Instance.playerShot = false;
    }
    #endregion
}