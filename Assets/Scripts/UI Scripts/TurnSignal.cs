using UnityEngine;
using UnityEngine.UI;

public class TurnSignal : MonoBehaviour
{
    [SerializeField] private Image turnSignalPrefab;
    [SerializeField] private Button turnSignal;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color currentColor;
    [SerializeField] private Light[] lights;
    public bool isPressed;

    void Start()
    {
        turnSignalPrefab.color = currentColor;
        turnSignal.onClick.AddListener(OnPointerClick);
    }

    void Update()
    {
        foreach(Light light in lights)
        {
            if (isPressed)
            {
                Color lerpedColor = Color.Lerp(currentColor, enableColor, Mathf.PingPong(Time.time, 1));
                turnSignalPrefab.color = lerpedColor;
                light.enabled = true;
            }
            else 
            {
                turnSignalPrefab.color = currentColor;
                light.enabled = false;
            }
        }
    }

    void OnPointerClick()
    {
        isPressed = !isPressed;
    }
}
