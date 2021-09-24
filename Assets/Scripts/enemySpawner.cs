using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class enemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;

    public int numberOfEnemies;

    public override void OnStartServer()
    {
        for(int i =0; i< numberOfEnemies; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-8f, 8f), 0, Random.Range(-8f, 8f)
                );

            var spawnRotation = Quaternion.Euler(
                new Vector3(0.0f, Random.Range(0, 180), 0.0f)
                );

            var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);

            NetworkServer.Spawn(enemy);
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
