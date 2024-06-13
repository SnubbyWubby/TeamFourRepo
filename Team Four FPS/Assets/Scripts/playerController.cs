using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float speed;
    [SerializeField] float sprintModifier;
    [SerializeField] int shootDamage;
    [SerializeField] int shootRate;
    [SerializeField] int shootDistance;

    Vector3 moveDirection;

    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();

        if (Input.GetButton("Fire1") && !isShooting)
            StartCoroutine(shoot());
    }

    void movement()
    {
        // Get input from player, create vector, then move player.
        moveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDirection * speed * Time.deltaTime);
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
}
