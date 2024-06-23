using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grappler : MonoBehaviour
{
    [SerializeField] int gappleTime;
    [SerializeField] int maxGrappleDistance;
    bool isGrappling = false;

    // Start is called before the first frame update
    void Start()
    {
        
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

        // Use raycast and get gameobject that is hit
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxGrappleDistance))
        {

            //hit.transform
        }
        yield return new WaitForSeconds(gappleTime);
    }
}
