using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLogic : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 10f;
    public float boostSpeed = 20f;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero; // Или rb.velocity в старых версиях
    }

    void Update()
    {
        // 1. Считываем горизонтальный ввод (WASD)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 2. Считываем вертикальный ввод (Space / Ctrl)
        float y = 0;
        if (Input.GetKey(KeyCode.Space)) y += 1f;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) y -= 1f;

        // 3. Получаем направление камеры
        Camera cam = Camera.main;
        if (cam != null)
        {
            // "Вперёд" камеры, но без вертикального наклона
            Vector3 camForward = cam.transform.forward;
            camForward.y = 0;
            camForward.Normalize();

            // "Вправо" камеры, тоже без наклона
            Vector3 camRight = cam.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            // Складываем направления: W/S = вперёд/назад камеры, A/D = вправо/влево камеры
            Vector3 horizontalMove = (camForward * v) + (camRight * h);

            // Если есть горизонтальный ввод — нормализуем, чтобы по диагонали не было быстрее
            if (horizontalMove.sqrMagnitude > 0)
            {
                horizontalMove.Normalize();
            }

            // Итоговый вектор движения: горизонталь + вертикаль
            moveInput = new Vector3(horizontalMove.x, y, horizontalMove.z);
        }
        else
        {
            // Фоллбэк, если камеры нет (на всякий случай)
            moveInput = new Vector3(h, y, v);
        }
    }

    void FixedUpdate()
    {
        // Если нет никакого ввода — стоим
        if (moveInput.sqrMagnitude == 0)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        // Определяем скорость (буст или обычная)
        float speed = Input.GetKey(KeyCode.LeftShift) ? boostSpeed : normalSpeed;

        // Применяем скорость
        rb.linearVelocity = moveInput.normalized * speed;
    }
}