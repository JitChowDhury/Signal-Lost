using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class TerminalManager : MonoBehaviour
{
    public static TerminalManager Instance;

    [Header("Terminals in Level")]
    public List<WaveTerminal> terminals = new List<WaveTerminal>();

    [Header("Progress Events")]
    public UnityEvent OnAllTerminalsActivated;

    private int solvedCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterTerminal(WaveTerminal terminal)
    {
        if (!terminals.Contains(terminal))
            terminals.Add(terminal);
    }

    public void TerminalSolved(WaveTerminal terminal)
    {
        if (!terminals.Contains(terminal)) return;

        solvedCount++;
        Debug.Log($"Terminal solved: {solvedCount}/{terminals.Count}");

        if (solvedCount >= terminals.Count)
            OnAllTerminalsActivated.Invoke();
    }
}
