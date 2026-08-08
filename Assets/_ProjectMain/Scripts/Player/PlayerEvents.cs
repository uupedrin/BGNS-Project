using System;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public Action<bool, Vector2> OnPlayerMove;
    public Action<bool, ItemInstance> OnPlayerSelectWeapon;
    public Action<Vector2> OnPlayerAim;
}
