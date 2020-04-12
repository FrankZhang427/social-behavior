using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public float moveSpeed = 5f;
    public bool positiveDir;
	// Use this for initialization
	void Start () {
		if (transform.position.x > 0.0f)
        {
            positiveDir = false;
            transform.localRotation *= Quaternion.Euler(0, 180, 0);
        } else
        {
            positiveDir = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (positiveDir)
        {
            transform.position += Vector3.right * Time.deltaTime * moveSpeed;
        }
        else
        {
            transform.position += Vector3.left * Time.deltaTime * moveSpeed;
        }

        if (transform.position.x < 6.5f && transform.position.x > -6.5f)
        {
            transform.GetChild(1).gameObject.SetActive(false);
        } else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
	}
}
