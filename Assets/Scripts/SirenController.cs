using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        Range[0] = minRange;
        Range[1] = maxRange;

        Intensity[0] = minIntensity;
        Intensity[1] = maxIntensity;
    }

    void Update()
    {
        if (isOn && !isBlinking) StartCoroutine(Blink());
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

    private void alterLight(Light light, Transform[] points)
    {
        int index = System.Convert.ToInt16(currentSide);

        light.transform.position = points[index].position;

        light.range = Range[index];
        
        light.intensity = Intensity[index];
    }
}
