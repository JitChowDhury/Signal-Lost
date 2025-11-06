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
        float df = Mathf.Abs(waveGraphic.playerFrequency - waveGraphic.targetFrequency);
        float da = Mathf.Abs(waveGraphic.playerAmplitude - waveGraphic.targetAmplitude);
        float dp = Mathf.Abs(Mathf.DeltaAngle(waveGraphic.playerPhase * Mathf.Rad2Deg, waveGraphic.targetPhase * Mathf.Rad2Deg) * Mathf.Deg2Rad);

        if (df < toleranceFrequency && da < toleranceAmplitude && dp < tolerancePhase)
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
        statusText.text = "<color=#00FFAA>SIGNAL LOCKED ✓</color>";
        waveGraphic.playerColor = Color.green;
        waveGraphic.targetColor = Color.green;
        waveGraphic.SetAllDirty();

        // if (audioSource && successClip)
        //     audioSource.PlayOneShot(successClip);

        // // Optional: ambient static stop
        // if (staticSource)
        //     staticSource.Stop();

        // Automatically close puzzle after delay
        StartCoroutine(CloseAfterDelay());
    }

    IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        // Find the active WaveTerminal and close it
        WaveTerminal terminal = FindFirstObjectByType<WaveTerminal>();
        if (terminal != null)
            terminal.ClosePuzzle();
    }
}
