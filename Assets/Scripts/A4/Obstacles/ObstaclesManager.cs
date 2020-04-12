using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesManager : MonoBehaviour {

    public int n = 2;
    public GameObject[] obstaclePrefab = new GameObject[7];
    public Transform[] obstacleSpawns = new Transform[10];
    public GameObject[] obstacles;
    // Use this for initialization
    void Start()
    {
        if (n < 2) n = 2;
        if (n > 5) n = 5;
        obstacles = new GameObject[n];

        for (int i = 0; i < 10; i++)
        {
            obstacleSpawns[i] = transform.GetChild(i);
        }

        int[] position = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < n; i++)
        {
            int index = Random.Range(i, 10);
            int tmp = position[i];
            position[i] = position[index];
            position[index] = tmp;
        }
        for (int i = 0; i < n; i++)
        {
            obstacles[i] = (GameObject)Instantiate(obstaclePrefab[Random.Range(0,7)], obstacleSpawns[position[i]].position, obstacleSpawns[position[i]].rotation);
            // perturb the obstacles a bit
            obstacles[i].transform.Rotate(Vector3.up, Random.Range(-10f, 10f));
            obstacles[i].transform.localScale += new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
            obstacles[i].transform.Translate(new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f)));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
