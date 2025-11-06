using System.Collections;
using UnityEngine;

public class WaveTerminal : Interactable
{
    [Header("Puzzle UI")]
    public GameObject puzzleCanvas;      // shared canvas for all terminals
    public PlayerLook playerLook;
    public InputManager inputManager;

    //public AudioSource ambientHum;

    [Header("Target Signal Settings")]
    public float targetFrequency = 1f;
    public float targetAmplitude = 1f;
    public float targetPhase = 0f;

    [Header("Visual Feedback")]
    public Light terminalLight;
    public Renderer indicatorMesh;
    public Color solvedColor = Color.green;

    private bool isActive = false;
    private bool solved = false;

    void Start()
    {
        TerminalManager.Instance.RegisterTerminal(this);
    }

    protected override void Interact()
    {
        if (solved || isActive) return;
        OpenPuzzle();
    }

    void OpenPuzzle()
    {
        isActive = true;
        playerLook.enabled = false;
        inputManager.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        puzzleCanvas.SetActive(true);

        // Tell the shared WaveManager which terminal is active
        WaveManager wave = puzzleCanvas.GetComponent<WaveManager>();
        wave.SetTargetValues(targetFrequency, targetAmplitude, targetPhase, this);

        // if (ambientHum) ambientHum.Play();
    }

    public void ClosePuzzle()
    {
        isActive = false;
        puzzleCanvas.SetActive(false);
        playerLook.enabled = true;
        inputManager.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // if (ambientHum) ambientHum.Stop();
    }

    public void MarkSolved()
    {
        solved = true;



        TerminalManager.Instance.TerminalSolved(this);
    }

    public void CloseAfterDelay(float delay)
    {
        StartCoroutine(CloseRoutine(delay));

    }

    private IEnumerator CloseRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (indicatorMesh)
            indicatorMesh.material.SetColor("_EmissionColor", solvedColor * 2f);

        if (terminalLight)
            terminalLight.color = solvedColor;
        ClosePuzzle();
    }
    public bool IsSolved => solved;
}
