using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scenes")]
    public string gameSceneName = "GameScene";
    [Header("UI")]
    public GameObject settingsPanel;
    [Header("Prefabs")]
    public GameObject settingsPanelPrefab;
    private GameObject instantiatedSettingsPanel;

    public void Play()
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            GameStartState.autostart = true;
            SceneManager.LoadScene(gameSceneName);
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            return;
        }

        if (settingsPanelPrefab != null)
        {
            if (instantiatedSettingsPanel == null)
            {
                instantiatedSettingsPanel = Instantiate(settingsPanelPrefab, transform);
            }
            instantiatedSettingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            return;
        }

        if (instantiatedSettingsPanel != null)
        {
            Destroy(instantiatedSettingsPanel);
            instantiatedSettingsPanel = null;
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
