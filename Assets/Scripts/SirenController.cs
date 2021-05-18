using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlling the police sirens
/// </summary>
public class SirenController : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField]
    private Light BlueLight;

    [SerializeField]
    private Light RedLight;

    [Header("Points")]
    [SerializeField]
    private Transform[] BluePoints;

    [SerializeField]
    private Transform[] RedPoints;

    [Header("Settings")]
    [SerializeField]
    private bool isOn = true;

    [SerializeField]
    private bool alternate = true;

    [SerializeField]
    private float delay = 0.5f;

    [SerializeField]
    private float minRange = 5f;

    [SerializeField]
    private float minIntensity = 1f;

    [SerializeField]
    private float maxRange = 10f;

    [SerializeField]
    private float maxIntensity = 7f;

    private float[] Range = new float[2];

    private float[] Intensity = new float[2];

    private bool currentSide = false;

    private bool currentAlternate = true; // currentAlternate = red ; !currentAlternate = blue

    private void Awake()
    {
        Range[0] = minRange;
        Range[1] = maxRange;

        Intensity[0] = minIntensity;
        Intensity[1] = maxIntensity;
    }

    void Update()
    {
        if (isOn && alternate && !isAlternating) StartCoroutine(Alternate());
        else if (isOn && !isBlinking && !alternate) StartCoroutine(Blink());
    }


    private bool isBlinking = false;

    IEnumerator Blink()
    {
        isBlinking = true;

        yield return new WaitForSeconds(delay);

        alterLight(RedLight, RedPoints);

        alterLight(BlueLight, BluePoints);

        currentSide = !currentSide;

        isBlinking = false;
    }

    private bool isAlternating = false;
    IEnumerator Alternate()
    {
        isAlternating = true;

        SetLight(RedLight, RedPoints, 1);
        SetLight(BlueLight, BluePoints, 1);

        RedLight.enabled = currentAlternate;
        BlueLight.enabled = !currentAlternate;

        currentAlternate = !currentAlternate;

        yield return new WaitForSeconds(delay);

        isAlternating = false;
    }

    /// <summary>
    /// Change the position of the lights for better visuals
    /// </summary>
    /// <param name="light"></param>
    /// <param name="points"></param>
    private void alterLight(Light light, Transform[] points)
    {
        SetLight(light, points, System.Convert.ToInt16(currentSide));
    }

    private void SetLight(Light light, Transform[] points, int index)
    {
        light.transform.position = points[index].position;
        light.transform.rotation = points[index].rotation;

        light.range = Range[index];

        light.intensity = Intensity[index];
    }
}
