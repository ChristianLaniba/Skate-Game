using System;

public static class GameEvents
{
    // Obstacle -> whoever cares (PlayerHealth listens)
    public static event Action<int> OnObstacleHit;

    public static void ObstacleHit(int damage)
    {
        OnObstacleHit?.Invoke(damage);
    }

    // Coin -> whoever cares (GameManager + HUD listen). Passes the new running total.
    public static event Action<int> OnCoinCollected;

    public static void CoinCollected(int totalCoins)
    {
        OnCoinCollected?.Invoke(totalCoins);
    }

    // Game flow
    public static event Action OnGameOver;
    public static void TriggerGameOver() => OnGameOver?.Invoke();

    public static event Action OnGameWon;
    public static void TriggerGameWon() => OnGameWon?.Invoke();
}