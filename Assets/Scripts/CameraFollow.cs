using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;   // optional: leave empty to auto-detect
    public Vector3 offset = new Vector3(0f, 8f, -20f);
    public float rotationFollowSpeed = 3f;

    private float currentYaw = 0f;
    private BoardController board;

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            if (target == null) return;
        }

        if (board == null || !board.IsDoingTrick)
        {
            float targetYaw = target.eulerAngles.y;

            // Exponential smoothing — identical behavior at any frame rate
            float t = 1f - Mathf.Exp(-rotationFollowSpeed * Time.deltaTime);
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, t);
        }

        Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);
        transform.position = target.position + yawRotation * offset;
        transform.LookAt(target.position);
    }

    void FindPlayer()
    {
        board = FindAnyObjectByType<BoardController>();
        if (board == null) return;

        target = board.transform;
        currentYaw = target.eulerAngles.y;
    }
}