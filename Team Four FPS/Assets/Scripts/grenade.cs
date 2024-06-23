using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] int damage;
    [SerializeField] float blastRadius;
    [SerializeField] float throwSpeed;
    [SerializeField] float detonateTime;
    [SerializeField] ParticleSystem explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.forward * throwSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Timer - decrease time until detonation
        detonateTime -= Time.deltaTime;

        if (detonateTime <= 0) {
            detonate();
        }
    }

    void detonate()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        IDamage damageable;

        // Determine all objects that are hit
        Collider[] hits = Physics.OverlapSphere(gameObject.transform.position, blastRadius);

        foreach (Collider hit in hits)
        {
            damageable = hit.GetComponent<IDamage>();

            if (hit.transform != transform && damageable != null)
            {
                Debug.Log(hit.transform.name);
                damageable.takeDamage(damage);
            }
        }

        GameManager.Instance.inGrenadeRadius = false;

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.inGrenadeRadius = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.inGrenadeRadius = false;
        }
    }
}
