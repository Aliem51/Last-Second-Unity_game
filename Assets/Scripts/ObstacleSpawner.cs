
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 1.7f;
    [SerializeField] private float spawnY = 5f;
    [SerializeField] private float laneDistance = 1.5f;

    [Header("Difficulty")]
    [SerializeField] private float startSpeed = 3.5f;
    [SerializeField] private float speedIncreasePerPoint = 0.08f;
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField, Range(0f, 1f)]
    private float initialDoubleObstacleChance = 0.2f;

    private float currentSpeed;
    private Coroutine spawnRoutine;

    private void Start()
    {
        currentSpeed = startSpeed;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine == null)
            return;

        StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    private IEnumerator SpawnLoop()
    {
        // Даём игроку немного времени освоиться.
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.IsGameOver)
                yield break;

            UpdateDifficulty();
            SpawnObstacleGroup();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void UpdateDifficulty()
    {
        ScoreManager scoreManager =
            FindFirstObjectByType<ScoreManager>();

        int score = scoreManager != null ? scoreManager.Score : 0;

        currentSpeed = Mathf.Min(
            startSpeed + score * speedIncreasePerPoint,
            maxSpeed
        );
    }

    
private void SpawnObstacleGroup()
{
    if (obstaclePrefab == null)
    {
        Debug.LogError("Obstacle Prefab не назначен!");
        return;
    }

    List<int> lanes = new List<int> { -1, 0, 1 };

    for (int i = 0; i < lanes.Count; i++)
    {
        int randomIndex = Random.Range(i, lanes.Count);

        int temp = lanes[i];
        lanes[i] = lanes[randomIndex];
        lanes[randomIndex] = temp;
    }

    ScoreManager scoreManager =
        FindFirstObjectByType<ScoreManager>();

    int score = scoreManager != null ? scoreManager.Score : 0;

    float doubleChance =
        Mathf.Clamp01(initialDoubleObstacleChance + score * 0.01f);

    int obstacleCount = Random.value < doubleChance ? 2 : 1;

    GameObject groupObject = new GameObject("ObstacleGroup");
    ObstacleGroup group = groupObject.AddComponent<ObstacleGroup>();
    group.Initialize(obstacleCount);

    for (int i = 0; i < obstacleCount; i++)
    {
        float x = lanes[i] * laneDistance;

        Vector3 position = new Vector3(x, spawnY, 0f);

        GameObject obstacle = Instantiate(
            obstaclePrefab,
            position,
            Quaternion.identity
        );

        ObstacleMover mover = obstacle.GetComponent<ObstacleMover>();

        if (mover != null)
        {
            mover.SetMoveSpeed(currentSpeed);
            mover.SetObstacleGroup(group);
        }
    }
}

}
