using UnityEngine;

public class WaveTerminal : Interactable
{
    [Header("Puzzle UI")]
    public GameObject puzzleCanvas;        // assign your wave puzzle UI
    public PlayerLook playerLook;          // assign player look script
    public InputManager inputManager;      // assign player input
    // public AudioSource ambientHum;         // optional – looping hum sound

    private bool isActive = false;

    protected override void Interact()
    {
        if (!isActive)
            OpenPuzzle();
    }

    void OpenPuzzle()
    {
        isActive = true;

        // Disable player control
        playerLook.enabled = false;
        inputManager.enabled = false;

        // Enable UI
        puzzleCanvas.SetActive(true);

        // Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Optional sound
        // if (ambientHum) ambientHum.Play();
    }

    public void ClosePuzzle()
    {
        isActive = false;
        puzzleCanvas.SetActive(false);

        // Re-enable player control
        playerLook.enabled = true;
        inputManager.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // if (ambientHum) ambientHum.Stop();
    }
}
