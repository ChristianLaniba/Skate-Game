using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private bool won;
    private bool lost;

    void OnEnable()
    {
        GameEvents.OnGameWon  += HandleWin;
        GameEvents.OnGameOver += HandleLose;
    }

    void OnDisable()
    {
        GameEvents.OnGameWon  -= HandleWin;
        GameEvents.OnGameOver -= HandleLose;
    }

    private void HandleWin()  => won  = true;
    private void HandleLose() => lost = true;

    void OnGUI()
    {
        if (!won && !lost) return;

        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 64,
            alignment = TextAnchor.MiddleCenter
        };
        titleStyle.normal.textColor = won ? Color.green : Color.red;

        string title = won ? "YOU WIN!" : "GAME OVER";

        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), title, titleStyle);

        var subStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            alignment = TextAnchor.MiddleCenter
        };
        subStyle.normal.textColor = Color.white;

        string sub = won ? "Press R to play again" : "Restarting...";
        GUI.Label(new Rect(0f, Screen.height * 0.5f + 80f, Screen.width, 60f), sub, subStyle);
    }
}