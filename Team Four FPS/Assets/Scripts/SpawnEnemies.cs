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

    bool spawnTruth;   
    bool spawnStart;  

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.updateGameGoal(spawnNumber); 
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnStart && spawnCount < spawnNumber && !spawnTruth) 
        {
            StartCoroutine(EnemySpawn()); 
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

        spawnCount++;

        yield return new WaitForSeconds(spawnTimer);

        adSpawn.PlayOneShot(spawnAudio[Random.Range(0, spawnAudio.Length)], spawnVolume);

        spawnTruth = false;
    }
}