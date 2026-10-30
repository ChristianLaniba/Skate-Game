using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnVolume : MonoBehaviour
{
    [Header("Where to respawn")]
    [Tooltip("Empty Transform marking the respawn point. " +
             "If left empty, the player's position at Start() is used.")]
    [SerializeField] private Transform respawnPoint;

    [Header("Optional safety offset")]
    [Tooltip("Extra vertical offset applied on respawn to avoid re-triggering the volume.")]
    [SerializeField] private float respawnHeightOffset = 1f;

    [Header("Velocity reset")]
    [SerializeField] private bool resetVelocity = true;
    [SerializeField] private bool resetAngularVelocity = true;

    private Rigidbody playerRb;
    private Vector3 fallbackRespawnPosition;
    private bool hasFallback;

    void Reset()
    {
        // Trigger when component is first added
        GetComponent<Collider>().isTrigger = true;
    }

    void Start()
    {
        // Capture the player's starting position as a fallback
        var board = FindAnyObjectByType<BoardController>();
        if (board != null)
        {
            playerRb = board.GetComponent<Rigidbody>();
            fallbackRespawnPosition = board.transform.position + Vector3.up * respawnHeightOffset;
            hasFallback = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var board = other.GetComponentInParent<BoardController>();
        if (board == null) return;

        // Prefer the assigned respawn point; fall back to the captured start position
        Vector3 target;
        if (respawnPoint != null)
            target = respawnPoint.position + Vector3.up * respawnHeightOffset;
        else if (hasFallback)
            target = fallbackRespawnPosition;
        else
            return;

        // Freeze physics briefly to avoid re-triggering from the current velocity
        if (playerRb != null)
        {
            playerRb.position = target;
            if (resetVelocity)        playerRb.linearVelocity  = Vector3.zero;
            if (resetAngularVelocity) playerRb.angularVelocity = Vector3.zero;
        }
        else
        {
            board.transform.position = target;
        }
    }
}