using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraPointerController : NetworkBehaviour
{
    private const float _maxDistance = 10;
    private GameObject _gazedAtObject = null;

    private bool hasPointerEntered = false;
    private float onPointerEnterCounter = 0f;
    public float onPointerClickTime = 3f;

    public GameObject Loader;
    private Slider SliderObject;

    public override void OnStartClient()
    {
        SliderObject = Loader.GetComponentInChildren<Slider>();
    }

    public void Update()
    {
        if (isLocalPlayer)
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

                    if( _gazedAtObject.tag != "Environment")
                    {
                        hasPointerEntered = true;
                    } else {
                        hasPointerEntered = false;
                    }
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

                Loader.SetActive(true);
                SliderObject.value = onPointerEnterCounter / onPointerClickTime;

                if (onPointerEnterCounter >= onPointerClickTime)
                {
                    _gazedAtObject?.SendMessage("OnPointerClick");

                    Loader.SetActive(false);
                    SliderObject.value = 0f;
                }
            } else {
                onPointerEnterCounter = 0;

                Loader.SetActive(false);
                SliderObject.value = 0f;
            }
        }
    }

    public float getOnPointerEnterCounter()
    {
        return onPointerEnterCounter;
    }

    public void handlePointerClick(Renderer target)
    {
        if (target.tag == "Teleportable") {
            Debug.Log("handlePointerClick: " + target.tag);
            transform.position = target.transform.position;
        }
    }
}
