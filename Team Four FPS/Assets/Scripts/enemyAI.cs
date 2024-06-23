using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{

    [SerializeField] int HP;
    [SerializeField] Renderer model;
    [SerializeField] int animTranSpeed;
    [SerializeField] Animator anim;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] bool willPatrol;
    [SerializeField] bool willRoam;

    bool wasShot;
    bool isShooting;
    bool playerInRange;
    bool canTakeDamage;
   

    Vector3 playerDirection;
    Vector3 origPos;
    Vector3 target;
    
    public Transform[] waypoints;
    int patrolPoint;
    
    
    
    

    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.updateGameGoal(1);
        origPos = transform.position;
        canTakeDamage = true;
        //SetRigidBodyState(true);
        //SetColliderState(false);
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = GameManager.Instance.Player.transform.position - transform.position;
        float agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTranSpeed));

        if (playerInRange)
        {
            FaceChaseShoot();
        }
        else if (willPatrol)
        {
            Retaliate();
        }
        

    }

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    IEnumerator Shoot()
    {
        isShooting = true;
        anim.SetTrigger("Shoot");
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void takeDamage(int amount)
    {
        if (canTakeDamage)
        {
            HP -= amount;
            wasShot = true;
            agent.SetDestination(GameManager.Instance.Player.transform.position);
            StartCoroutine(flashDamage());
            if (HP <= 0)
            {
                canTakeDamage = false;
                GameManager.Instance.updateGameGoal(-1);

                StopMoving();
                Destroy(gameObject, 1f);
                StartCoroutine(ResetBool2());
                Ragdoll();



            }
            StartCoroutine(ResetBool1());
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = Color.white;
    }

    void UpdateDestination()
    {
        target = waypoints[patrolPoint].position;
        agent.SetDestination(target);
    }

    void UpdateIndex()
    {
        patrolPoint++;
        if(patrolPoint == waypoints.Length)
        {
            patrolPoint = 0;    
        }
    }
    IEnumerator ResetBool1()
    {
        wasShot = true;
        yield return new WaitForSeconds(10);
        wasShot = false;
        agent.SetDestination(origPos);
    }

    IEnumerator ResetBool2()
    {
        isShooting = true;
        yield return new WaitForSeconds(20);
        isShooting = false;
    }
    void SetRigidBodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }
        GetComponent<Rigidbody>().isKinematic = !state;
    }
    void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
        GetComponent<Collider>().enabled = !state;  
    }
    public void Ragdoll()
    {
        GetComponent<Animator>().enabled = false;
        //SetRigidBodyState(false);
        //SetColliderState(true);
    }
    public void StopMoving()
    {
        agent.isStopped = true;
    }
    public void FaceChaseShoot()
    {
        agent.SetDestination(GameManager.Instance.Player.transform.position);
        if (agent.remainingDistance < agent.stoppingDistance)
        {
            FaceTarget();

        }
        if (!isShooting)
        {
            StartCoroutine(Shoot());
        }
    }
    public void Retaliate()
    {
        if (wasShot)
        {
          return;
        }
        UpdateDestination();
        if (Vector3.Distance(transform.position, target) < 10)
        {
            UpdateIndex();
        }
    }
}
