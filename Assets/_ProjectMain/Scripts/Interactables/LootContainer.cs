using UnityEngine;

public class LootContainer : MonoBehaviour, IInteractable
{
    public void Interact(GameObject interactor)
    {
        Debug.Log($"Interacted with {gameObject.name}");
    }
}
