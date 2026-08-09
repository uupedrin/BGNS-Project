using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    #region References
    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button loadGameButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("UI Components")]
    [SerializeField] private RectTransform buttonsContainer;
    [SerializeField]private Image skyLayer;
    [SerializeField] private Image gameLogo;
    [SerializeField] private TMP_Text savedDayText;
    [SerializeField] private CreditsUI creditsUi;

    [Header("Animation")]
    [SerializeField] private float skyLoopAnimationDuration = 10f;
    [SerializeField] private float menuItemsAnimationDuration = 1.2f;

    private Material skyMaterialInstance = null;
    #endregion

    private void OnEnable()
    {
        newGameButton?.onClick.AddListener(OnNewGameClick);
        loadGameButton?.onClick.AddListener(OnLoadGameClick);
        creditsButton?.onClick.AddListener(OnOptionsClick);
        quitButton?.onClick.AddListener(OnQuitGameClick);
    }

    private void OnDisable()
    {
        newGameButton?.onClick.RemoveListener(OnNewGameClick);
        loadGameButton?.onClick.RemoveListener(OnLoadGameClick);
        creditsButton?.onClick.RemoveListener(OnOptionsClick);
        quitButton?.onClick.RemoveListener(OnQuitGameClick);
    }

    private void OnDestroy()
    {
        if(skyMaterialInstance != null)
        {
            Destroy(skyMaterialInstance);
        }
    }

    private void Start()
    {
        SkyLoop();
        ShowMenuItems();

        bool hasSave = SaveManager.HasSave();
        loadGameButton.gameObject.SetActive(hasSave);
        if (hasSave)
        {
            SaveManager.LoadGame();
            savedDayText.text = $"Day {SaveData.current.dayData.currentDay}";
        }
    }

    #region Animation
    private void SkyLoop()
    {
        skyMaterialInstance = new Material(skyLayer.material);
        skyLayer.material = skyMaterialInstance;
        skyLayer.material.DOOffset(Vector2.right, skyLoopAnimationDuration).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void ShowMenuItems()
    {
        Vector2 logoInitialPos = gameLogo.rectTransform.anchoredPosition;
        Vector2 logoOutOfScreenPos = new Vector2(-gameLogo.rectTransform.rect.width * 2, logoInitialPos.y);
        Vector2 buttonsContainerInitialPos = buttonsContainer.anchoredPosition;
        Vector2 buttonsContainerOutOfScreenPos = new Vector2(buttonsContainer.rect.width * 2, buttonsContainerInitialPos.y);

        gameLogo.rectTransform.anchoredPosition = logoOutOfScreenPos;
        buttonsContainer.anchoredPosition = buttonsContainerOutOfScreenPos;
        gameLogo.rectTransform.DOAnchorPos(logoInitialPos, menuItemsAnimationDuration).SetEase(Ease.OutQuad);
        buttonsContainer.DOAnchorPos(buttonsContainerInitialPos, menuItemsAnimationDuration).SetEase(Ease.OutQuad);
    }
    #endregion

    #region ButtonBehaviour
    private void OnNewGameClick()
    {
        SaveData.current = new SaveData();
        SaveManager.pendingLoad = false;
        SceneHandler.Instance.LoadScene(Scenes.Game);
    }

    private void OnLoadGameClick()
    {
        SaveManager.LoadGame();
        SaveManager.pendingLoad = true;
        SceneHandler.Instance.LoadScene(Scenes.Game);
    }

    private void OnOptionsClick()
    {
        creditsUi.OpenCredits();
    }

    private void OnQuitGameClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}
