using UnityEngine;
using UnityEngine.UI;

public class TurnSignal : MonoBehaviour
{
    [SerializeField] private Image turnSignalPrefab;
    [SerializeField] private Button turnSignal;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color currentColor;
    public bool isPressed;
    void Start()
    {
        turnSignalPrefab.color = currentColor;
        turnSignal.onClick.AddListener(OnPointerClick);

    }

    void Update()
    {
        if (isPressed)
        {
            Color lerpedColor = Color.Lerp(currentColor, enableColor, Mathf.PingPong(Time.time, 1));
            turnSignalPrefab.color = lerpedColor;
        }
        else 
        {
            turnSignalPrefab.color = currentColor;
        }
    }

    void OnPointerClick()
    {
        isPressed = !isPressed;
    }
}
