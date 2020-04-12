using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravellerBehaviour : MonoBehaviour {

    public GameObject manager;
    public AgentsManager am;
    public Transform exit;
    public int choice;

    // timeout parameters
    public float timeout = 20f;
    public float timer = 0f;

    // steering behaviours
    public Vector3 force;
    public Vector3 velocity;
    public float maxSpeed = 20f;
    public float maxForce = 20f;
    public float maxSeeAhead = 20f;
    public float steeringMagnitude = 20f;

    // Use this for initialization
    void Start () {
        manager = GameObject.Find("AgentsManager");
        am = GameObject.Find("AgentsManager").GetComponent<AgentsManager>();
        // set choice of exit
        if (Random.value > 0.5f)
        {
            exit = manager.transform.GetChild(1);
            choice = 1;
        }
        else
        {
            exit = manager.transform.GetChild(2);
            choice = 2;
        }
        maxSpeed = Random.Range(15f, 20f);
        // populate other parameters
        maxSeeAhead = maxSpeed;
        maxForce = maxSpeed;
        steeringMagnitude = maxSpeed;
        velocity = new Vector3(-maxSpeed, 0f, 0f);
    }
	
	// Update is called once per frame
	void Update () {
        timer = timer + Time.deltaTime;
        // switch exit
        if (timer > timeout)
        {
            if (choice == 1)
            {
                exit = manager.transform.GetChild(2);
                choice = 2;
            }
            else
            {
                exit = manager.transform.GetChild(1);
                choice = 1;
            }
            timer = 0f;
        }

    }


    void FixedUpdate()
    {
        ComputeSteeringForce();
        ApplySteeringForce();
    }
    // compute steering
    void ComputeSteeringForce()
    {
        // clear existing force
        force = new Vector3(0f, 0f, 0f);
        force += Seek();        
    }

    // update velocity
    void ApplySteeringForce()
    {
        velocity = velocity + force * Time.deltaTime;
        if (velocity.magnitude > maxSpeed)
        {
            velocity = velocity.normalized * maxSpeed;
        }
        if (velocity == Vector3.zero) return;
        transform.position += velocity * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
    }

    Vector3 Seek()
    {
        Vector3 resultForce = new Vector3(0f, 0f, 0f);
        Vector3 desiredForce;
        if (exit == null) return resultForce; 

        desiredForce = exit.position - transform.position;
        // make sure no movement in y direction
        desiredForce.y = 0f;


        desiredForce = desiredForce.normalized * steeringMagnitude;
        resultForce = desiredForce - velocity;


        return resultForce * 2;
    }

    Vector3 ObstacleForce(GameObject gameObj)
    {
        Vector3 ahead = transform.position + velocity.normalized * maxSeeAhead;

        Vector3 resultForce = ahead - gameObj.transform.position;
        resultForce.y = 0f;

        resultForce = resultForce.normalized * maxForce * 2;
        
        return resultForce;
    }

    void OnCollisionEnter (Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            velocity += ObstacleForce(collision.gameObject) * Time.deltaTime;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        // encounter obstacle
        if (collision.gameObject.tag == "Obstacle")
        {
            velocity += ObstacleForce(collision.gameObject) * Time.deltaTime;
        }
    }
}
