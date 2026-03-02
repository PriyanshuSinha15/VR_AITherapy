using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotatePlayer : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private bool inverted;
    private bool canRotate;
    private Vector2 rotation;

    [SerializeField] private InputAction pressed, axis;

    private void Awake()
    {
        pressed.Enable();
        axis.Enable();

        pressed.performed += _ => { StartCoroutine(Rotate()); };
        pressed.canceled += _ => { canRotate = false; };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };
    }

    private IEnumerator Rotate()
    {
        canRotate = true;
        while (canRotate)
        {
            rotation *= speed;
            transform.Rotate(Vector3.up *(inverted? 1 : -1) * rotation.x, Space.World);
            yield return null;
        }
    }

    //private void OnMouseDrag()
    //{
    //    float rotX = Input.GetAxis("Mouse X") * speed * Mathf.Deg2Rad;

    //    transform.Rotate(Vector3.up, -rotX);

    //    Debug.Log("Rotate the player");
    //}




    public void ResetRotation()
    {
        Vector3 originalRot = new Vector3(0, 0, 0);

        transform.eulerAngles = originalRot;
    }
}
