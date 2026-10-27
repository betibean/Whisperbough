using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Optional bounds (leave at 0 to disable)")]
    [SerializeField] private float minX, maxX, minY, maxY;
    [SerializeField] private bool useBounds;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 wanted = target.position + offset;
        if (useBounds)
        {
            wanted.x = Mathf.Clamp(wanted.x, minX, maxX);
            wanted.y = Mathf.Clamp(wanted.y, minY, maxY);
        }

        transform.position = Vector3.SmoothDamp(transform.position, wanted, ref velocity, smoothTime);
    }
}