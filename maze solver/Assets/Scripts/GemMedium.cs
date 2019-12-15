using UnityEngine;

public class GemMedium : Gem
{
    private float reward = 0.1f;
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
