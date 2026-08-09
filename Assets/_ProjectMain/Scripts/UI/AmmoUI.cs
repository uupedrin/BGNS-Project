using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private GameObject bulletsPanel;
    [SerializeField] private GameObject[] ammoIcons;

    public void SetAmmo(int currentClip)
    {
        for (int i = 0; i < ammoIcons.Length; i++)
        {
            ammoIcons[i].SetActive(i < currentClip);
        }
    }

    public void SetVisible(bool visible)
    {
        bulletsPanel.SetActive(visible);
    }
}
