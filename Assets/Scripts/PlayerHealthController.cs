using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private PlayerHealthData healthData;
    [SerializeField] private float loseScreenDuration = 1.5f;

    void OnEnable()
    {
        if (healthData == null)
        {
            Debug.LogError("PlayerHealthController: assign a PlayerHealthData asset!");
            return;
        }

        healthData.Initialize();
        healthData.OnPlayerDied  += HandlePlayerDied;
        GameEvents.OnObstacleHit += HandleObstacleHit;
    }

    void OnDisable()
    {
        if (healthData == null) return;
        healthData.OnPlayerDied  -= HandlePlayerDied;
        GameEvents.OnObstacleHit -= HandleObstacleHit;
    }

    private void HandleObstacleHit(int damage) => healthData.TakeDamage(damage);

    private void HandlePlayerDied()
    {
        // Win takes priority — no reload if the player already won
        var gm = FindAnyObjectByType<GameManager>();
        if (gm != null && gm.IsGameOver) return;

        GameEvents.TriggerGameOver();
        StartCoroutine(ReloadAfter(loseScreenDuration));
    }

    private IEnumerator ReloadAfter(float seconds)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}