using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public int t; // Нарушение: неинформативное имя переменной. (Строка 5)
    private bool state;

    public void changeState()
    { // Нарушение: имя метода должно быть PascalCase. (Строка 10)
        state = !state;
        if (state == true) // Нарушение: избыточное сравнение (if(state) достаточно). (Строка 12)
            t = 0;
    }

}
