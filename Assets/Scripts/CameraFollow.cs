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
        // Keep looking until a player exists (works if it spawns after the camera)
        if (target == null)
        {
            FindPlayer();
            if (target == null) return;
        }

        // Keep the camera's yaw steady while the board does its 360 trick
        if (board == null || !board.IsDoingTrick)
        {
            float targetYaw = target.eulerAngles.y;
            currentYaw = Mathf.LerpAngle(currentYaw, targetYaw, rotationFollowSpeed * Time.deltaTime);
        }

        Quaternion yawRotation = Quaternion.Euler(0f, currentYaw, 0f);

        // Snap position instantly, no lerp, no jitter
        transform.position = target.position + yawRotation * offset;
        transform.LookAt(target.position);
    }

    void FindPlayer()
    {
        board = FindFirstObjectByType<BoardController>();
        if (board == null) return;

        target = board.transform;
        currentYaw = target.eulerAngles.y;   // start behind the board
    }
}