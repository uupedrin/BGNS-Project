using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefeatUI : MonoBehaviour
{
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_Text dayReachedText;

    private void Awake()
    {
        defeatPanel.SetActive(false);
        mainMenuButton.onClick.AddListener(OnMainMenuClick);
    }

    private void Start()
    {
        HouseHealth.Instance.OnDefeat += ShowDefeat;
    }

    private void OnDestroy()
    {
        mainMenuButton.onClick.RemoveListener(OnMainMenuClick);
        if (HouseHealth.Instance != null)
            HouseHealth.Instance.OnDefeat -= ShowDefeat;
    }

    private void ShowDefeat()
    {
        defeatPanel.SetActive(true);
        dayReachedText.text = $"You survived till Day {DayNightManager.Instance.CurrentDay}";
        Time.timeScale = 0f;
    }

    private void OnMainMenuClick()
    {
        Time.timeScale = 1f;
        SceneHandler.Instance.LoadScene(Scenes.MainMenu);
    }
}
