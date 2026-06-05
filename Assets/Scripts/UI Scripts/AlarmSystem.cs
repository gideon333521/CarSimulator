using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private Image alarmSystemPrefab;
    [SerializeField] private Button alarmSystem;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color currentColor;
    [SerializeField] private Light[] lights;
    public bool isPressed;
    private Coroutine coroutine;
    void Start()
    {
        alarmSystemPrefab.color = currentColor;
        alarmSystem.onClick.AddListener(OnPointerClick);
    }

    void Update()
    {
        foreach (Light light in lights)
        {
            light.enabled = isPressed;
            if (isPressed)
            {
                if (coroutine == null)
                {
                    coroutine = StartCoroutine(Blink());
                }
            }
            else
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                    coroutine = null;
                    alarmSystemPrefab.color = currentColor;
                }
            }
        }
    }

    IEnumerator Blink()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            alarmSystemPrefab.color = enableColor;
            yield return wait;

            alarmSystemPrefab.color = currentColor;
            yield return wait;
        }
    }

    void OnPointerClick()
    {
        isPressed = !isPressed;
    }
}
