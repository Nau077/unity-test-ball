using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3.5f;
    public float runSpeed = 6.5f;
    public float acceleration = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    private CharacterController cc;
    private Vector2 moveInput;
    private Vector3 velocity;
    private float currentSpeed;
    private bool isSprinting;

    // NEW: Animator
    private Animator animator;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); // стоит на Magician_RIO_Unity
    }

    void OnMove(InputValue v) => moveInput = v.Get<Vector2>();
    void OnSprint(InputValue v) => isSprinting = v.isPressed;

    void OnJump()
    {
        if (cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpHeight);
            // опционально: если сделаешь триггер Jump в Animator
            // if (animator) animator.SetTrigger("Jump");
        }
    }

    void Update()
    {
        // направление относительно камеры
        Vector3 fwd = Camera.main ? Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
        Vector3 right = Camera.main ? Camera.main.transform.right : Vector3.right;
        Vector3 inputDir = (fwd * moveInput.y + right * moveInput.x).normalized;

        float targetSpeed = (isSprinting ? runSpeed : walkSpeed) * (inputDir.sqrMagnitude > 0 ? 1f : 0f);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);

        Vector3 move = inputDir * currentSpeed;

        // гравитация
        if (cc.isGrounded && velocity.y < 0f) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;

        cc.Move((move + velocity) * Time.deltaTime);

        // поворот по направлению движения
        if (move.sqrMagnitude > 0.0001f)
        {
            Quaternion look = Quaternion.LookRotation(move, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, 12f * Time.deltaTime);
        }

        // NEW: обновляем параметр speed в Animator (0..1)
        if (animator)
        {
            float animSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
            animator.SetFloat("speed", animSpeed);
        }
        animator.SetFloat("speed", currentSpeed);
    }
}
