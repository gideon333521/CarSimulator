using UnityEngine;

public class CarDebug : MonoBehaviour
{
    void Start()
    {
        CheckCarComponents();
    }

    void CheckCarComponents()
    {
        // Проверяем Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("❌ Отсутствует Rigidbody на автомобиле!");
            return;
        }
        else
        {
            Debug.Log($"✅ Rigidbody найден. Mass: {rb.mass}, IsKinematic: {rb.isKinematic}");
        }

        // Проверяем Wheel Colliders
        WheelCollider[] wheelColliders = GetComponentsInChildren<WheelCollider>();
        if (wheelColliders.Length == 0)
        {
            Debug.LogError("❌ Не найдены Wheel Colliders!");
            return;
        }
        else
        {
            Debug.Log($"✅ Найдено Wheel Colliders: {wheelColliders.Length}");
        }

        // Проверяем наш контроллер
        CarController controller = GetComponent<CarController>();
        if (controller == null)
        {
            Debug.LogError("❌ Отсутствует CarController компонент!");
            return;
        }
        else
        {
            Debug.Log("✅ CarController найден");
        }

        // Проверяем настройки коллайдеров
        foreach (WheelCollider wheel in wheelColliders)
        {
            Debug.Log($"🎯 {wheel.name}: MotorTorque: {wheel.motorTorque}, BrakeTorque: {wheel.brakeTorque}, SteerAngle: {wheel.steerAngle}");
        }
    }
}
