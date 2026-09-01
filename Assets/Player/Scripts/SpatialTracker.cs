using UnityEngine;
using UnityEngine.InputSystem;

public class SpatialTracker : MonoBehaviour
{
    public Camera camera;
    public Vector3 playerPosition;
    public Vector2 cursorDirection;

    void Update()
    {
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = camera.ScreenToWorldPoint(mouseScreenPos);

        cursorDirection = (mouseWorldPos - transform.position).normalized;
        playerPosition = transform.position;
    }
}
