using UnityEngine;

public class Keypad : Interactable
{
    [SerializeField] private GameObject door;
    private bool isOpen;
    protected override void Interact()
    {
        isOpen = !isOpen;
        door.GetComponent<Animator>().SetBool("isOpen", isOpen);
    }

}
