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

    bool isShooting;
    bool playerInRange;
    Vector3 playerDirection;

    //PATROL IMPLEMENTATION
    public Transform[] waypoints;
    int patrolPoint;
    Vector3 target;
    bool wasShot;
    

    
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = GameManager.Instance.Player.transform.position - transform.position;
        float agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTranSpeed));

        if (playerInRange)
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
        else
        {
            if (wasShot)
            {
                StartCoroutine(ResetBool());
                return;
            }
            UpdateDestination();
            if(Vector3.Distance(transform.position, target) < 10)
            {
                UpdateIndex();
            }
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
        HP -= amount;
        wasShot = true;
        agent.SetDestination(GameManager.Instance.Player.transform.position);
        StartCoroutine(flashDamage());
        if(HP <= 0)
        {
            GameManager.Instance.updateGameGoal(-1);
            Destroy(gameObject);
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
    IEnumerator ResetBool()
    {
        yield return new WaitForSeconds(10);
        wasShot = false;
    }
}
