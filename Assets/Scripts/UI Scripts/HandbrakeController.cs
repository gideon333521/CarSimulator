using UnityEngine;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HandbrakeController : MonoBehaviour
{
    [SerializeField] private GameObject handbrake;
    [SerializeField] private Text text;

    public bool isPressed;
    public float impact;

    void Start()
    {
        SetUpHandbrake();
    }

    void Update()
    {
        if (isPressed)
        {
            text.text = "Handbrake On";
            impact = 1;
        }
        else
        {
            text.text = "Handbrake Off";
            impact = 0;
        }
        impact = Mathf.Clamp(impact, 0, 1);
    }

    void SetUpHandbrake()
    {
        EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

        var pointerClickDown = new EventTrigger.Entry();
        pointerClickDown.eventID = EventTriggerType.PointerClick;
        pointerClickDown.callback.AddListener((e) => onClick());
        trigger.triggers.Add(pointerClickDown);
    }

    void onClick()
    {
        isPressed = !isPressed;

    }
}
