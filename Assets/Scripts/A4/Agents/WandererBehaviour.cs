using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandererBehaviour : MonoBehaviour {

    public GameObject manager;
    public AgentsManager am;

    // steering behaviours
    public Vector3 force;
    public Vector3 velocity;
    public Vector3 desiredForce;
    public float maxSpeed = 20f;

    // seeking traveller
    private bool hasTargetTraveller;
    private Vector3 target;
    private TravellerBehaviour closest;

    // wander parameters
    private float offset = 1.0f;
    private float radius = 1.0f;
    private float jitter = 0.2f;
    private Vector3 seekPosition;
    private Vector3 targetDirection;
    private Vector3 randomDirection;
    private Vector3 circlePosition;

    // Use this for initialization
    void Start () {
        manager = GameObject.Find("AgentsManager");
        am = GameObject.Find("AgentsManager").GetComponent<AgentsManager>();

        maxSpeed = Random.Range(10f, 12f); // slow at start
        velocity = new Vector3(0f, 0f, 0f);
        hasTargetTraveller = false;
    }
	
	// Update is called once per frame
	void Update () {
        ClosestTraveller();
        // set detect range to be 15f
        if(closest != null) hasTargetTraveller = Vector3.Distance(transform.position, closest.transform.position) < 20f; // doorway width or two

    }

    void FixedUpdate()
    {
        if (hasTargetTraveller)
        {
            // persuit
            if (Vector3.Distance(transform.position, closest.exit.position) < Vector3.Distance(closest.transform.position, closest.exit.position))
            {
                maxSpeed = 20f; // slightly faster than traveller
                target = closest.transform.position + closest.velocity * Time.deltaTime * 10f;
                ComputePursuitForce();
                ApplySteeringForce();
            }

        }
        else
        {
            // wander
            maxSpeed = Random.Range(10f, 12f);
            ComputeWanderForce();
            ApplySteeringForce();
        }
    }


    void ComputePursuitForce()
    {
        // clear existing force
        force = new Vector3(0f, 0f, 0f);
        force += Seek();

    }

    void ComputeWanderForce()
    {
        // clear existing force
        force = new Vector3(0f, 0f, 0f);
        force += Wander();

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

        desiredForce = target - transform.position;
        // make sure no movement in y direction
        desiredForce.y = 0f;

        desiredForce = desiredForce.normalized * maxSpeed;
        resultForce = desiredForce - velocity;

        return resultForce * 2;
    }

    Vector3 Wander()
    {
        Vector3 resultForce = new Vector3(0f, 0f, 0f);

        randomDirection = new Vector3(Random.Range(0, 0x7fff) - (0x7fff * 0.5f), 0, Random.Range(0, 0x7fff) - (0x7fff * 0.5f));
        randomDirection = randomDirection.normalized * jitter;

        targetDirection = (targetDirection + randomDirection).normalized * radius;

        seekPosition = transform.position + targetDirection + transform.forward * offset;
        desiredForce = seekPosition - transform.position;

        if (desiredForce != Vector3.zero)
        { 
            desiredForce = desiredForce.normalized * maxSpeed;
            resultForce = desiredForce - velocity;
        }

        return resultForce * 2;
    }

    Vector3 ObstacleForce(GameObject gameObj)
    {
        Vector3 ahead = transform.position + velocity.normalized * maxSpeed;

        Vector3 resultForce = ahead - gameObj.transform.position;
        resultForce.y = 0f;

        resultForce = resultForce.normalized * maxSpeed * 2;

        return resultForce;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            velocity += ObstacleForce(collision.gameObject) * Time.deltaTime;
        }
        else if (collision.gameObject.tag == "Wall")
        {
            velocity *= -1f;
        }
    }
    void OnCollisionStay(Collision collision)
    {
        // encounter obstacle
        if (collision.gameObject.tag == "Obstacle")
        {
            velocity += ObstacleForce(collision.gameObject) * Time.deltaTime;
        }
        else if (collision.gameObject.tag == "Wall")
        {
            velocity *= -1f;
        }
    }

    void ClosestTraveller()
    {
        float min = float.MaxValue;

        for (int i = 0; i < am.travellers.Length; i++)
        {
            if (am.travellers[i] == null) continue;
            float distance = Vector3.Distance(transform.position, am.travellers[i].transform.position);
            if (distance < min)
            {
                min = distance;
                closest = am.travellers[i].GetComponent<TravellerBehaviour>();
            }
        }
    }
}
