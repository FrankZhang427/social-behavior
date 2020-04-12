using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorwayTrigger : MonoBehaviour {

    public float timeout = 60f;
    public float timer = 0f;
    public bool stop = false;
    public int count = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        // switch exit
        if (timer > timeout)
        {
            stop = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Traveller")
        {
            // destroy object
            Destroy(collision.gameObject);
            if (!stop)
            {
                count++;
            }
        }
    }
}
