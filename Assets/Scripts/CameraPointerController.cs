using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraPointerController : NetworkBehaviour
{
    private const float _maxDistance = float.PositiveInfinity;
    private GameObject _gazedAtObject = null;

    private bool hasPointerEntered = false;
    private float onPointerEnterCounter = 0f;
    public float onPointerClickTime = 3f;

    private UILogger loggerScript;
    private CameraPointerController cameraPointerController;
    private Camera camera;
    private Slider SliderObject;
    private string[] interactiveTags = { "Interactive", "Teleportable", "Player" };
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

                if (_gazedAtObject.tag != "Environment")
                {
                    hasPointerEntered = true;
                }
                else
                {
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
        }
        else
        {
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
        if (target.tag == "Teleportable")
        {
            Debug.Log("handlePointerClick: " + target.tag);
            transform.position = GetTargetEdgePosition(target);
        }
    }

    Vector3 GetTargetEdgePosition(GameObject target)
    {
        if (!TryGetTargetBounds(target, out Bounds bounds))
            return target.transform.position;

        Vector3 center = bounds.center;
        Vector3 direction = transform.position - center;
        if (direction.sqrMagnitude < 0.0001f)
            direction = -transform.forward;
        else
            direction.Normalize();

        const float margin = 0.05f;
        return GetBoundsSurfacePoint(bounds, direction) + direction * margin;
    }

    static bool TryGetTargetBounds(GameObject target, out Bounds bounds)
    {
        bounds = default;
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return true;
        }

        Collider[] colliders = target.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
            return false;

        bounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
            bounds.Encapsulate(colliders[i].bounds);
        return true;
    }

    static Vector3 GetBoundsSurfacePoint(Bounds bounds, Vector3 direction)
    {
        Vector3 extents = bounds.extents;
        float distance = float.PositiveInfinity;

        for (int i = 0; i < 3; i++)
        {
            float component = Mathf.Abs(direction[i]);
            if (component > 0.0001f)
                distance = Mathf.Min(distance, extents[i] / component);
        }

        return bounds.center + direction * distance;
    }

    public void sendMessage(GameObject target, string message)
    {
        if (target && System.Array.IndexOf(interactiveTags, target.tag) >= 0)
        {
            target.SendMessage(message);
        }
    }
}
