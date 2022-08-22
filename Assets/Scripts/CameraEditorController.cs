using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEditorController : MonoBehaviour
{
    public float sensitivity = 2f;
    private float mouseX = 0.0f, mouseY = 0.0f;
    void Start()
    {
        if (!Application.isEditor)
        {
            return;
        }

        print("We are running this from inside of the editor!");

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * sensitivity;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity;
        transform.eulerAngles = new Vector3(mouseY, mouseX, 0);
    }
}
