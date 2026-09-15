using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTarget;

    [Header("Crouch Settings")] //Challenge 4
    [SerializeField] private float crouchSpeed = 2f;

    private bool isCrouching;

    public bool IsCrouching => isCrouching;

    private void Update()
    {
        // Tahan CTRL untuk crouch (Challenge 4)
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        MovePlayer();
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        float currentSpeed = //Challenge 4
            isCrouching
            ? crouchSpeed
            : moveSpeed;

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical).normalized;

        if (inputDirection == Vector3.zero)
            return;

        // Ambil arah depan kamera
        Vector3 camForward = cameraTarget.forward;

        // Ambil arah kanan kamera
        Vector3 camRight = cameraTarget.right;

        // Abaikan rotasi vertikal kamera
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Hitung arah gerak berdasarkan arah kamera
        Vector3 movement =
            camForward * inputDirection.z +
            camRight * inputDirection.x;

        movement.Normalize();

        // Gerakkan player
        transform.position +=
            movement *
            currentSpeed *
            Time.deltaTime;

        // Putar player menghadap arah gerak
        Quaternion targetRotation =
            Quaternion.LookRotation(movement);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }
}