using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public bool showPausedText = true;

    public static bool IsPaused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!IsPaused);

        // Restart only while paused (ESC → R). Free safety net for noclip / soft-locks.
        if (IsPaused && Input.GetKeyDown(KeyCode.R))
            RestartScene();
    }

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }

    void RestartScene()
    {
        // Unpause BEFORE loading, or the new scene starts frozen
        SetPaused(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDisable()
    {
        if (IsPaused) SetPaused(false);
    }

    void OnGUI()
    {
        if (!IsPaused || !showPausedText) return;

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 48,
            alignment = TextAnchor.MiddleCenter
        };
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height),
            "PAUSED\nPress ESC to unpause\nPress R to restart", style);
    }
}