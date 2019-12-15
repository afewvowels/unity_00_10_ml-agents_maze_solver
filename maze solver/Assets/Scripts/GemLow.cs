using UnityEngine;

public class GemLow : Gem
{
    private float reward = 0.01f;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("mazeagent"))
        {
            MazeAgent agent = collision.gameObject.GetComponent<MazeAgent>();
            agent.AddReward(reward);
            agent.rewards += reward;
            agent.UpdateRewardsText();
            agent.PickedUpTreasure();
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }
}
