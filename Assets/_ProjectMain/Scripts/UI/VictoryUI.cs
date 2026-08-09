using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_Text dayReachedText;

    private void Awake()
    {
        victoryPanel.SetActive(false);
        mainMenuButton.onClick.AddListener(OnMainMenuClick);
    }

    private void Start()
    {
        DayNightManager.Instance.OnVictory += ShowVictory;
    }

    private void OnDestroy()
    {
        mainMenuButton.onClick.RemoveListener(OnMainMenuClick);
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnVictory -= ShowVictory;
    }

    private void ShowVictory()
    {
        victoryPanel.SetActive(true);
        dayReachedText.text = $"You survived all nights and left them 2 die";
        Time.timeScale = 0f;
    }

    private void OnMainMenuClick()
    {
        Time.timeScale = 1f;
        SceneHandler.Instance.LoadScene(Scenes.MainMenu);
    }
}
