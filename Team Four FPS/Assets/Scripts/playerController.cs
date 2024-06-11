using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float speed;
    [SerializeField] float sprintModifier;

    Vector3 moveDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
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
}
