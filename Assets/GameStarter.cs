using UnityEngine;

// Simple global state to indicate the game was started from the main menu
public static class GameStartState
{
    public static bool autostart = false;
}

public class GameStarter : MonoBehaviour
{
    [Header("UI")]
    // optional UI shown when the scene is loaded directly (not via MainMenu)
    public GameObject startPanel;

    private void Awake()
    {
        if (GameStartState.autostart)
        {
            GameStartState.autostart = false;
            StartGame();
        }
        else
        {
            PauseForStart();
        }
    }

    private void PauseForStart()
    {
        Time.timeScale = 0f;
        if (startPanel != null) startPanel.SetActive(true);
        var rc = FindObjectOfType<RocketController>();
        if (rc != null && rc.upButton != null && rc.upButton.action != null)
        {
            rc.upButton.action.Disable();
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        if (startPanel != null) startPanel.SetActive(false);
        var rc = FindObjectOfType<RocketController>();
        if (rc != null && rc.upButton != null && rc.upButton.action != null)
        {
            rc.upButton.action.Enable();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
