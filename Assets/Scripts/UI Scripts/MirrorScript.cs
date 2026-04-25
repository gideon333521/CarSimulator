using UnityEngine;
using UnityEngine.UI;

public class MirrorScript : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private GameObject LeftMirror;
    [SerializeField] private GameObject RightMirror;
    [SerializeField] private TurnSignal LeftTS;
    [SerializeField] private TurnSignal RightTS;
    [SerializeField] private AlarmSystem alarm;
    [SerializeField] private AutomatTransmission automat;
    private bool isPressed;
    void Start()
    {
        btn.onClick.AddListener(OnPointerClick);
    }

     void Update()
     {
        if (isPressed || automat.IsReverse()) 
        {
            LeftMirror.SetActive(true);
            RightMirror.SetActive(true);
        }
        else
        {
            LeftMirror.SetActive(false);
            RightMirror.SetActive(false);
        }

        if (LeftTS.isPressed)
        {
            LeftMirror.SetActive(true);
            RightMirror.SetActive(false);
        }

        if (RightTS.isPressed)
        {
            LeftMirror.SetActive(false);
            RightMirror.SetActive(true);
        }

        if (alarm.isPressed)
        {
            LeftMirror.SetActive(false);
            RightMirror.SetActive(false);
        }
     }

    void OnPointerClick()
    {
        isPressed = !isPressed;
    }
}
