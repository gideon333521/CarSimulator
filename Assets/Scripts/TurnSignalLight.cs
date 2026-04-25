using System.Collections;
using UnityEngine;

public class TurnSignalLight : MonoBehaviour
{
    [SerializeField] private Light TSlight;
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;
    void Start()
    {
        TSlight = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        float t = 0.0f;
        float duration = Random.Range(0f, 0.1f); // Длительность мерцания
        float currIntensity = TSlight.intensity;
        float targetIntensity = TSlight.enabled ? minIntensity : maxIntensity;
        float variation = Random.Range(-20.0f, 20.0f);
        targetIntensity += variation;

        while (t < duration)
        {
            TSlight.intensity = Mathf.Lerp(currIntensity, targetIntensity, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

         StartCoroutine(Flicker());
    }
}

