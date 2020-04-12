using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : MonoBehaviour {

    /*
     *  Script for the goal in the opposite terrain
     *  
     *  The golden coin is rotating on its own.
     *  If the player reachs the coin, it gets destroyed and game over.
     *  
     **/
    private float rotateSpeed = 5f;
    GameManager GM;
    // Use this for initialization
    void Start () {
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.back * rotateSpeed);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GM.playerScore++;
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Agent")
        {
            GM.AIScore++;
            Destroy(gameObject);
        }
    }
}
