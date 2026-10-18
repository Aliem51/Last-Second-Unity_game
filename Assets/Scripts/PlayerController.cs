
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float laneDistance = 1.5f;

    // -1 = левая дорожка, 0 = центр, 1 = правая.
    private int currentLane = 0;
    private Vector3 targetPosition;
    private bool hasCollided = false;
    private PointerEventData uiPointerEventData;
    private readonly List<RaycastResult> uiRaycastResults =
        new List<RaycastResult>();

    private void Start()
    {
        currentLane = 0;

        targetPosition = new Vector3(
            0f,
            transform.position.y,
            transform.position.z
        );

        transform.position = targetPosition;
    }

    private void Update()
    {
        // Если игра окончена, управление заблокировано.
        if (GameManager.Instance != null &&
            (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted))
            return;

        Vector2 pointerPosition;
        bool pressed = false;

        // Сначала проверяем сенсорный экран.
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            pointerPosition =
                Touchscreen.current.primaryTouch.position.ReadValue();

            pressed = true;
        }
        // Если касания нет, проверяем мышь.
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.wasPressedThisFrame)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            pressed = true;
        }
        else
        {
            return;
        }

        if (!pressed)
            return;

        if (IsPointerOverButton(pointerPosition))
            return;

        // Делим экран на три одинаковые зоны.
        float screenThird = Screen.width / 3f;

        if (pointerPosition.x < screenThird)
        {
            currentLane = -1;
        }
        else if (pointerPosition.x < screenThird * 2f)
        {
            currentLane = 0;
        }
        else
        {
            currentLane = 1;
        }

        targetPosition = new Vector3(
            currentLane * laneDistance,
            transform.position.y,
            transform.position.z
        );

        Debug.Log("Input received. Target lane: " + currentLane);
    }

    private bool IsPointerOverButton(Vector2 screenPosition)
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null)
            return false;

        if (uiPointerEventData == null)
            uiPointerEventData = new PointerEventData(eventSystem);

        uiPointerEventData.position = screenPosition;
        uiRaycastResults.Clear();
        eventSystem.RaycastAll(uiPointerEventData, uiRaycastResults);

        foreach (RaycastResult result in uiRaycastResults)
        {
            if (result.gameObject.GetComponentInParent<Button>() != null)
                return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null &&
            (GameManager.Instance.IsGameOver || !GameManager.Instance.IsGameStarted))
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Obstacle"))
            return;

        // Не допускаем повторного срабатывания столкновения.
        if (hasCollided)
            return;

        hasCollided = true;

        Debug.Log("Player collided with: " + other.name);

        // Проигрываем звук столкновения.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollision();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }
}
