using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; 
    private Vector3 offset;

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, target.position.z + offset.z);
            targetPosition.x = Mathf.Lerp(transform.position.x, target.position.x, Time.deltaTime * 5f);
            transform.position = targetPosition;
        }
    }
}