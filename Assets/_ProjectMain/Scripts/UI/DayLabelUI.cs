using DG.Tweening;
using TMPro;
using UnityEngine;

public class DayLabelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayLabelTxt;
    [SerializeField] private float fadeSpeed = 1.2f;
    [SerializeField] private float holdSpeed = 1f;

    private Sequence sequence;

    private void Start()
    {
        DayNightManager.Instance.OnDayStart += ShowDayLabel;
        dayLabelTxt.alpha = 0f;
    }

    private void OnDisable()
    {
        sequence?.Kill();
        if (DayNightManager.Instance != null)
            DayNightManager.Instance.OnDayStart -= ShowDayLabel;
    }

    private void ShowDayLabel(int currentDay)
    {
        dayLabelTxt.text = $"Day {currentDay}";
        dayLabelTxt.alpha = 0f;
        sequence = DOTween.Sequence()
            .Append(dayLabelTxt.DOFade(1f, fadeSpeed))
            .AppendInterval(holdSpeed)
            .Append(dayLabelTxt.DOFade(0f, fadeSpeed));
    }
}
