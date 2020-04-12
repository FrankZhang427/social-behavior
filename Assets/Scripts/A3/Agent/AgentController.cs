using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour {

    GameObject[] enemies;
    GameObject[] coins;
    GameObject player;
    GameManager GM;
    public NavMeshAgent agent;
    public int traps = 2;
    private Planner planner;
    public bool doneSearching;
    public bool searchSuccessful;
    private Queue<List<string>> actionQueue;
    private List<string> currentAction;
    private State worldState;
    private bool busy;
    private bool update;
    private float startTime;
    private Vector3 startMarker;
    private Vector3 endMarker;
    private float speed = 15f;
    private float journeyLength;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        planner = new Planner(typeof(Domain), new Domain().GetMethodsDict(), typeof(Domain));
        update = true;
        actionQueue = new Queue<List<string>>();
        worldState = new State("state1");
        
    }

    void Awake()
    {
        worldState = new State("state1");
    }
    // Update is called once per frame
    void Update()
    {
        if (GM.over)
        {
            agent.isStopped = true;
            return;
        }
        if (busy)
        {
            ContinueAction();
        }
        else if (actionQueue.Count > 0)
        {
            currentAction = actionQueue.Dequeue();
            DoNextAction(currentAction);
        }
        else
        {
            SearchPlanAndExecutePlan();
        }
        if (update)
        {
            Move();
        }
    }

    private void ContinueAction()
    {
        switch (currentAction[0])
        {
            case "MoveTo":
                MoveTo(endMarker);
                break;
            case "Hide":
                currentAction.Clear();
                busy = false;
                break;
            case "Finish":
                currentAction.Clear();
                busy = false;
                break;
        }
    }

    private void MoveTo(Vector3 target)
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startMarker, target, fracJourney);

        if (transform.position == target)
        {
            currentAction.Clear();
            busy = false;
        }
    }

    private void DoNextAction(List<string> action)
    {
        switch (action[0])
        {
            case "MoveTo":
                startMarker = transform.position;
                endMarker = new Vector3(startMarker.x, 0, startMarker.z);
                startTime = Time.time;
                journeyLength = Vector3.Distance(startMarker, endMarker);
                MoveTo(endMarker);
                busy = true;
                State state = (worldState != null) ? new State(worldState) : null;
                UpdateKnowledge("at", state.GetStateOfVar("at")[0], false);
                UpdateKnowledge("at", action[1], true);
                break;
            case "Hide":
                // basically stay in the alcove
                state = (worldState != null) ? new State(worldState) : null;
                UpdateKnowledge("at", state.GetStateOfVar("at")[0], false);
                UpdateKnowledge("at", action[1], true);
                busy = true;
                break;
            case "Finish":
                busy = true;
                break;
        }
    }
    void Move()
    {
        coins = GM.coins;
        float closestDist = float.MaxValue, dist = 0f;
        if (coins.Length == 0) return;
        GameObject closest = coins[0];
        agent.isStopped = true;
        foreach (GameObject coin in coins)
        {
            dist = Vector3.Distance(transform.position, coin.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = coin;
            }
        }
        agent.SetDestination(closest.transform.position);
        agent.isStopped = false;
        if (traps > 0)
        {
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float distToEnemy0 = Vector3.Distance(enemies[0].transform.GetChild(0).position, transform.position);
            float distToEnemy1 = Vector3.Distance(enemies[1].transform.GetChild(0).position, transform.position);
            float distToPlayer = player != null ? Vector3.Distance(player.transform.position, transform.position) : 10000f;
            if ((distToEnemy0 < 60f || distToEnemy1 < 60f) && distToPlayer > 55f && Random.Range(0, 50) == 0)
            {
                UseTeleportTrap();
            }
            else if (distToPlayer < 25f && GM.playerScore + GM.AIScore > 6)
            {
                UseTeleportTrap();
            }
        }
    }
    void UseTeleportTrap()
    {
        if (GM.over) return;
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
        dist = player != null ? Vector3.Distance(player.transform.position, transform.position) : 10000f;
        if (dist < closestDist) GM.TeleportAgent(false);// teleport player
        else  Destroy(closest);
    }

    public void UpdateKnowledge(string variable, string value, bool truthValue)
    {
        if (truthValue) worldState.Add(variable, value);
        else  worldState.Remove(variable, value);
    }
 
    public void UpdateKnowledge(string relation, string firstElement, string secondElement, bool truthValue)
    {
        if (truthValue) worldState.Add(relation, firstElement, secondElement);
        else  worldState.Remove(relation, firstElement, secondElement);
    }

    public void CancelSearch()
    {
        planner.CancelSearch = true;
    }

    public void ExecutePlan(List<string> plan)
    {
        actionQueue.Clear();
        foreach (string step in plan)
        {
            List<string> action = new List<string>();
            if (step.Contains(","))
            {
                action.Add(step.Substring(1, step.IndexOf(',') - 1));
                string stepRemainder = step.Substring(step.IndexOf(',') + 2);
                while (stepRemainder.Contains(","))
                {
                    action.Add(stepRemainder.Substring(0, stepRemainder.IndexOf(',')));
                    stepRemainder = step.Substring(step.IndexOf(',') + 2);
                }
                action.Add(stepRemainder.Substring(0, stepRemainder.IndexOf(')')));
            }
            else action.Add(step.Substring(1, step.IndexOf(')') - 1));
            actionQueue.Enqueue(action);
        }
    }

    private void SearchPlanAndExecutePlan()
    {
        doneSearching = false;
        searchSuccessful = false;
        if (update) return;
        State initialState = (worldState != null) ? new State(worldState) : null;
        if (initialState.ContainsVar("at")) initialState.Add("checked", initialState.GetStateOfVar("at")[0]);
        List<List<string>> goalTasks = new List<List<string>>();
        goalTasks.Add(new List<string>(new string[1] { "WinGame" }));
        List<string> plan = planner.SolvePlanningProblem(initialState, goalTasks);
        this.doneSearching = true;
        if (plan != null)  ExecutePlan(plan);
    }

}
