using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class FinalTransmission : MonoBehaviour
{
    [Header("Audio & Visuals")]
    public AudioSource transmissionSound;
    public Light[] powerLights;
    public Image fadeScreen;
    public Text messageText;

    [Header("Timing")]
    public float lightDelay = 0.5f;
    public float fadeDuration = 2f;
    public float messageDelay = 1f;

    public void ActivateTransmission()
    {
        StartCoroutine(TransmissionSequence());
    }

    private IEnumerator TransmissionSequence()
    {
        Debug.Log("🚀 Final Transmission Activated!");

        // 1️⃣ Power up lights one by one
        foreach (var light in powerLights)
        {
            light.enabled = true;
            yield return new WaitForSeconds(lightDelay);
        }

        // 2️⃣ Play signal sound
        if (transmissionSound) transmissionSound.Play();

        // 3️⃣ Wait for message
        yield return new WaitForSeconds(messageDelay);

        if (messageText)
        {
            messageText.text = "SIGNAL ESTABLISHED";
            messageText.color = Color.green;
        }

        // 4️⃣ Fade screen
        if (fadeScreen)
        {
            Color c = fadeScreen.color;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                c.a = Mathf.Lerp(0, 1, t / fadeDuration);
                fadeScreen.color = c;
                yield return null;
            }
        }

        // 5️⃣ End or move to next scene
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
