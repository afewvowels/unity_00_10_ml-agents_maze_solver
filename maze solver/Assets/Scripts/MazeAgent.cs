using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class MazeAgent : Agent
{
    public MazeAcademy academy;
    public MazeGenerator mazeGenerator;
    public Rigidbody agentRB;
    public int actions;
    public int goals;
    public int treasuresCollected;
    public float rewards;
    public Text rewardsText;
    public Text actionsText;
    public Text treasuresText;
    public Text goalsText;
    public Text collisionsText;
    public int currentMazeWidth;
    public Color goodRewardsColor;
    public Color badRewardsColor;
    public int collisionCount;
    public int maxCollisions;
    public GameObject goalLocation;

    private void Start()
    {
        collisionCount = 0;
        maxCollisions = 300;
        collisionsText.text = collisionCount.ToString();
        academy = FindObjectOfType<MazeAcademy>();
        agentRB = GetComponent<Rigidbody>();
        actions = 0;
        treasuresCollected = 0;
        goals = 0;
        rewards = 0.0f;
        actionsText.text = "0";
        rewardsText.text = rewards.ToString("0.000");
        goalsText.text = goals.ToString();
        // treasuresText.text = treasuresCollected.ToString();
        MakeMaze();
    }

    public override void InitializeAgent()
    {

    }

    public override void CollectObservations()
    {
        AddVectorObs(collisionCount);
        AddVectorObs(actions);

        AddVectorObs(goalLocation.transform.position.x);
        AddVectorObs(goalLocation.transform.position.z);

        AddVectorObs(transform.position.x);
        AddVectorObs(transform.position.z);
    }

    public override void AgentAction(float[] vectorAction)
    {
        var moveAction = Mathf.FloorToInt(vectorAction[0]);
        var rotateAction = Mathf.FloorToInt(vectorAction[1]);

        Vector3 movement = new Vector3(0.0f, 0.0f, 1.0f);

        float existentialPenalty = -1.0f / 50000.0f;

        switch (moveAction)
        {
            case 0:
                AddReward(existentialPenalty * 5.0f);
                break;
            case 1:
                agentRB.AddRelativeForce(movement, ForceMode.VelocityChange);
                AddReward(-existentialPenalty);
                break;
            case 2:
                agentRB.AddRelativeForce(movement * 2.0f, ForceMode.VelocityChange);
                AddReward(-existentialPenalty);
                break;
            case 3:
                // agentRB.AddRelativeForce(movement * -0.333f, ForceMode.VelocityChange);
                AddReward(existentialPenalty);
                agentRB.velocity = Vector3.zero;
                break;
        }

        switch (rotateAction)
        {
            case 1:
                transform.Rotate(0.0f, -5.0f, 0.0f);
                break;
            case 2:
                transform.Rotate(0.0f, 5.0f, 0.0f);
                break;
        }

        AddReward(existentialPenalty);
        rewards += existentialPenalty;
        UpdateRewardsText();

        actions++;
        actionsText.text = actions.ToString();
    }

    public override float[] Heuristic()
    {
        var action = new float[2];

        if (Input.GetKey(KeyCode.W))
        {
            action[0] = 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            action[0] = 3.0f;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            action[0] = 2.0f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            action[1] = 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            action[1] = 2.0f;
        }

        return action;
    }

    public override void AgentReset()
    {
        collisionCount = 0;
        collisionsText.text = collisionCount.ToString();
        agentRB.velocity = Vector3.zero;
        agentRB.angularVelocity = Vector3.zero;
        mazeGenerator.DestroyMaze();
        rewards = 0.0f;
        rewardsText.text = rewards.ToString("0.000");
        actions = 0;
        actionsText.text = actions.ToString();
        // treasuresCollected = 0;
        // treasuresText.text = treasuresCollected.ToString();
        MakeMaze();
    }

    public void UpdateRewardsText()
    {
        rewardsText.text = rewards.ToString("0.000");
        if (rewards >= 0.0f)
        {
            rewardsText.color = goodRewardsColor;
        }
        else
        {
            rewardsText.color = badRewardsColor;
        }
    }

    public void UpdateCollisions()
    {
        collisionCount++;
        collisionsText.text = collisionCount.ToString();
        if (collisionCount > maxCollisions)
        {
            AddReward(-0.5f);
            Done();
        }
    }

    private void MakeMaze()
    {
        bool obstacles;
        bool makeInterior;

        if ((int)academy.resetParameters["obstacles"] == 0)
        {
            obstacles = false;
        }
        else
        {
            obstacles = true;
        }

        if ((int)academy.resetParameters["interior"] == 0)
        {
            makeInterior = false;
        }
        else
        {
            makeInterior = true;
        }

        int width = Random.Range((int)academy.resetParameters["min_width"], (int)academy.resetParameters["max_width"]);

        if (width % 2 == 0)
        {
            width++;
        }

        // if (width > 20)
        // {
        //     this.agentParameters.maxStep = 15000;
        // }
        // else if (width > 15)
        // {
        //     this.agentParameters.maxStep = 10000;
        // }
        // else if (width > 10)
        // {
        //     this.agentParameters.maxStep = 5000;
        // }
        // else
        // {
        //     this.agentParameters.maxStep = 2500;
        // }

        currentMazeWidth = width;
        mazeGenerator.guideChance = academy.resetParameters["guidechance"];
        mazeGenerator.CreateMaze(width, width, makeInterior, obstacles);
    }

    public void ScoredGoal()
    {
        goals++;
        goalsText.text = goals.ToString();   
    }

    public void PickedUpTreasure()
    {
        // treasuresCollected++;
        // treasuresText.text = treasuresCollected.ToString();
    }

    private void OnCollisionEnter(Collision c)
    {
        if (c.gameObject.CompareTag("mazewalls"))
        {
            float penalty = -1.0f / 1000.0f;
            AddReward(penalty);
            rewards += penalty;
            UpdateRewardsText();
            UpdateCollisions();
        }
    }

    private void OnCollisionStay(Collision c)
    {
        if (c.gameObject.CompareTag("mazewalls"))
        {
            float penalty = -1.0f / 500.0f;
            AddReward(penalty);
            rewards += penalty;
            UpdateRewardsText();
            UpdateCollisions();
        }
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("mazeguide"))
        {
            float reward = 1.0f / 10.0f;
            AddReward(reward);
            rewards += reward;
            UpdateRewardsText();
            Destroy(c.gameObject);
        }

        if (c.gameObject.CompareTag("mazegoal"))
        {
            if (actions < (10000.0f / 10.0f))
            {
                AddReward(2.0f);
            }
            else if (actions < (10000.0f / 5.0f))
            {
                AddReward(1.0f);
            }
            else if (actions < (10000.0f / 2.0f))
            {
                AddReward(0.5f);
            }

            ScoredGoal();
            AddReward(2.0f);
            Done();
        }
    }
}
