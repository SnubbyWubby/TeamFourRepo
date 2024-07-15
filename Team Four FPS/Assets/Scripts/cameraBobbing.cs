using System.Collections;
using System.Collections.Generic;
using TackleBox;
using UnityEngine;

public class cameraBobbing : MonoBehaviour
{
    [Header("<=====Sprint Modifiers=====>")]

    [SerializeField] float bounceSpeed;
    [SerializeField] float bounceDistance;

    [Header("<=====Sprint Modifiers=====>")]

    [Range(0.1f, 5f)] [SerializeField] float bounceSprintSpeed;
    [Range(0.1f, 5f)] [SerializeField] float bounceSprintModifier;

    float originalYPosition;
    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        originalYPosition = transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.stopCameraRotation)
        {
            if (GameManager.Instance.PlayerScript.isWalking && !GameManager.Instance.PlayerScript.isSprinting) // If player is walking and not sprinting
            {
                timer += Time.deltaTime * bounceSpeed;
                transform.localPosition = new Vector3(transform.localPosition.x, originalYPosition + Mathf.Sin(timer) * bounceDistance, transform.localPosition.z);
            }
            else if (GameManager.Instance.PlayerScript.isWalking && GameManager.Instance.PlayerScript.isSprinting) // If player is sprinting
            {
                timer += Time.deltaTime * bounceSpeed * bounceSprintSpeed;
                transform.localPosition = new Vector3(transform.localPosition.x, originalYPosition + Mathf.Sin(timer) * (bounceDistance * bounceSprintModifier), transform.localPosition.z);
            }
            else // If player is idle
            {
                timer = 0;
                transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, originalYPosition, Time.deltaTime * bounceSpeed), transform.localPosition.z);
            }
        }
    }
}
