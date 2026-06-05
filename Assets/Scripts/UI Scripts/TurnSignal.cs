using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TurnSignal : MonoBehaviour
{
    [SerializeField] private Image turnSignalPrefab;
    [SerializeField] private Button turnSignal;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color currentColor;
    [SerializeField] private TurnSignalLight[] lights;
    public bool isPressed;
    private Coroutine coroutine;

    void Start()
    {
        turnSignalPrefab.color = currentColor;
        turnSignal.onClick.AddListener(OnPointerClick);
    }

    void Update()
    {
        foreach(TurnSignalLight light in lights)
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
                    turnSignalPrefab.color = currentColor;
                }
            }
        }
    }

    IEnumerator Blink()
    {
        var wait = new WaitForSeconds(1);
        while (true)
        {
            turnSignalPrefab.color = enableColor;
            yield return wait;

            turnSignalPrefab.color = currentColor;
            yield return wait;
        }
    }

    void OnPointerClick()
    {
        isPressed = !isPressed;

    }
}
