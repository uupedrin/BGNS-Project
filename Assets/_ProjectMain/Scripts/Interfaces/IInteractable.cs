using UnityEngine;

public interface IInteractable
{
    public void Interact(GameObject interactor);
    public void Highlight(bool active);
    public bool ValidForInteraction();
}
