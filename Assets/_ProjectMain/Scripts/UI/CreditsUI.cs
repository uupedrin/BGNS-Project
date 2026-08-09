using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        creditsPanel.SetActive(false);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnCloseButtonClick);
    }

    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    private void OnCloseButtonClick()
    {
        creditsPanel.SetActive(false);
    }
}
