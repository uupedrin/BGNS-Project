using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button resumeButton;

    private bool isPaused = false;

    private void Awake()
    {
        pausePanel.SetActive(false);
        resumeButton.onClick.AddListener(Resume);
        mainMenuButton.onClick.AddListener(OnMainMenuClick);
    }
    private void OnEnable()
    {
        pauseAction.action?.Enable();

    }
    private void OnDisable()
    {
        pauseAction.action?.Disable();
    }
    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(Resume);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClick);
    }

    private void Update()
    {
        if (pauseAction.action.WasPressedThisFrame())
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    private void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }


    private void OnMainMenuClick()
    {
        Time.timeScale = 1f;
        SceneHandler.Instance.LoadScene(Scenes.MainMenu);
    }
}
