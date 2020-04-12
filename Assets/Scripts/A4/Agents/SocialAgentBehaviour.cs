using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialAgentBehaviour : MonoBehaviour {

    public GameObject manager;
    public AgentsManager am;

    // steering behaviours
    public Vector3 force;
    public Vector3 velocity;
    public float maxSpeed = 20f;
    private float stoppingDistance;

    // seeking social agent
    private bool hasTargetSocial;
    private Vector3 target;
    private SocialAgentBehaviour closest;

    // wander parameters
    private float offset = 1.0f;
    private float radius = 1.0f;
    private float jitter = 0.2f;
    private Vector3 seekPosition;
    private Vector3 targetDirection;
    private Vector3 randomDirection;
    private Vector3 circlePosition;

    // social parameters
    private Vector3 groupPosition;
    public bool canSocial;
    private Renderer renderer_;
    // Use this for initialization
    void Start()
    {
        manager = GameObject.Find("AgentsManager");
        am = GameObject.Find("AgentsManager").GetComponent<AgentsManager>();
        renderer_ = GetComponent<Renderer>();
        maxSpeed = Random.Range(8f, 10f); // slow at start
        stoppingDistance = 3f;
        velocity = new Vector3(0f, 0f, 0f);
        groupPosition = transform.position;
        hasTargetSocial = false;
        canSocial = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canSocial)
        {
            ClosestSocial();
            if (closest != null) hasTargetSocial = Vector3.Distance(transform.position, closest.transform.position) < 20f;
        }
    }

    void FixedUpdate()
    {
        if (canSocial && hasTargetSocial && Random.value > 0.5f)
        {
            groupPosition = (closest.groupPosition + transform.position) / 2.0f;
            closest.groupPosition = groupPosition;
            target = groupPosition;
            maxSpeed = 8f;
            ComputeArrivalForce();
            ApplySteeringForce();

        }
        else
        {
            // wander
            maxSpeed = Random.Range(8f, 10f);
            ComputeWanderForce();
            ApplySteeringForce();
        }
    }

    // cool off timer
    IEnumerator Cooloff()
    {
        canSocial = false;
        yield return new WaitForSeconds(5f);
        canSocial = true;
        renderer_.material.color = Color.yellow;
    }
    // social timer
    IEnumerator SocialTime()
    {
        canSocial = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        canSocial = false;
        renderer_.material.color = Color.green;
        StartCoroutine(Cooloff());
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

    void ComputeArrivalForce()
    {
        // clear existing force
        force = new Vector3(0f, 0f, 0f);
        force += Arrival();

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
        Vector3 desiredForce;
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

    Vector3 Arrival()
    {
        Vector3 resultForce = new Vector3(0f, 0f, 0f);
        Vector3 desiredForce =  target - this.transform.position;
        desiredForce.y = 0;

        float distance = desiredForce.magnitude;
        desiredForce = distance < stoppingDistance ? desiredForce.normalized * maxSpeed * (distance / stoppingDistance) : desiredForce.normalized * maxSpeed;
        resultForce = desiredForce - velocity;

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
        else if (collision.gameObject.tag == "Social Agent")
        {
            StartCoroutine(SocialTime());
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

    void ClosestSocial()
    {
        float min = float.MaxValue;

        for (int i = 0; i < am.socials.Length; i++)
        {
            if (am.socials[i] == null) continue;
            float distance = Vector3.Distance(transform.position, am.socials[i].transform.position);
            if (distance < min)
            {
                min = distance;
                closest = am.socials[i].GetComponent<SocialAgentBehaviour>();
            }
        }
    }
}
