using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public WaveGraphic waveGraphic;     // reference to WaveGraphic component on WavePanel

    [Header("Target Settings")]
    public float targetFreqMin = 0.8f;
    public float targetFreqMax = 3.0f;
    public float targetAmpMin = 0.4f;
    public float targetAmpMax = 1.2f;

    [Header("UI Controls")]
    public Slider frequencySlider;
    public Slider amplitudeSlider;
    public Slider phaseSlider;
    public TMP_Text statusText;

    [Header("Matching")]
    public float toleranceFrequency = 0.08f;
    public float toleranceAmplitude = 0.08f;
    public float tolerancePhase = 0.15f; // radians
    private WaveTerminal currentTerminal;

    bool locked = false;

    void Start()
    {
        RandomizeTarget();
        // Initialize sliders to some default
        frequencySlider.value = waveGraphic.playerFrequency;
        amplitudeSlider.value = waveGraphic.playerAmplitude;
        phaseSlider.value = waveGraphic.playerPhase;
    }

    void Update()
    {
        if (locked) return;

        // Update player wave from sliders
        waveGraphic.playerFrequency = frequencySlider.value;
        waveGraphic.playerAmplitude = amplitudeSlider.value;
        waveGraphic.playerPhase = phaseSlider.value;

        // Check match
        // Calculate differences
        float df = Mathf.Abs(waveGraphic.playerFrequency - waveGraphic.targetFrequency);
        float da = Mathf.Abs(waveGraphic.playerAmplitude - waveGraphic.targetAmplitude);

        // Normalize phase difference (wrap-around safe)
        float dp = Mathf.Abs(waveGraphic.playerPhase - waveGraphic.targetPhase);
        dp = Mathf.Repeat(dp, Mathf.PI * 2f);
        dp = Mathf.Min(dp, Mathf.PI * 2f - dp); // shortest phase distance

        // Adjust tolerances — easier to match visually
        float freqTol = 0.15f;
        float ampTol = 0.15f;
        float phaseTol = 0.25f;

        // Visual feedback while tuning
        float closeness = 1f - Mathf.Clamp01((df / freqTol + da / ampTol + dp / phaseTol) / 3f);
        Color mixColor = Color.Lerp(Color.magenta, Color.green, closeness);
        waveGraphic.playerColor = new Color(mixColor.r, mixColor.g, mixColor.b, 0.9f);
        waveGraphic.SetAllDirty();

        // Lock check
        if (df < freqTol && da < ampTol && dp < phaseTol)
        {
            locked = true;
            OnSignalLocked();
        }
        else
        {
            statusText.text = "TUNING...";
        }

        waveGraphic.SetAllDirty();
    }

    void RandomizeTarget()
    {
        waveGraphic.targetFrequency = Random.Range(targetFreqMin, targetFreqMax);
        waveGraphic.targetAmplitude = Random.Range(targetAmpMin, targetAmpMax);
        waveGraphic.targetPhase = Random.Range(0f, Mathf.PI * 2f);

        // Optionally initialize player with different values
        waveGraphic.playerFrequency = (targetFreqMin + targetFreqMax) / 2f;
        waveGraphic.playerAmplitude = (targetAmpMin + targetAmpMax) / 2f;
        waveGraphic.playerPhase = 0f;

        // Set slider ranges
        frequencySlider.minValue = 0.5f;
        frequencySlider.maxValue = 6f;
        amplitudeSlider.minValue = 0.2f;
        amplitudeSlider.maxValue = 2f;
        phaseSlider.minValue = 0f;
        phaseSlider.maxValue = Mathf.PI * 2f;
    }

    void OnSignalLocked()
    {
        if (currentTerminal != null)
        {
            currentTerminal.MarkSolved();
            currentTerminal.CloseAfterDelay(2f);
        }
        waveGraphic.playerAmplitude = waveGraphic.targetAmplitude;
        waveGraphic.playerFrequency = waveGraphic.targetFrequency;
        waveGraphic.playerPhase = waveGraphic.targetPhase;

        statusText.text = "<color=#00FFAA>SIGNAL LOCKED ✓</color>";
        waveGraphic.playerColor = Color.green;
        waveGraphic.targetColor = Color.green;
        waveGraphic.SetAllDirty();

        // if (audioSource && successClip)
        //     audioSource.PlayOneShot(successClip);

        // StartCoroutine(CloseAfterDelay());
    }


    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        // Find the active WaveTerminal and close it
        WaveTerminal terminal = FindFirstObjectByType<WaveTerminal>();
        if (terminal != null)
            terminal.ClosePuzzle();
    }
    public void SetTargetValues(float freq, float amp, float phase, WaveTerminal terminal)
    {
        waveGraphic.targetFrequency = freq;
        waveGraphic.targetAmplitude = amp;
        waveGraphic.targetPhase = phase;
        currentTerminal = terminal;

        locked = false;
        statusText.text = "TUNING...";
        waveGraphic.SetAllDirty();
    }

}
