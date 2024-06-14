using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage 
{
    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] int jumpMax;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDistance;

    [SerializeField] float speed;
    [SerializeField] float sprintModifier;

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

    int originalHP;
    int jumpCount;

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
        crouch();

        if (Input.GetButton("Fire1") && !isShooting)
            StartCoroutine(shoot());

        if (Input.GetButtonDown("Grenade") && grenadeCount > 0)
            StartCoroutine(throwGrenade());
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
            speed *= sprintModifier;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintModifier;
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

    void crouch()
    {
        if (Input.GetButtonDown("Crouch")) // Mapped to 'c' in input settings
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
