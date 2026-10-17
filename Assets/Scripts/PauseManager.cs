using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool showPausedText = true;

    public static bool IsPaused { get; private set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPaused(!IsPaused);
    }

    void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;   // freezes physics and anything time-based
        AudioListener.pause = paused;        // freezes sound too
    }

    void OnDisable()
    {
        // Safety: never leave the game frozen if this object is destroyed or the scene changes
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
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "PAUSED. Press ESC again to unpause.", style);
    }
}
