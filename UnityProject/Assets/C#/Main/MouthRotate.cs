using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouthRotate : MovableObject
{
    [Header("InputActions")]
    [SerializeField] private InputAction press;
    [SerializeField] private InputAction axis;
    [SerializeField] private InputAction screenPos;
    [Header("Vectors")]
    [SerializeField] private Vector3 rotation;
    [SerializeField] private Vector3 curScreenPos;
    [Header("Speed")]
    [Range(0.01f, 5f)]
    [SerializeField] private float speed = 1;
    [Header("Other")]
    [SerializeField] private bool inverted;
    [SerializeField] private bool onObject;
    [SerializeField] private bool available;

    private Transform Cam => Camera.main.transform;

    private void Awake() => SetUp();

    public void Reset()
    {
        transform.rotation = Quaternion.Euler(defaultRotation);
    }

    private void OnMouseEnter() {
        onObject = true;
    }

    private void OnMouseExit() {
        onObject = false;
    }

    private void SetUp()
    {
        press.Enable();
        axis.Enable();

        press.performed += _ => { if(onObject) StartCoroutine(Rotate()); };
        press.canceled += _ => { available = false; };

        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };

        screenPos.performed += context => { curScreenPos = context.ReadValue<Vector2>(); };
    }

    private IEnumerator Rotate()
    {
        available = true;
        while (available)
        {
            rotation *= speed;
            transform.Rotate(Vector3.up * (inverted ? 1 : -1), rotation.x, Space.World);
            transform.Rotate(Cam.right * (inverted ? -1 : 1), rotation.y, Space.World);
            yield return null;
        }
    }
}
