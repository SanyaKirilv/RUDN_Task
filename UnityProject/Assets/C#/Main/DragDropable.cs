using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragDropable : MovableObject
{
    [Header("InputActions")]
    [SerializeField] private InputAction press;
    [SerializeField] private InputAction screenPos;
    [Header("Other")]
    [SerializeField] private Vector3 curScreenPos;
    [SerializeField] private bool isDragging;
    [SerializeField] private Transform place;

    private Transform Cam => Camera.main.transform;

    private Vector3 WorldPos
    {
        get
        {
            float z = Camera.main.WorldToScreenPoint(transform.position).z;
            return Camera.main.ScreenToWorldPoint(curScreenPos + new Vector3(0, 0, z));
        }
    }
    private bool OnObject
    {
        get
        {
            Ray ray = Cam.GetComponent<Camera>().ScreenPointToRay(curScreenPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform == transform;
            }
            return false;
        }
    }

    private void Awake() => SetUp();

    public void SetDefault()
    {
        transform.position = defaultPosition;
        transform.rotation = Quaternion.Euler(defaultRotation);
    }

    public void Connect()
    {
        transform.SetParent(place);
        transform.localPosition = new Vector3(0, 0, 0);
        transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    private void SetUp()
    {
        press.Enable();
        screenPos.Enable();

        press.performed += _ =>
        {
            if (OnObject)
            {
                StartCoroutine(Drag());
                transform.SetParent(null);
            }
        };
        press.canceled += _ => { isDragging = false; };

        screenPos.performed += context => { curScreenPos = context.ReadValue<Vector2>(); };
    }

    private IEnumerator Drag()
    {
        isDragging = true;
        Vector3 offset = transform.position - WorldPos;
        while (isDragging)
        {
            transform.position = WorldPos + offset;
            yield return null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "#" + this.name && !isDragging)
        {
            if (transform.parent == null)
            {
                Connect();
            }
        }
    }
}
