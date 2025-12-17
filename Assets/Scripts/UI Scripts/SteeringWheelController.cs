using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SteeringWheelController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,IDragHandler
{
    public bool Wheelbeingheld = false;   
    [SerializeField] private RectTransform SteeringWheel;
    private float WheelAngle = 0f;
    private float LastWheelAngle = 0f;
    private Vector2 center;
    [SerializeField] private float MaxSteerAngle = 200f;
    [SerializeField] private float ReleaseSpeed = 300f;
    public float OutPut;

    void Update()
    {
        if (!Wheelbeingheld && WheelAngle != 0f)
        {
            float DeltaAngle = ReleaseSpeed * Time.deltaTime;
            if (Mathf.Abs(DeltaAngle) > Mathf.Abs(WheelAngle))
                WheelAngle = 0f;
            else if (WheelAngle > 0f)
                WheelAngle -= DeltaAngle;
            else
                WheelAngle += DeltaAngle;
        }
        SteeringWheel.localEulerAngles = new Vector3(0, 0, -MaxSteerAngle * OutPut);
        OutPut = WheelAngle / MaxSteerAngle;
    }

    public void OnPointerDown(PointerEventData data)
    {
        Wheelbeingheld = true;
        center = RectTransformUtility.WorldToScreenPoint(data.pressEventCamera, SteeringWheel.position);
        LastWheelAngle = Vector2.Angle(Vector2.up, data.position - center);
    }

    public void OnDrag(PointerEventData data)
    {
        float NewAngle = Vector2.Angle(Vector2.up, data.position - center);
        if ((data.position - center).sqrMagnitude >= 400)
        {
            if (data.position.x > center.x)
                WheelAngle += NewAngle - LastWheelAngle;
            else
                WheelAngle -= NewAngle - LastWheelAngle;
        }
        WheelAngle = Mathf.Clamp(WheelAngle, -MaxSteerAngle, MaxSteerAngle);
        LastWheelAngle = NewAngle;
    }

    public void OnPointerUp(PointerEventData data)
    {
        OnDrag(data);
        Wheelbeingheld = false;
    }
}
