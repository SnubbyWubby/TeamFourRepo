using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage 
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP; 
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    [SerializeField] float speed;
    [SerializeField] float sprintModifier;

    [SerializeField] float slideDuration;
    [SerializeField] float slideModifier;

    [SerializeField] float crouchCenterYOffset;
    [SerializeField] float standingCenterYOffset;

    [SerializeField] int grenadeCount;
    [SerializeField] float grenadeReloadTime;
    [SerializeField] Transform throwPos;
    [SerializeField] GameObject grenade; 

    Vector3 moveDirection;
    Vector3 playerVelocity;

    bool isShooting;
    bool isCrouching;
    bool isSprinting;

    int originalHP;
    int jumpCount;
    int selectedGun;

    float playerHeight;
    float crouchHeight;

    // Start is called before the first frame update
    void Start()
    {
        originalHP = HP;

        updatePlayerUI();

        playerHeight = controller.height;
        crouchHeight = controller.height / 2;

        GameManager.Instance.updateGrenadeCount(grenadeCount);
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();

        if (Input.GetButton("Fire1") && !isShooting)
            StartCoroutine(shoot());

        if (Input.GetButtonDown("Grenade") && grenadeCount > 0)
            StartCoroutine(throwGrenade());

        if (Input.GetButtonDown("Crouch") && !isSprinting) // Crouch
            crouch();

        if (Input.GetButtonDown("Crouch") && isSprinting) // Slide
            StartCoroutine(slide());
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        selectedGun = gunList.Count - 1;
        Debug.Log("Gun Added");
        
        shootDamage = gun.shootDamage;
        shootRate = gun.shootRate;
        shootDistance = gun.shootDistance;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1 )
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
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }

        // Get input from player, create vector, then move player.
        moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDirection * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
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

    IEnumerator shoot()
    {
        isShooting = true;

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
        }

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();

        if(HP <= 0)
        {
            GameManager.Instance.GameLoss();
        }
    }

    void updatePlayerUI()
    {
        GameManager.Instance.playerHPBar.fillAmount = (float)HP / originalHP;
    }

    public void HealthPack(int amount) 
    { 
        HP += amount;

        // Player Will Regenerate Full Health When Walking Or Running Towards The Blue Health Sphere

        GameManager.Instance.playerHPBar.fillAmount = (float)HP * originalHP; 
    }

    public void ArmorShield(int amount) 
    { 
        HP += amount;

        // Player Will Regenerate Full Armor When Walking Or Running Towards The Green Armor Shield 

        GameManager.Instance.playerHPBar.fillAmount = (float)HP * originalHP; 
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

        grenadeCount--;
        GameManager.Instance.updateGrenadeCount(-1);
    }
}