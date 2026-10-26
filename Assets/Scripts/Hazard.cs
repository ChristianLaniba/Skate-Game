using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HazardObstacle : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitCooldown = 1.5f;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 12f;   // horizontal
    [SerializeField] private float knockbackUpForce = 4f;  // upward pop

    private float lastHitTime = -999f;

    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var board = other.GetComponentInParent<BoardController>();
        if (board == null) return;

        if (Time.time - lastHitTime < hitCooldown) return;
        lastHitTime = Time.time;

        // Push the board away from this hazard
        Rigidbody rb = board.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 away = board.transform.position - transform.position;
            away.y = 0f;

            if (away.sqrMagnitude < 0.001f)
                away = -board.transform.forward;

            away.Normalize();

            Vector3 velocity = away * knockbackForce;
            velocity.y = knockbackUpForce;
            rb.linearVelocity = velocity;
        }

        // Fire the damage event
        GameEvents.ObstacleHit(damage);
    }
}