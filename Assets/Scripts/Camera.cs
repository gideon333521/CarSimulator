using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private GameObject car;
    [SerializeField] private Transform camera_Car;
    [SerializeField] private Vector3 offset;


    void Start()
    {
        offset = camera_Car.transform.position - car.transform.position;
    }

    void LateUpdate()
    {
        camera_Car.transform.position = car.transform.position + offset;
    }
}
