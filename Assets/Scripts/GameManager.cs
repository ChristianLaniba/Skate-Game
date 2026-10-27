using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Objective")]
    [SerializeField] private int coinsToWin = 3;

    [Header("State (read-only in inspector)")]
    [SerializeField] private bool hasWon = false;
    [SerializeField] private bool hasLost = false;

    public int CoinsToWin => coinsToWin;
    public bool IsGameOver => hasWon || hasLost;

    void OnEnable()
    {
        GameEvents.OnCoinCollected += HandleCoinCollected;
        GameEvents.OnGameOver      += HandleGameOver;
    }

    void OnDisable()
    {
        GameEvents.OnCoinCollected -= HandleCoinCollected;
        GameEvents.OnGameOver      -= HandleGameOver;
    }

    void Start()
    {
        // Fresh run: reset the static coin counter so the HUD and objective start clean.
        Coin.ResetTotal();
    }

    private void HandleCoinCollected(int totalCoins)
    {
        if (IsGameOver) return;
        if (totalCoins < coinsToWin) return;

        hasWon = true;

        // Freeze the player the same way PauseManager does
        Time.timeScale = 0f;

        GameEvents.TriggerGameWon();
    }

    private void HandleGameOver()
    {
        if (IsGameOver) return;
        hasLost = true;
    }

    void Update()
    {
        // Allow R to restart from the win screen. (ESC → R already handled by PauseManager.)
        if (hasWon && Input.GetKeyDown(KeyCode.R))
            RestartScene();
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;   // unpause before loading
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}