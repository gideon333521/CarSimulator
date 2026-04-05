using UnityEngine;

public class Camera : MonoBehaviour
{
    private float moveSmoothness;
    private float rotSmoothness;

    private Vector3 moveOffset;
    private Vector3 rotOffset;

    [SerializeField] private Transform carTarget;

     void FixedUpdate()
     {
        FollowTraget();
     }

    void FollowTraget()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        Vector3 TargetPos = new Vector3();
        TargetPos = carTarget.TransformPoint(moveOffset);
        transform.position = Vector3.Lerp(transform.position, TargetPos, moveSmoothness * Time.deltaTime);
    }

    void HandleRotation()
    {
        var direction = carTarget.position - transform.position;
        var rotation = new Quaternion();
        rotation = Quaternion.LookRotation(direction + rotOffset, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotSmoothness * Time.deltaTime);
    }
}
