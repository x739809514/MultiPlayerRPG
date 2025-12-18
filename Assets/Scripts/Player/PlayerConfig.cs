using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Custom/Player/PlayerConfig")]
public class PlayerConfig: ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float sprintSpeed = 8f;
    [SerializeField] public float jumpHeight = 2f;
    [SerializeField] public float gravity = -9.81f;   
    [SerializeField] public float groundDistance = 0.4f;
}