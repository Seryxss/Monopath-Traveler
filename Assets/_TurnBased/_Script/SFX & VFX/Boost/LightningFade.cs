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
    // private float endIntensity = 250;

    // Start is called before the first frame update
    void Start()
    {
        objectLight = GetComponent<Light>();
        initialIntensity = objectLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the rate of intensity decrease per frame
        float intensityDecreaseRate = initialIntensity / fadeDuration;

        // Calculate the new intensity for the light
        float newIntensity = objectLight.intensity - (intensityDecreaseRate * Time.deltaTime);
        if (flag && newIntensity < minIntensity){
            newIntensity = minIntensity;
            // this.transform.position += new Vector3(0f,0f,0.05f);
            objectLight.intensity = newIntensity;
            return;
        }

        // Clamp the intensity to ensure it doesn't go below 0
        newIntensity = Mathf.Clamp(newIntensity, 0f, initialIntensity);

        // Set the new intensity to the light
        objectLight.intensity = newIntensity;

        // Check if the intensity has reached 0, and you can optionally destroy or disable the light at this point
        if (newIntensity <= 0f)
        {
            // Destroy(this.gameObject);
        }
    }

    public void SetMinimumIntensity(float intensity)
    {
        flag = true;
        minIntensity = intensity;
    }
}
