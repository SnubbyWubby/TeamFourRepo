using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    [SerializeField] float gappleTime;

    [SerializeField] int maxGrappleDistance;
    bool isGrappling = false;



    CharacterController Controller;

    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Grapple") && !isGrappling)
            StartCoroutine(Grapple());
    }

    IEnumerator Grapple()
    {
        RaycastHit hit;
        isGrappling = true;
        // Use raycast and get gameobject that is hit
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxGrappleDistance))
        {
            Debug.Log(hit.point);
            if (hit.transform != transform)
            {
                transform.position = Vector3.Lerp(transform.position, hit.point, gappleTime);
            }
        }
        yield return new WaitForSeconds(gappleTime);
        isGrappling = false;
    }
}
