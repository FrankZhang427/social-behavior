using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject playerPrefab;
    public GameObject agentPrefab;
    public GameObject player;
    public GameObject agent;
    public GameObject[] coins;
    public int playerScore = 0;
    public int AIScore = 0;
    public bool over = false;
    void Awake()
    {
        // Hide the canvas at start
        gameOverPanel.SetActive(false);
    }

    // Use this for initialization
    void Start()
    {
        int a = Random.Range(0, 10);
        int b = Random.Range(0, 10);
        while (b == a) b = Random.Range(0, 10);
        player = (GameObject)Instantiate(playerPrefab, transform.GetChild(a).transform.position, transform.GetChild(a).transform.rotation);
        agent = (GameObject)Instantiate(agentPrefab, transform.GetChild(b).transform.position, transform.GetChild(b).transform.rotation);
        coins = GameObject.FindGameObjectsWithTag("GOLD");
    }

    // Update is called once per frame
    void Update()
    {
        coins = GameObject.FindGameObjectsWithTag("GOLD");
        if (playerScore + AIScore == 10) GameOver();
        if (player == null && agent == null) GameOver();
    }

    /* 
    * method to finish the game
    **/
    public void GameOver()
    {
        // show the canvas
        over = true;
        gameOverPanel.SetActive(true);
        if (playerScore > AIScore) gameOverText.text = "You Win!";
        else if (playerScore < AIScore) gameOverText.text = "AI Wins!";
        else gameOverText.text = "Tie!";
        // quit the application
        Application.Quit();
    }

    public void TeleportAgent(bool teleportAI)
    {
        int a = Random.Range(0, 10);
        if (teleportAI)
        {
            agent.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(transform.GetChild(a).transform.position);
        }
        else
        {
            player.transform.position =  transform.GetChild(a).transform.position;
        }
    }
}
