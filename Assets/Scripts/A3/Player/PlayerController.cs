using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    GameObject[] enemies;
    GameObject AI;
    GameManager GM;
    public Camera cam;
    // public NavMeshAgent agent;
    public float moveSpeed = 20f;
    public int traps = 2;
    
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        AI = GameObject.FindGameObjectWithTag("Agent");
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

	// Update is called once per frame
	void Update () {
        if (GM.over) return;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, 0f, moveZ);
        move.Normalize();
        move.Scale(new Vector3(moveSpeed * Time.deltaTime, moveSpeed * Time.deltaTime, moveSpeed * Time.deltaTime));
        transform.Translate(move);
       
        if (Input.GetKeyDown("space"))
        {
            UseTeleportTrap();
        }
    }

    void UseTeleportTrap()
    {
        if (traps <= 0) return;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        traps--;
        float closestDist = float.MaxValue, dist = 0f;
        GameObject closest = enemies[0];
        foreach (GameObject enemy in enemies)
        {
            dist = Vector3.Distance(enemy.transform.GetChild(0).position, transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }
        dist = Vector3.Distance(AI.transform.position, transform.position);
        if (dist < closestDist)
        {
            // teleport agent
            GM.TeleportAgent(true);
        } else
        {
            Destroy(closest);
        }
    }
}
