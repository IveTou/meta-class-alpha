using UnityEngine;

[ExecuteAlways]
public class SunLightController : MonoBehaviour
{
    [SerializeField] Transform sunTransform;
    [SerializeField] Transform lightTarget;
    [SerializeField] Light sunLight;
    [SerializeField] string[] sunObjectNames = { "Sun", "SUN", "Sphere.009" };

    void Reset()
    {
        sunLight = GetComponent<Light>();
        ConfigureSunLight();
    }

    void Awake()
    {
        if (sunLight == null)
            sunLight = GetComponent<Light>();

        ConfigureSunLight();
    }

    void LateUpdate()
    {
        if (sunLight == null)
            sunLight = GetComponent<Light>();

        Transform sun = ResolveSunTransform();
        if (sun == null || sunLight == null)
            return;

        transform.position = sun.position;

        Vector3 targetPoint = ResolveTargetPoint(sun);
        Vector3 direction = targetPoint - sun.position;
        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void ConfigureSunLight()
    {
        if (sunLight == null)
            return;

        sunLight.type = LightType.Directional;
        sunLight.color = new Color(1f, 0.96f, 0.84f);
        sunLight.intensity = 1.2f;
        sunLight.useColorTemperature = true;
        sunLight.colorTemperature = 5778f;
    }

    Transform ResolveSunTransform()
    {
        if (sunTransform != null)
            return sunTransform;

        foreach (string objectName in sunObjectNames)
        {
            GameObject found = GameObject.Find(objectName);
            if (found != null)
                return found.transform;
        }

        GameObject solarSystem = GameObject.Find("SolarSystem");
        if (solarSystem == null)
            return null;

        foreach (Transform child in solarSystem.GetComponentsInChildren<Transform>(true))
        {
            foreach (string objectName in sunObjectNames)
            {
                if (child.name.Equals(objectName, System.StringComparison.OrdinalIgnoreCase))
                    return child;
            }
        }

        return solarSystem.transform;
    }

    Vector3 ResolveTargetPoint(Transform sun)
    {
        if (lightTarget != null)
            return lightTarget.position;

        GameObject solarSystem = GameObject.Find("SolarSystem");
        if (solarSystem != null)
        {
            Transform mercury = solarSystem.transform.Find("MERCURY");
            if (mercury != null)
                return mercury.position;

            Transform earth = solarSystem.transform.Find("EARTH");
            if (earth != null)
                return earth.position;
        }

        return sun.position + Vector3.forward * 10f;
    }
}
