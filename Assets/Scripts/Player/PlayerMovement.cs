using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement
{
    private PlayerController controller;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private Transform groundCheck;
    private LayerMask groundLayer;
    private PlayerConfig playerConfig;
    private Transform transform;
    private Vector2 moveVec;
    private bool jumpPressed;

    public PlayerMovement(PlayerController player)
    {
        controller = player;
        characterController = controller.characterController;
        groundCheck = controller.groundCheck;
        groundLayer = controller.groundMask;
        playerConfig = controller.playerConfig;
        transform = controller.transform;
        
        RegisterInputListener();
    }

    private void RegisterInputListener()
    {
        controller.inputReader.MoveEvent += MoveHandle;
        controller.inputReader.JumpEvent += JumpHandle;
    }

    private void JumpHandle()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(playerConfig.jumpHeight * -2f * playerConfig.gravity);
        }
    }

    private void MoveHandle(Vector2 arg0)
    {
        moveVec = arg0;
    }

    public void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, playerConfig.groundDistance,groundLayer);

        if (isGrounded && velocity.y<0f)
        {
            velocity.y = -2f;
        }

        float moveX = moveVec.x;
        float moveZ = moveVec.y;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float currentSpeed =  playerConfig.moveSpeed;
        characterController.Move(move * currentSpeed * Time.deltaTime);

        velocity.y += playerConfig.gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void OnDispose()
    {
        controller.inputReader.MoveEvent -= MoveHandle;
        controller.inputReader.JumpEvent -= JumpHandle;
    }
}