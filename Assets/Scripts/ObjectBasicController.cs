using System.Collections;
using UnityEngine;

public class ObjectBasicController : MonoBehaviour
{
    public Material InactiveMaterial;
    public Material GazedAtMaterial;

    // The objects are about 1 meter in radius, so the min/max target distance are
    // set so that the objects are always within the room (which is about 5 meters
    // across).
    private const float _minObjectDistance = 2.5f;
    private const float _maxObjectDistance = 3.5f;
    private const float _minObjectHeight = 0.5f;
    private const float _maxObjectHeight = 3.5f;

    private Renderer _myRenderer;
    private Vector3 _startingPosition;

    void Start()
    {
        _startingPosition = transform.parent.localPosition;
        _myRenderer = GetComponent<Renderer>();
        SetMaterial(false);
    }

    /// Sets this instance's material according to gazedAt status.
    /// Value `true` if this object is being gazed at, `false` otherwise.
    private void SetMaterial(bool gazedAt)
    {
        if (InactiveMaterial != null && GazedAtMaterial != null)
        {
            _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
        }
    }

    /// This method is called by the Main Camera when it starts gazing at this GameObject.
    public void OnPointerEnter()
    {
        SetMaterial(true);
    }

    /// This method is called by the Main Camera when it stops gazing at this GameObject.
    public void OnPointerExit()
    {
        SetMaterial(false);
    }

    public void OnPointerClick()
    {
        Debug.Log("Hello: " + _myRenderer.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
