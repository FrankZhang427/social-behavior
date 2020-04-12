using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public GameObject enemyPrefab;
    public Transform[] enemySpawns = new Transform[4];
    public GameObject[] enemies = new GameObject[2];
    // Use this for initialization
    void Start () {
		for (int i = 0; i < 4; i++)
        {
            enemySpawns[i] = transform.GetChild(i);
        }
        if (Random.value > 0.5f) enemies[0] = (GameObject)Instantiate(enemyPrefab, enemySpawns[0].position, enemySpawns[0].rotation);
        else enemies[0] = (GameObject)Instantiate(enemyPrefab, enemySpawns[1].position, enemySpawns[1].rotation);
        if (Random.value > 0.5f) enemies[1] = (GameObject)Instantiate(enemyPrefab, enemySpawns[2].position, enemySpawns[2].rotation);
        else enemies[1] = (GameObject)Instantiate(enemyPrefab, enemySpawns[3].position, enemySpawns[3].rotation);
    }
	
	// Update is called once per frame
	void Update () {
		if (enemies[0] == null)
        {
            if (Random.value > 0.5f) enemies[0] = (GameObject)Instantiate(enemyPrefab, enemySpawns[0].position, enemySpawns[0].rotation);
            else enemies[0] = (GameObject)Instantiate(enemyPrefab, enemySpawns[1].position, enemySpawns[1].rotation);
        }
        if (enemies[1] == null)
        {
            if (Random.value > 0.5f) enemies[1] = (GameObject)Instantiate(enemyPrefab, enemySpawns[2].position, enemySpawns[2].rotation);
            else enemies[1] = (GameObject)Instantiate(enemyPrefab, enemySpawns[3].position, enemySpawns[3].rotation);
        }
	}
}
