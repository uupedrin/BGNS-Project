using System;
using UnityEngine;

public class WeaponAnimationEvents : MonoBehaviour
{
    public event Action OnReloadComplete;
    public event Action OnAmmoInsert;

    public void ReloadComplete() => OnReloadComplete?.Invoke();
    public void AmmoInsert() => OnAmmoInsert?.Invoke();
}
