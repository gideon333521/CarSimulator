using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Transform camera_Car;
    [SerializeField] Vector3 offset;


    void Start()
    {
        offset = camera_Car.transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        camera_Car.transform.position = player.transform.position + offset;
    }
}
