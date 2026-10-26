using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    public float spinSpeed = 120f;

    public static int TotalCoins { get; private set; }

    private bool collected;

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        // Only the player's board collects it
        if (other.GetComponentInParent<BoardController>() == null) return;

        // Guard: only the first trigger per coin counts
        if (collected) return;
        collected = true;

        TotalCoins += value;
        Debug.Log($"Coins: {TotalCoins}");
        Destroy(gameObject);
    }
}