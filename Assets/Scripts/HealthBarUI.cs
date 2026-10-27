using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private PlayerHealthData healthData;
    [SerializeField] private Vector2 barSize = new Vector2(300f, 28f);
    [SerializeField] private Vector2 barPosition = new Vector2(20f, 20f);
    [SerializeField] private Color fullColor = new Color(0.2f, 0.9f, 0.3f);
    [SerializeField] private Color emptyColor = new Color(0.85f, 0.2f, 0.2f);

    private int currentHealth;
    private int maxHealth = 4;

    void OnEnable()
    {
        if (healthData == null) return;
        healthData.OnHealthChanged += HandleHealthChanged;
        currentHealth = healthData.Health;
        maxHealth = healthData.MaxHealth;
    }

    void OnDisable()
    {
        if (healthData == null) return;
        healthData.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(int current, int max)
    {
        currentHealth = current;
        maxHealth = max;
    }

    void OnGUI()
    {
        if (maxHealth <= 0) return;

        float segmentWidth = barSize.x / maxHealth;
        for (int i = 0; i < maxHealth; i++)
        {
            Rect seg = new Rect(barPosition.x + i * segmentWidth, barPosition.y, segmentWidth - 2f, barSize.y);
            bool filled = i < currentHealth;

            var old = GUI.color;
            GUI.color = filled ? fullColor : emptyColor;
            GUI.DrawTexture(seg, Texture2D.whiteTexture);
            GUI.color = old;
        }
    }
}