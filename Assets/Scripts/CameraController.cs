using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance & Height")]
    public float distance = 10f;
    public float heightOffset = 1.5f;

    [Header("Rotation")]
    // Увеличили чувствительность с 3 до 8. Меняйте в Inspector, если нужно
    public float rotationSpeed = 8f;
    public float verticalLimit = 80f;

    [Header("Smooth Follow")]
    public float smoothTime = 0.1f;

    private float yaw = 0f;
    private float pitch = 20f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Блокируем курсор в центре экрана
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Если курсор вырвался (например, при Alt+Tab), захватываем его обратно по клику
        if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 1. Вращение мышью
        yaw += Input.GetAxis("Mouse X") * rotationSpeed;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);

        // 2. Расчет позиции
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 targetPos = target.position + Vector3.up * heightOffset;
        Vector3 desiredPosition = targetPos - rotation * Vector3.forward * distance;

        // 3. Плавное следование
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        transform.LookAt(targetPos);
    }
}