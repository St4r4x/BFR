using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class RocketController : MonoBehaviour
{
    [SerializeField] private float vitesseBase = 1f;
    [SerializeField] private float ascendSpeed = 2f;
        // Retourne la distance de descente pour ce frame (units par frame, scaled by deltaTime)
    public InputActionReference upButton;
    public bool jumpPressed;

    public GameObject canvas;
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    private bool isPaused = false;
    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";
    private bool isDead = false;
    private Rigidbody2D rb;
    private Animator animator;
    private ParticleSystem[] particleSystems;
    private ObstacleSpawner[] spawners;
    private Obstacle[] obstacles;
    private AudioSource[] sceneAudioSources;
    [Header("Score")]
    public TMP_Text inGameScoreText;
    public TMP_Text finalScoreText;
    private float elapsedTime = 0f;
    public UnityEvent onDeath;

    private void Start()
    {
        canvas.SetActive(false);
        isDead = false;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        // gather spawners, current obstacles and audio sources in scene
        spawners = UnityEngine.Object.FindObjectsByType<ObstacleSpawner>(FindObjectsSortMode.None);
        obstacles = UnityEngine.Object.FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        sceneAudioSources = UnityEngine.Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
    } 

    private void OnEnable()
    {
        upButton.action.Enable();
        upButton.action.performed += onJump;
        upButton.action.canceled += onJump;
    }
    private void OnDisable()
    {
        upButton.action.Disable();
        upButton.action.performed -= onJump;
        upButton.action.canceled -= onJump;
    }

    private void onJump(InputAction.CallbackContext context)
    {
        // set jumpPressed true when performed, false when canceled
        jumpPressed = context.phase == InputActionPhase.Performed;
    }

    private float fallSpeed()
    {
        return vitesseBase * Time.deltaTime;
    }

    private void Update()
    {
        if (isDead) return;
        // toggle pause menu with Escape (Input System)
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
        // update elapsed time and HUD
        elapsedTime += Time.deltaTime;
        if (inGameScoreText != null)
        {
            inGameScoreText.text = $"Score: {elapsedTime:F2}s";
        }
        if (jumpPressed)
        {
            // monter lorsque la touche est pressée
            transform.position += Vector3.up * ascendSpeed * Time.deltaTime;
        }
        else
        {
            // descendre sinon
            transform.position += Vector3.down * fallSpeed();
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Sol") && !isDead)
        {
            OnDeath();
        }
    }

    // centralise la logique de fin de partie pour plus de lisibilité
    // centralise la logique de fin de partie pour plus de lisibilité
    public void OnDeath()
    {
        isDead = true;
        jumpPressed = false;

        if (upButton != null && upButton.action != null)
        {
            upButton.action.Disable();
        }
        if (canvas != null)
        {
            canvas.SetActive(true);
        }

        // disable physics and animations
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }
        if (animator != null)
        {
            animator.enabled = false;
        }

        // stop particle systems
        if (particleSystems != null)
        {
            foreach (var ps in particleSystems)
            {
                if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        // disable all spawners
        if (spawners != null)
        {
            foreach (var s in spawners)
            {
                if (s != null) s.enabled = false;
            }
        }

        // stop and disable current obstacles
        var currentObstacles = UnityEngine.Object.FindObjectsByType<Obstacle>(FindObjectsSortMode.None);
        if (currentObstacles != null)
        {
            foreach (var o in currentObstacles)
            {
                if (o != null) o.enabled = false;
            }
        }

        // stop all audio sources in scene
        if (sceneAudioSources != null)
        {
            foreach (var a in sceneAudioSources)
            {
                if (a != null)
                {
                    a.Stop();
                    a.enabled = false;
                }
            }
        }

        // set final score text
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final score: {elapsedTime:F2}s";
        }

        // invoke external listeners
        if (onDeath != null)
        {
            onDeath.Invoke();
        }

        // ensure pause is disabled and timeScale restored on death
        if (isPaused)
        {
            isPaused = false;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    // Hook this to the Restart button OnClick in the UI
    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Pause menu controls
    public void TogglePause()
    {
        if (isDead) return;
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            if (upButton != null && upButton.action != null) upButton.action.Disable();
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (upButton != null && upButton.action != null) upButton.action.Enable();
        }
    }

    public void ContinueGame()
    {
        if (isPaused) TogglePause();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
            return;
        }
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
