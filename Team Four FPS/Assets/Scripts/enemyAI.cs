using System.Collections;
using System.Collections.Generic;
using TackleBox;
using TackleBox.Audio;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [Header("<=====COMPONENTS=====>")]

    [SerializeField] Animator anim;
    [SerializeField] Renderer model;
    [SerializeField] GameObject bullet;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [Header("<=====ENEMY_GUN_STATS=====>")]

    [SerializeField] int HP;
    [SerializeField] int shootAngle;
    [SerializeField] int animTranSpeed;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float shootRate;

    [Header("<=====ENEMY_MOVEMENT=====>")]

    [SerializeField] int viewAngle;
    [SerializeField] float roamDist;
    [SerializeField] float roamTimer;
    [SerializeField] bool willPatrol;
    [SerializeField] bool willRoam;

    [Header("<=====ENEMY_AUDIO_SOUNDS=====>")]

    [SerializeField] AudioSource enemyAudio;

    [SerializeField] AudioClip[] gunAudio;
    [SerializeField] float gunVolume;

    //[SerializeField] AudioClip[] enemyHitAudio;
    //[SerializeField] float enemyHitVolume; 

    bool wasShot;
    bool isShooting;
    bool playerInRange;
    bool canTakeDamage;
    bool destChosen;

    Vector3 playerDirection;
    Vector3 origPos;
    Vector3 target;
    
    public Transform[] waypoints;
    int patrolPoint;
    float stoppingDistOrig;
    float angleToPlayer;
     
    // Start is called before the first frame update
    void Start()
    {
        origPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        canTakeDamage = true;
    }

    // Update is called once per frame
    void Update()
    {
        playerDirection = GameManager.Instance.Player.transform.position - transform.position;
        float agentSpeed = agent.velocity.normalized.magnitude;
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agentSpeed, Time.deltaTime * animTranSpeed));
        //agent.SetDestination(GameManager.Instance.Player.transform.position);

        //comment this out and uncomment section beneath for standard ai behaviour
        if (playerInRange)
        {
            FaceChaseShoot();
        }

        //if (playerInRange && !canSeePlayer())
        //{
        //    if (willRoam)
        //    {
        //        StartCoroutine(roam());
        //    }
        //    else if (willPatrol)
        //    {
        //        Retaliate();
        //    }
        //}
        //else if (!playerInRange)
        //{
        //    if (willPatrol)
        //    {
        //        Retaliate();
        //    }
        //    else if (willRoam)
        //    {
        //        StartCoroutine(roam());
        //    }
        //}
    }

    IEnumerator roam()
    {
        if (!destChosen && agent.remainingDistance < 0.05f)
        {
            destChosen = true;
            yield return new WaitForSeconds(roamTimer);

            agent.stoppingDistance = 0;

            Vector3 ranPos = Random.insideUnitSphere * roamDist;
            ranPos += origPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
            agent.SetDestination(hit.position);

            destChosen = false;
        }
    }

    bool canSeePlayer()
    {
        playerDirection = GameManager.Instance.Player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, playerDirection.y + 0.5f, playerDirection.z), transform.forward);

        Debug.DrawRay(headPos.position, new Vector3(playerDirection.x, playerDirection.y + 0.5f, playerDirection.z));

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(GameManager.Instance.Player.transform.position);
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    FaceTarget();
                }

                if (!isShooting && angleToPlayer <= shootAngle)
                    StartCoroutine(Shoot());

                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
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
        enemyAudio.PlayOneShot(gunAudio[Random.Range(0, gunAudio.Length)], gunVolume);
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

        AudioManager soundManager = AudioManager.Instance;
        Audio Damage = soundManager.GetSoundByID("DamageHits");
        Damage.PlayOneShot(enemyAudio);

        //enemyAudio.PlayOneShot(enemyHitAudio[Random.Range(0, enemyHitAudio.Length)], enemyHitVolume); 
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