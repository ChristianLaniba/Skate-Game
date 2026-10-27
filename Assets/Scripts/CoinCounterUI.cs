using UnityEngine;

public class CoinCounterUI : MonoBehaviour
{
    [SerializeField] private int coinsToWin = 3;

    private int current;

    void OnEnable()
    {
        GameEvents.OnCoinCollected += HandleCoin;
    }

    void OnDisable()
    {
        GameEvents.OnCoinCollected -= HandleCoin;
    }

    void Start()
    {
        // Sync with whatever the static counter is right now
        current = Coin.TotalCoins;
    }

    private void HandleCoin(int total) => current = total;

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label) { fontSize = 56 };
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(20f, 60f, 400f, 80f),
            $"Coins: {current} / {coinsToWin}", style);
    }
}