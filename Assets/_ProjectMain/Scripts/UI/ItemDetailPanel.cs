using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPanel : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private RectTransform textContainers;
    [SerializeField] private TMP_Text itemNameTxt;
    [SerializeField] private TMP_Text itemTypeTxt;
    [SerializeField] private TMP_Text descriptionTxt;

    private void Awake()
    {
        Clear();
    }

    public void Show(ItemSO item)
    {
        iconImage.sprite = item.itemSprite;
        iconImage.preserveAspect = true;
        iconImage.enabled = true;

        itemNameTxt.text = item.itemName;
        descriptionTxt.text = item.itemDescription;
        itemTypeTxt.text = item.itemType.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(textContainers);
    }

    public void Clear()
    {
        iconImage.enabled = false;
        itemNameTxt.text = string.Empty;
        descriptionTxt.text = string.Empty;
        itemTypeTxt.text = string.Empty;
    }
}
