using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "SmallObstacle")
        {
            if (Random.value < 1.0f / 3.0f)
            {
                // pass through
                //Vector3 position = transform.parent.position;
                //transform.parent.position = new Vector3(-1f * position.x, position.y, position.z);
            } else if (Random.value > 2.0f / 3.0f)
            {
                // disappear (destroy)
                Destroy(transform.parent.gameObject);
            } else
            {
                // reverse
                transform.parent.GetComponent<EnemyMovement>().positiveDir = !transform.parent.GetComponent<EnemyMovement>().positiveDir;
                transform.parent.localRotation *= Quaternion.Euler(0, 180, 0);
            }
        }
        if (collider.gameObject.tag == "Doorway")
        {
            // destroy object
            Destroy(transform.parent.gameObject);
        }
    }
}
