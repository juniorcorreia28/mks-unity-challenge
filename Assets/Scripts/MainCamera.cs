using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private float zoom;
    [SerializeField]
    private BoxCollider2D sceneBounds;

    private new Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();    
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, zoom);
        Vector3 cameraPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * velocity);

        float screenAspect = (float)Screen.width / (float)Screen.height;
        float camHalfHeight = camera.orthographicSize;
        float camHalfWidth = screenAspect * camHalfHeight;

        float clampX = Mathf.Clamp(cameraPosition.x, sceneBounds.bounds.min.x + camHalfWidth, sceneBounds.bounds.max.x + -camHalfWidth);
        float clampY = Mathf.Clamp(cameraPosition.y, sceneBounds.bounds.min.y + camHalfHeight, sceneBounds.bounds.max.y + -camHalfHeight);

        Vector3 clampedPosition = new Vector3(clampX, clampY, cameraPosition.z);

        transform.position = clampedPosition;
    }
}
