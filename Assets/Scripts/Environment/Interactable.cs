using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;
    public bool useEvents;
    public void BaseInteract()
    {
        if (useEvents)
        {
            GetComponent<InteractionEvents>().OnInteract.Invoke();
        }
        Interact();
    }
    protected virtual void Interact()
    {

    }

}
