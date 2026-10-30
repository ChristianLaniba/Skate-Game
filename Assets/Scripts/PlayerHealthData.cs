using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHealthData", menuName = "Skatepark/Player Health Data")]
public class PlayerHealthData : ScriptableObject
{
    [Header("Configuration")]
    public int maxHealth = 4;
    public int damagePerHit = 1;

    [SerializeField, Tooltip("Inspector-only display. Edit through TakeDamage/Heal.")]
    private int _health;   // private backing field

    public int Health
    {
        get => _health;
        private set
        {
            int clamped = Mathf.Clamp(value, 0, maxHealth);

            // No-op if nothing changed — avoids event spam and false deaths
            if (clamped == _health) return;

            _health = clamped;
            OnHealthChanged?.Invoke(_health, maxHealth);

            if (_health == 0)
                OnPlayerDied?.Invoke();
        }
    }

    public int MaxHealth => maxHealth;

    // Events — subscribers listen to these
    public delegate void HealthChangedHandler(int current, int max);
    public delegate void PlayerDiedHandler();

    public event HealthChangedHandler OnHealthChanged;
    public event PlayerDiedHandler OnPlayerDied;

    void OnEnable()
    {
        // Reset on play / fresh scene
        _health = maxHealth;
        OnHealthChanged?.Invoke(_health, maxHealth);
    }

    public void Initialize() => Health = maxHealth;

    public void TakeDamage(int amount) => Health -= amount;   // clamped by setter

    public void Heal(int amount) => Health += amount;         // clamped by setter
}