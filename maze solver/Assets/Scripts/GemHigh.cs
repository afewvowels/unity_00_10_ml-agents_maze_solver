using UnityEngine;

public class GemHigh : Gem
{
    private float reward = 0.3f;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("mazeagent"))
        {
            collision.gameObject.GetComponent<MazeAgent>().AddReward(reward);
            collision.gameObject.GetComponent<MazeAgent>().rewards += reward;
            collision.gameObject.GetComponent<MazeAgent>().UpdateRewardsText();
            StopAllCoroutines();
            Destroy(this.gameObject);
        }
    }
}
