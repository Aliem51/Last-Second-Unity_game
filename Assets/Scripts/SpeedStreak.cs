
using UnityEngine;

public class SpeedStreak : MonoBehaviour
{
    [SerializeField] private float speed = 450f;
    [SerializeField] private float topY = 950f;
    [SerializeField] private float bottomY = -950f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.IsGameOver)
        {
            return;
        }

        Vector2 position = rectTransform.anchoredPosition;
        position.y -= speed * Time.deltaTime;

        if (position.y < bottomY)
        {
            position.y = topY;
        }

        rectTransform.anchoredPosition = position;
    }
}
