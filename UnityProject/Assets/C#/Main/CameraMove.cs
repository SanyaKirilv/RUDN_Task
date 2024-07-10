using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MovableObject
{
    [Header("InputActions")]
    [SerializeField] private InputAction press;
    [SerializeField] private InputAction moveHorizontal;
    [SerializeField] private InputAction moveVertical;
    [SerializeField] private InputAction look;
    [SerializeField] private InputAction wheel;
    [SerializeField] private InputAction shift;
    [Header("Speed parameters")]
    [Range(0.01f, 10f)]
    [SerializeField] private float moveSpeed = 5f;
    [Range(0.01f, 1f)]
    [SerializeField] private float moveSpeedMultiplyer = 1f;
    [Range(0.01f, 10f)]
    [SerializeField] private float lookSpeed = 3.5f;
    [Header("Directions")]
    public Vector3 moveDirection;
    public Vector2 lookDirection;
    [Header("States")]
    public bool isRightClick;

    private float CurrentMoveSpeed => moveSpeed * moveSpeedMultiplyer;

    private void Awake() => SetUp();

    public void SetDefault()
    {
        transform.position = defaultPosition;
        transform.rotation = Quaternion.Euler(defaultRotation);
    }

    private void SetUp()
    {
        press.Enable();
        moveHorizontal.Enable();
        moveVertical.Enable();
        look.Enable();
        wheel.Enable();
        shift.Enable();

        press.performed += _ => { isRightClick = true; };
        press.canceled += _ => { isRightClick = false; };

        moveHorizontal.performed += context => { 
            moveDirection.x = context.ReadValue<Vector2>().x; 
            moveDirection.z = context.ReadValue<Vector2>().y; 
            };
        moveHorizontal.canceled += context => { moveDirection = Vector3.zero; };

        moveVertical.performed += context => { moveDirection.y = context.ReadValue<Vector2>().y; };
        moveVertical.canceled += context => { moveDirection = Vector2.zero; };

        look.performed += context => { lookDirection += isRightClick ? context.ReadValue<Vector2>() : Vector2.zero;};

        wheel.performed += context => { 
            var delta = Mathf.Clamp(context.ReadValue<float>(), -.01f, .01f); 
            moveSpeedMultiplyer = moveSpeedMultiplyer + delta > 0.01f && moveSpeedMultiplyer + delta < 1f ? moveSpeedMultiplyer + delta : moveSpeedMultiplyer;
            };

        shift.performed += _ => { moveSpeed *= 2; };
        shift.canceled += _ => { moveSpeed /= 2; };
    }

    private void Update()
    {
         if (!isRightClick)
            return;

        MoveTransfrom(moveDirection);
        LookTransfrom(lookDirection);

    }

    private void MoveTransfrom(Vector3 direction)
    {
        transform.position += (transform.right * direction.x + transform.up * direction.y + transform.forward * direction.z) * Time.deltaTime * CurrentMoveSpeed;
    }

    private void LookTransfrom(Vector2 direction)
    {
        transform.rotation = Quaternion.Euler(-direction.y * lookSpeed, direction.x * lookSpeed, 0);
    }
}
