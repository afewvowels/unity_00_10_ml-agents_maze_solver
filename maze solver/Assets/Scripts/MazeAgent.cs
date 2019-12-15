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
    public int currentMazeWidth;

    private void Start()
    {
        agentRB = GetComponent<Rigidbody>();
        actions = 0;
        treasuresCollected = 0;
        goals = 0;
        rewards = 0.0f;
        actionsText.text = "0";
        rewardsText.text = "0.000";
        academy = FindObjectOfType<MazeAcademy>();
        MakeMaze();
    }

    public override void InitializeAgent()
    {

    }

    public override void AgentAction(float[] vectorAction)
    {
        var moveAction = Mathf.FloorToInt(vectorAction[0]);
        var rotateAction = Mathf.FloorToInt(vectorAction[1]);

        Vector3 movement = new Vector3(125.0f, 0.0f, 0.0f);

        switch(moveAction)
        {
            case 1:
                agentRB.AddRelativeForce(movement, ForceMode.Force);
                break;
            case 2:
                agentRB.AddRelativeForce(movement * 2.0f, ForceMode.Force);
                break;
            case 3:
                agentRB.AddRelativeForce(-movement, ForceMode.Force);
                break;
        }

        switch(rotateAction)
        {
            case 1:
                transform.Rotate(0.0f, -5.0f, 0.0f);
                break;
            case 2:
                transform.Rotate(0.0f, 5.0f, 0.0f);
                break;
        }

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
        agentRB.velocity = Vector3.zero;
        agentRB.angularVelocity = Vector3.zero;
        mazeGenerator.DestroyMaze();
        MakeMaze();
    }

    public void UpdateRewardsText()
    {
        rewardsText.text = rewards.ToString("0.000");
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
        currentMazeWidth = width;
        mazeGenerator.CreateMaze(width, width, makeInterior, obstacles);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.CompareTag("mazegoal"))
        {
            AddReward(1.0f);
            Done();
        }
    }
}
