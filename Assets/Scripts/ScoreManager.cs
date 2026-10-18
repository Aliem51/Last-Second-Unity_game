
using TMPro;
using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;

    [Header("Score Animation")]
    [SerializeField] private float animationScale = 1.25f;
    [SerializeField] private float animationDuration = 0.15f;

    [Header("Score Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color greenColor = new Color(0.2f, 0.9f, 0.4f);
    [SerializeField] private Color goldColor = new Color(1f, 0.7f, 0.1f);
    [SerializeField] private Color redColor = new Color(1f, 0.25f, 0.25f);

    public int Score { get; private set; }
    public int BestScore { get; private set; }

    private const string BestScoreKey = "LastSecond_BestScore";

    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    private void Start()
    {
        Score = 0;
        BestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        if (scoreText != null)
        {
            originalScale = scoreText.rectTransform.localScale;
        }

        UpdateScoreText();
        UpdateScoreColor();
    }

    public void AddScore(int amount)
    {
        Score += amount;

        // Звук получения очков
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayScore();
        }

        if (Score > BestScore)
        {
            BestScore = Score;
            PlayerPrefs.SetInt(BestScoreKey, BestScore);
            PlayerPrefs.Save();
        }

        UpdateScoreText();
        UpdateScoreColor();
        PlayScoreAnimation();
    }

    public void ResetCurrentScore()
    {
        Score = 0;

        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }

        if (scoreText != null)
            scoreText.rectTransform.localScale = originalScale;

        UpdateScoreText();
        UpdateScoreColor();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + Score;
        }
    }

    private void UpdateScoreColor()
    {
        if (scoreText == null) return;

        if (Score >= 30)
            scoreText.color = redColor;
        else if (Score >= 20)
            scoreText.color = goldColor;
        else if (Score >= 10)
            scoreText.color = greenColor;
        else
            scoreText.color = normalColor;
    }

    private void PlayScoreAnimation()
    {
        if (scoreText == null) return;

        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(AnimateScore());
    }

    private IEnumerator AnimateScore()
    {
        RectTransform rect = scoreText.rectTransform;

        float halfDuration = animationDuration / 2f;
        float elapsed = 0f;
        Vector3 enlargedScale = originalScale * animationScale;

        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            rect.localScale = Vector3.Lerp(originalScale, enlargedScale, t);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / halfDuration);
            rect.localScale = Vector3.Lerp(enlargedScale, originalScale, t);
            yield return null;
        }

        rect.localScale = originalScale;
        scaleCoroutine = null;
    }
}
