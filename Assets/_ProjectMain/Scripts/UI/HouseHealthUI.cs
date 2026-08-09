using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class HouseHealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] private float fillDuration = 0.2f;

    private void Start()
    {
        HouseHealth.Instance.OnHealthChanged += SetFill;
        SetFill(HouseHealth.Instance.CurrentHealthNormalided);
    }

    private void OnDisable()
    {
        if(HouseHealth.Instance != null)
        {
            HouseHealth.Instance.OnHealthChanged -= SetFill;
        }
    }

    private void SetFill(float normalized)
    {
        healthBar.DOKill();
        healthBar.DOFillAmount(normalized, fillDuration);
    }
}