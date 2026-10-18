
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip retrySound;
    public AudioClip scoreSound;
    public AudioClip collisionSound;

    private bool isSceneReloadPending;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayRetry()
    {
        PlaySound(retrySound);
    }

    public void PlayScore()
    {
        PlaySound(scoreSound);
    }

    public void PlayCollision()
    {
        PlaySound(collisionSound);
    }
    public void PlayRetryAndRestart()
    {
        PlayClickAndReloadScene();
    }

    public void PlayHomeAndReturnToMenu()
    {
        PlayClickAndReloadScene();
    }

    private void PlayClickAndReloadScene()
    {
        if (isSceneReloadPending)
            return;

        isSceneReloadPending = true;

        if (retrySound == null || audioSource == null)
        {
            RestartScene();
            return;
        }

        audioSource.PlayOneShot(retrySound);
        StartCoroutine(RestartAfterSound(retrySound.length));
    }

    private System.Collections.IEnumerator RestartAfterSound(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        RestartScene();
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }
}
