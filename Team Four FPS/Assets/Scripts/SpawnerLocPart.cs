using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnerLocPart : MonoBehaviour
{

    [SerializeField] ParticleSystem _spawner;
    Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    void Update()
    {
        turnOffParticles();
    }

    // Start is called before the first frame update
   public void turnOffParticles()
    {
        while (!_transform.GetComponent<SpawnEnemies>())
        {
            _transform = transform.parent;
        }
        if(_transform.GetComponent<SpawnEnemies>().killParticles == true){
            var a = _spawner.emission;
            a.rateOverTime = 0;
        }
    }
    

}
