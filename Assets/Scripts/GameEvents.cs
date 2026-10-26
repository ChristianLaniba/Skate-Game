using System;

public static class GameEvents
{
    // Obstacle -> whoever cares (PlayerHealth listens)
    public static event Action<int> OnObstacleHit;

    public static void ObstacleHit(int damage)
    {
        OnObstacleHit?.Invoke(damage);
    }

    // Optional: useful for UI / restart flow
    public static event Action OnGameOver;
    public static void TriggerGameOver() => OnGameOver?.Invoke();
}