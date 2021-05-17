using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrafficController : MonoBehaviour
{
    [SerializeField]
    private float minInterval = 4f;

    [SerializeField]
    private float maxInterval = 7f;

    [SerializeField]
    private bool isOn = true;

    private Animator[] animators;

    private bool inProgress = false;

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    private void LateUpdate()
    {
        if (!inProgress && isOn) StartCoroutine(StartTraffic());
    }

    IEnumerator StartTraffic()
    {
        inProgress = true;

        while(isOn)
        {
            pickCar().SetTrigger("Pass");

            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }

        inProgress = false;
    }

    Animator pickCar()
    {
        Animator pickedCar = animators[Random.Range(0, animators.Length)];

        foreach(Animator car in animators)
        {
            car.gameObject.SetActive(car == pickedCar);
        }

        return pickedCar;
    }
}
