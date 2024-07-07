using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("<=====COMPONENTS=====>")]

    [SerializeField] GameObject spawnObjects;
    [SerializeField] Transform[] spawnPosition;

    [Header("<=====SPAWN_AUDIO=====>")]

    [SerializeField] AudioSource adSpawn; 

    [SerializeField] AudioClip[] spawnAudio;
    [SerializeField] float spawnVolume; 

    [Header("<=====ENEMY_SPAWN=====>")]

    [SerializeField] int spawnTimer;
    [SerializeField] int spawnNumber;

    int spawnCount;
    int numberSpawned=0;

    bool spawnTruth;   
    bool spawnStart;  

    // Start is called before the first frame update
    void Start()
    {
        
        spawnCount = 15;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.enemyCount == 0)
        {
            GameManager.Instance.spawnMoreEnemies = true;
        }
        if (GameManager.Instance.spawnMoreEnemies&& !spawnTruth && !GameManager.Instance.roundTransition )//spawnStart && spawnCount < spawnNumber && !spawnTruth) 
        {
            StartCoroutine(EnemySpawn());
            if(GameManager.Instance.enemyCount == 0)
            GameManager.Instance.updateGameGoal(15);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        { 
            spawnStart = true; 
        }
    }

    IEnumerator EnemySpawn() 
    {
        spawnTruth = true;

        int posArray = Random.Range(0, spawnPosition.Length);

        Instantiate(spawnObjects, spawnPosition[posArray].position, spawnPosition[posArray].rotation);

        numberSpawned++;
        
        yield return new WaitForSeconds(spawnTimer);

        //adSpawn.PlayOneShot(spawnAudio[Random.Range(0, spawnAudio.Length)], spawnVolume);
        

        spawnTruth = false;

        if (numberSpawned >= spawnCount)
        {
            GameManager.Instance.spawnMoreEnemies = false;
            numberSpawned = 0;
        }
        
        
        
    }
}