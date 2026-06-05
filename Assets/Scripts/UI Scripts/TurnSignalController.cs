using UnityEngine;
using UnityEngine.UI;
public class TurnSignalController : MonoBehaviour
{
    [SerializeField] private TurnSignal LeftTS;
    [SerializeField] private TurnSignal RightTS;
    [SerializeField] private TurnSignal alarm;
    //private TurnSignal turnSignal;

    void Update()
    {
        if (LeftTS.isPressed)
        {
            RightTS.isPressed = false;
        }

        else if (RightTS.isPressed)
        {
            LeftTS.isPressed = false;
        }
        //if (alarm.isPressed)
        //{
        //    LeftTS.isPressed = true;
        //    RightTS.isPressed = true;
        //}
        //else
        //{
        //    LeftTS.isPressed = false;
        //    RightTS.isPressed = false;
        //}

        //    switch (turnSignal.isPressed)
        //    {
        //        case LeftTS.isPressed:
        //            LeftTS.isPressed = true;
        //            RightTS.isPressed = false; 
        //            break;

        //        case RightTS.isPressed:
        //            LeftTS.isPressed = false;
        //            RightTS.isPressed = true;
        //            break;

        //        case alarm.isPressed:
        //            LeftTS.isPressed = true;
        //            RightTS.isPressed = true;
        //            break;
        //    }
       }
    }
