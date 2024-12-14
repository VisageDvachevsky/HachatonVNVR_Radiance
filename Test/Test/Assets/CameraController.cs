using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 2.0f; 
    private float yaw = 0.0f;
    private float pitch = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; 
    }

    void Update()
    {
        yaw += rotationSpeed * Input.GetAxis("Mouse X");
        pitch -= rotationSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -45f, 45f); 

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}