using System.Collections;
using UnityEngine;

public class TurnSignalLight : MonoBehaviour
{
    [SerializeField] private Light TSlight;
    [SerializeField] private TurnSignal LeftTS;
    [SerializeField] private TurnSignal RightTS;
    [SerializeField] private AlarmSystem alarm;
    [SerializeField] private float time;
    private Coroutine coroutine;
    void Start()
    {
        TSlight = GetComponent<Light>();
        TSlight.enabled = false;
    }

    private void Update()
    {
        if (LeftTS.isPressed || RightTS.isPressed || alarm.isPressed)
        {
            if (coroutine == null)
            {
               coroutine = StartCoroutine(Flicker());
            }
        }
        else
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
                TSlight.enabled = false;
            }
        }
    }

    private IEnumerator Flicker()
    {
        var waitTime = new WaitForSeconds(time);
        while (true)
        {
            TSlight.enabled = !TSlight.enabled;
            yield return waitTime;
        }
    }
}

