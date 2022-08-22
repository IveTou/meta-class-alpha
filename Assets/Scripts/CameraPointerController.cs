using System.Collections;
using UnityEngine;

public class CameraPointerController : MonoBehaviour
{
    private const float _maxDistance = 10;
    private GameObject _gazedAtObject = null;

    private bool hasPointerEntered = false;
    private float onPointerEnterCounter = 0f;
    public float onPointerClickTime = 3f;

    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
        {
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                _gazedAtObject?.SendMessage("OnPointerExit");
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnter");

                hasPointerEntered = true;
            }
        }
        else
        {
            // No GameObject detected in front of the camera.
            _gazedAtObject?.SendMessage("OnPointerExit");
            _gazedAtObject = null;

            hasPointerEntered = false;
        }

        if (hasPointerEntered)
        {
            onPointerEnterCounter += Time.deltaTime;

            if (onPointerEnterCounter >= onPointerClickTime)
            {
                _gazedAtObject?.SendMessage("OnPointerClick");
            }
        } else {
            onPointerEnterCounter = 0;
        }
    }

    public float getOnPointerEnterCounter()
    {
        return onPointerEnterCounter;
    }
}
