using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFade : MonoBehaviour
{
    private Light objectLight;
    private float initialIntensity;
    private float fadeDuration = 1f;
    private bool flag = false;
    private float minIntensity = 0;

    void Start()
    {
        objectLight = GetComponent<Light>();
        initialIntensity = objectLight.intensity;
    }

    void Update()
    {
        float intensityDecreaseRate = initialIntensity / fadeDuration;

        float newIntensity = objectLight.intensity - (intensityDecreaseRate * Time.deltaTime);
        if (flag && newIntensity < minIntensity){
            newIntensity = minIntensity;
            objectLight.intensity = newIntensity;
            return;
        }

        newIntensity = Mathf.Clamp(newIntensity, 0f, initialIntensity);

        objectLight.intensity = newIntensity;

        // Check if the intensity has reached 0, and you can optionally destroy or disable the light at this point
        if (newIntensity <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetMinimumIntensity(float intensity)
    {
        flag = true;
        minIntensity = intensity;
    }
}
