using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Check")] 
    public Transform groundCheck;
    public LayerMask groundMask;
    
    [Header("Config")] 
    public PlayerConfig playerConfig;
    public InputReader inputReader;
    
    [HideInInspector]
    public CharacterController characterController;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = new PlayerMovement(this);
        inputReader.EnablePlayerInput();
    }

    private void Update()
    {
        playerMovement.Update();
    }

    private void OnDestroy()
    {
        playerMovement.OnDispose();
    }
}