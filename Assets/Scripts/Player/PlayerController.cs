using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float crouchTransitionTime = 0.25f;

    private bool isGrounded;
    private bool crouching;
    private bool sprinting;

    private float speed;
    private Vector3 playerVelocity;

    private float crouchTimer = 0f;
    private float currentHeight;

    void Start()
    {

        characterController = GetComponent<CharacterController>();
        speed = walkSpeed;
        currentHeight = characterController.height;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;

        // Gravity handling
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;

        // Smooth crouch interpolation
        float targetHeight = crouching ? crouchHeight : standHeight;


        if (Mathf.Abs(characterController.height - targetHeight) > 0.01f)
        {
            crouchTimer += Time.deltaTime / crouchTransitionTime;
            crouchTimer = Mathf.Clamp01(crouchTimer);


            float startHeight = crouching ? standHeight : crouchHeight;


            characterController.height = Mathf.Lerp(startHeight, targetHeight, crouchTimer);
        }
        else
        {

            crouchTimer = 0f;
        }
        Debug.Log(characterController.height);
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 move = transform.TransformDirection(new Vector3(input.x, 0, input.y));
        characterController.Move(move * speed * Time.deltaTime);

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }

    public void Crouch()
    {
        crouching = !crouching;

        if (crouching)
        {
            speed = crouchSpeed;
            sprinting = false;
        }
        else
        {
            speed = walkSpeed;
        }
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting && !crouching)
            speed = runSpeed;
        else if (!crouching)
            speed = walkSpeed;
    }
}
