
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float destroyY = -6f;
    [SerializeField] private float scoreY = -0.8f;

    private bool scoreAdded;
    private ObstacleGroup obstacleGroup;

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetObstacleGroup(ObstacleGroup group)
    {
        obstacleGroup = group;
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        transform.position +=
            Vector3.down * moveSpeed * Time.deltaTime;

        if (!scoreAdded && transform.position.y <= scoreY)
        {
            scoreAdded = true;

            if (obstacleGroup != null)
            {
                obstacleGroup.ObstaclePassed();
            }
        }

        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
