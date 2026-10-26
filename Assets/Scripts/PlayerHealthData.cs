using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHealthData", menuName = "Skatepark/Player Health Data")]
public class PlayerHealthData : ScriptableObject
{
    [Header("Configuration")]
    public int maxHealth = 4;
    public int damagePerHit = 1;   // 1 of 4 quarters

    [Header("Runtime (read-only in inspector)")]
    [SerializeField] private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    // Events — subscribers listen to these
    public delegate void HealthChangedHandler(int current, int max);
    public delegate void PlayerDiedHandler();

    public event HealthChangedHandler OnHealthChanged;
    public event PlayerDiedHandler OnPlayerDied;

    void OnEnable()
    {
        // Reset on play (and also acts as a safe default)
        currentHealth = maxHealth;
    }

    public void Initialize()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
            OnPlayerDied?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}