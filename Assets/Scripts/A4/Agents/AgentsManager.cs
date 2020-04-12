using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsManager : MonoBehaviour {

    public int numberOfTravellers = 1; // maximum of travellers
    public int numberOfWanderers = 1; // maximum of wanderers
    public int numberOfSocialAgents = 1; // maximum of social agents
    public GameObject travellerPrefab;
    public GameObject wandererPrefab;
    public GameObject socialAgentPrefab;
    public Transform travellerSpawn;
    public GameObject[] travellers;
    public GameObject[] socials;

    // spawning parameters
    public float spawnRate = 1f;
    public float spawnTimer = 0f;
    // Use this for initialization
    void Start () {
        travellers = new GameObject[numberOfTravellers];
        socials = new GameObject[numberOfSocialAgents];
        for (int i = 0; i < numberOfWanderers; i++)
        {
            GameObject wanderer = (GameObject)Instantiate(wandererPrefab);
            wanderer.transform.position = new Vector3(Random.Range(-180f,180f), 2, Random.Range(-90f, 90f));
        }

        for (int i = 0; i < numberOfSocialAgents; i++)
        {
            socials[i] = (GameObject)Instantiate(socialAgentPrefab);
            socials[i].transform.position = new Vector3(Random.Range(-180f, 180f), 2, Random.Range(-90f, 90f));
        }
    }
	
	// Update is called once per frame
	void Update () {
        // create travellers one by one
        spawnTimer = spawnTimer + Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            for (int i = 0; i < numberOfTravellers; i++)
            {
                if (travellers[i] == null)
                {
                    travellers[i] = (GameObject)Instantiate(travellerPrefab, travellerSpawn.position, travellerSpawn.rotation);
                    break;
                }
            }
            spawnTimer = 0f;
        }
    }
}
