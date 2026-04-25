using UnityEngine;
using UnityEngine.UI;

public class AlarmSystem : MonoBehaviour
{
    [SerializeField] private Image alarmSystemPrefab;
    [SerializeField] private Button alarmSystem;
    [SerializeField] private Color enableColor;
    [SerializeField] private Color currentColor;
    //[SerializeField] TurnSignal LeftTS;
    //[SerializeField] TurnSignal RightTS;
    [SerializeField] private Light[] lights;
    public bool isPressed;
    void Start()
    {
        alarmSystemPrefab.color = currentColor;
        alarmSystem.onClick.AddListener(OnPointerClick);
    }

    void Update()
    {
        foreach (Light light in lights)
        {
           if (isPressed)
           {
              Color lerpedColor = Color.Lerp(currentColor, enableColor, Mathf.PingPong(Time.time, 1));
              alarmSystemPrefab.color = lerpedColor;
              light.enabled = true; 
           }
           else
           {
              alarmSystemPrefab.color = currentColor;
              light.enabled = false;
           }
        }
    }

    void OnPointerClick()
    {
        isPressed = !isPressed;
    }
}
