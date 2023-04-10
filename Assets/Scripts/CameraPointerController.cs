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

    private UILogger loggerScript;
    private CameraPointerController cameraPointerController;
    private Camera camera;
    private Slider SliderObject;
    private string[] interactiveTags = {"Interactive", "Teleportable", "Player"};
    public GameObject Loader;

    public override void OnStartClient()
    {
        SliderObject = Loader.GetComponentInChildren<Slider>();
        transform.position = new Vector3(Random.Range(0f, 3f), 1, Random.Range(0f, 3f));

        cameraPointerController = this.GetComponentInChildren<CameraPointerController>();
        loggerScript = this.GetComponentInChildren<UILogger>();

        if (isLocalPlayer)
        {
            Camera camera = cameraPointerController.GetComponent<Camera>();
            Canvas canvas = this.GetComponentInChildren<Canvas>();
            camera.enabled = true;
            canvas.enabled = true;
            return;
        }

    }

    public void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance))
        {            
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                loggerScript.SetMessage("");
                sendMessage(_gazedAtObject, "OnPointerExit");

                _gazedAtObject = hit.transform.gameObject;
                sendMessage(_gazedAtObject, "OnPointerEnter");
                loggerScript.SetMessage("OnPointerEnter:" + _gazedAtObject.name);

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
            sendMessage(_gazedAtObject, "OnPointerExit");
            loggerScript.SetMessage("OnPointerExit");
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
                loggerScript.SetMessage("OnPointerClick");
                cameraPointerController.handlePointerClick(_gazedAtObject);

                Loader.SetActive(false);
                SliderObject.value = 0f;
            }
        } else {
            onPointerEnterCounter = 0;

            Loader.SetActive(false);
            SliderObject.value = 0f;
        }
    }

    public float getOnPointerEnterCounter()
    {
        return onPointerEnterCounter;
    }

    public void handlePointerClick(GameObject target)
    {
        if (target.tag == "Teleportable") {
            Debug.Log("handlePointerClick: " + target.tag);
            transform.position = target.transform.position;
        }
    }

    public void sendMessage(GameObject target, string message)
    {
        if (target && System.Array.IndexOf(interactiveTags, target.tag) >= 0)
        {
            target.SendMessage(message);
        }
    }
}
