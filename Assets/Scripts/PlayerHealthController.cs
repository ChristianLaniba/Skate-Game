using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private PlayerHealthData healthData;

    void OnEnable()
    {
        if (healthData == null)
        {
            Debug.LogError("PlayerHealthController: assign a PlayerHealthData asset!");
            return;
        }

        healthData.Initialize();                       // reset to max on scene start
        healthData.OnPlayerDied += HandlePlayerDied;
        GameEvents.OnObstacleHit += HandleObstacleHit;
    }

    void OnDisable()
    {
        if (healthData == null) return;
        healthData.OnPlayerDied -= HandlePlayerDied;
        GameEvents.OnObstacleHit -= HandleObstacleHit;
    }

    private void HandleObstacleHit(int damage)
    {
        healthData.TakeDamage(damage);
    }

    private void HandlePlayerDied()
    {
        GameEvents.TriggerGameOver();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}