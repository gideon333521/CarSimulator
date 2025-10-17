using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] Vector3 offset;


    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        transform.position = player.transform.position + offset;
    }
}
