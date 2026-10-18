
using UnityEngine;

public class ObstacleGroup : MonoBehaviour
{
    private int remainingObstacles;
    private bool scoreAdded;

    public void Initialize(int obstacleCount)
    {
        remainingObstacles = obstacleCount;
        scoreAdded = false;
    }

    public void ObstaclePassed()
    {
        if (scoreAdded) return;

        remainingObstacles--;

        if (remainingObstacles <= 0)
        {
            scoreAdded = true;

            ScoreManager scoreManager =
                FindFirstObjectByType<ScoreManager>();

            if (scoreManager != null)
            {
                scoreManager.AddScore(1);
            }
        }
    }
}
