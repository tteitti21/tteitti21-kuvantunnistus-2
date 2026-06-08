using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 10f;      // Base movement speed
    public float fastSpeed = 50f;      // Speed when holding Shift
    public float sensitivity = 2f;     // Mouse look sensitivity

    private float rotationX;
    private float rotationY;

    void Update()
    {
        // ----- Mouse look -----
        if (Input.GetMouseButton(1)) // Right mouse button held
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivity;
            rotationY -= Input.GetAxis("Mouse Y") * sensitivity;
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }

        // ----- Movement -----
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;  // keep horizontal
        right.y = 0f;    // keep horizontal

        if (Input.GetKey(KeyCode.W)) transform.position += forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) transform.position -= forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) transform.position -= right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) transform.position += right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E)) transform.position += Vector3.up * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) transform.position -= Vector3.up * speed * Time.deltaTime;
    }
}
