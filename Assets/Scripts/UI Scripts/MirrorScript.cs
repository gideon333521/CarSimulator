using UnityEngine;
using UnityEngine.UI;

public class MirrorScript : MonoBehaviour
{
    private AutomatTransmission automat;
    [SerializeField] private GameObject LeftMirror;
    [SerializeField] private GameObject RightMirror;
    [SerializeField] private Button btn;

    private bool isPressed;
    void Start()
    {
        btn.onClick.AddListener(OnPointerClick);
    }

     void Update()
     {
        if (isPressed)
        {
            EnableMirror();
        }
        else
        {
            DisableMirror();
        }
     }

    void EnableMirror()
    {
        LeftMirror.SetActive(true);
        RightMirror.SetActive(true);
    }

    void DisableMirror()
    {
        LeftMirror.SetActive(false);
        RightMirror.SetActive(false);
    }

    void OnPointerClick()
    {
        isPressed = !isPressed;
    }
}
