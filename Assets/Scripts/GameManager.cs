
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject scoreHUD;
    [SerializeField] private TMP_Text scoreHUDText;
    [SerializeField] private GameObject speedStreaks;
    [SerializeField] private GameObject laneLines;

    [Header("Start Screen")]
    [SerializeField] private bool showStartScreen = true;

    [Header("Game Over Animation")]
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private RectTransform gameOverCard;
    [SerializeField] private float animationDuration = 0.35f;
    [SerializeField] private float startScale = 0.85f;

    public bool IsGameOver { get; private set; }
    public bool IsGameStarted { get; private set; }

    private static bool startImmediatelyAfterRetry;
    private static bool forceStartScreenOnNextLoad;

    private Vector3 cardOriginalScale;
    private Canvas sceneCanvas;
    private GameObject startScreen;
    private GameObject hudHomeButton;
    private RectTransform hudHomeButtonRect;
    private Coroutine gameOverAnimation;
    private Rect lastSafeArea;
    private int lastScreenWidth;
    private int lastScreenHeight;
    private bool shouldShowStartScreen;
    private bool isRestarting;
    private bool isReturningHome;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsGameOver = false;
        shouldShowStartScreen = forceStartScreenOnNextLoad ||
                                (showStartScreen && !startImmediatelyAfterRetry);
        forceStartScreenOnNextLoad = false;
        startImmediatelyAfterRetry = false;
        IsGameStarted = !shouldShowStartScreen;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        sceneCanvas = FindFirstObjectByType<Canvas>();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverPanel не назначен в GameManager!");
        }

        SetScoreHUDVisible(IsGameStarted);

        if (shouldShowStartScreen)
            ShowStartScreen();

        if (gameOverCard != null)
            cardOriginalScale = gameOverCard.localScale;

        if (sceneCanvas != null)
        {
            hudHomeButton = StartScreenUI.CreateHomeButton(
                sceneCanvas.transform,
                ReturnToMainMenu,
                "HUDHomeButton",
                new Vector2(90f, 90f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f));
            hudHomeButtonRect = hudHomeButton.GetComponent<RectTransform>();
            PositionHUDHomeButton();
            hudHomeButton.SetActive(IsGameStarted && !IsGameOver);

            if (gameOverPanel != null)
            {
                StartScreenUI.CreateHomeButton(
                    gameOverPanel.transform,
                    ReturnToMainMenu,
                    "GameOverHomeButton",
                    new Vector2(560f, 96f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0f, -378f));
            }
        }
    }

    private void Update()
    {
        if (hudHomeButtonRect == null)
            return;

        if (lastSafeArea != Screen.safeArea ||
            lastScreenWidth != Screen.width ||
            lastScreenHeight != Screen.height)
        {
            PositionHUDHomeButton();
        }
    }

    private void PositionHUDHomeButton()
    {
        if (hudHomeButtonRect == null)
            return;

        Rect safeArea = Screen.safeArea;
        if (safeArea.width <= 0f || safeArea.height <= 0f)
            safeArea = new Rect(0f, 0f, Screen.width, Screen.height);

        hudHomeButtonRect.anchorMin = new Vector2(
            safeArea.xMax / Screen.width,
            safeArea.yMax / Screen.height);
        hudHomeButtonRect.anchorMax = hudHomeButtonRect.anchorMin;
        hudHomeButtonRect.pivot = new Vector2(1f, 1f);
        hudHomeButtonRect.anchoredPosition = new Vector2(-52f, -52f);

        lastSafeArea = Screen.safeArea;
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;
    }

    private void ShowStartScreen()
    {
        if (startScreen != null)
            return;

        if (sceneCanvas == null)
        {
            Debug.LogError("Cannot show the start screen because this scene has no Canvas.");
            IsGameStarted = true;
            SetGameplayVisualsVisible(true);
            Time.timeScale = 1f;
            return;
        }

        startScreen = StartScreenUI.Create(sceneCanvas.transform, BeginGame);
        IsGameStarted = false;
        SetGameplayVisualsVisible(false);
        Time.timeScale = 0f;
    }

    public void BeginGame()
    {
        if (IsGameStarted || IsGameOver)
            return;

        IsGameStarted = true;
        if (startScreen != null)
            Destroy(startScreen);

        SetGameplayVisualsVisible(true);
        Time.timeScale = 1f;
    }

    private void SetGameplayVisualsVisible(bool visible)
    {
        SetScoreHUDVisible(visible);
        if (laneLines != null) laneLines.SetActive(visible);
        if (speedStreaks != null) speedStreaks.SetActive(visible);
        if (hudHomeButton != null) hudHomeButton.SetActive(visible);
    }

    private void SetScoreHUDVisible(bool visible)
    {
        if (scoreHUDText != null)
            scoreHUDText.enabled = visible;

        if (scoreHUD != null)
            scoreHUD.SetActive(visible);
    }

    public void GameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;

        ScoreManager scoreManager =
            FindFirstObjectByType<ScoreManager>();

        if (scoreManager != null)
        {
            if (finalScoreText != null)
                finalScoreText.text = "YOUR SCORE: " + scoreManager.Score;

            if (bestScoreText != null)
                bestScoreText.text = "BEST SCORE: " + scoreManager.BestScore;
        }

        SetGameplayVisualsVisible(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Flush the hidden HUD before the Game Over fade begins.
        Canvas.ForceUpdateCanvases();

        // Подготавливаем начальное состояние анимации.
        if (panelCanvasGroup != null)
            panelCanvasGroup.alpha = 0f;

        if (gameOverCard != null)
            gameOverCard.localScale = cardOriginalScale * startScale;

        // Останавливаем игру, но анимация UI продолжится.
        Time.timeScale = 0f;

        if (panelCanvasGroup != null && gameOverCard != null)
            gameOverAnimation = StartCoroutine(AnimateGameOver());
        else
            Debug.LogWarning(
                "Для анимации назначь Panel Canvas Group и Game Over Card."
            );

        Debug.Log("GAME OVER");
    }

    private IEnumerator AnimateGameOver()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            // Плавное замедление в конце анимации.
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            panelCanvasGroup.alpha = easedT;
            gameOverCard.localScale = Vector3.Lerp(
                cardOriginalScale * startScale,
                cardOriginalScale,
                easedT
            );

            yield return null;
        }

        panelCanvasGroup.alpha = 1f;
        gameOverCard.localScale = cardOriginalScale;
        gameOverAnimation = null;
    }

    public void RestartGame()
    {
        if (isRestarting || isReturningHome || !IsGameOver)
            return;

        isRestarting = true;
        forceStartScreenOnNextLoad = false;
        startImmediatelyAfterRetry = true;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayRetryAndRestart();
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        if (isReturningHome || isRestarting)
            return;

        isReturningHome = true;
        IsGameStarted = false;
        IsGameOver = true;
        forceStartScreenOnNextLoad = true;
        startImmediatelyAfterRetry = false;

        ObstacleSpawner spawner = FindAnyObjectByType<ObstacleSpawner>();
        if (spawner != null)
            spawner.StopSpawning();

        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null)
            scoreManager.ResetCurrentScore();

        if (gameOverAnimation != null)
        {
            StopCoroutine(gameOverAnimation);
            gameOverAnimation = null;
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        SetGameplayVisualsVisible(false);
        Time.timeScale = 0f;
        ShowStartScreen();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayHomeAndReturnToMenu();
        else
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
