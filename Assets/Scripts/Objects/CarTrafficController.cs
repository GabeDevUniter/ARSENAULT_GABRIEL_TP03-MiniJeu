using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTrafficController : MonoBehaviour
{
    [SerializeField]
    private bool isOn = true;

    [SerializeField]
    private float minInterval = 4f;

    [SerializeField]
    private float maxInterval = 7f;

    [SerializeField]
    private float speed = 0.6f;

    private Animator[] animators;

    private bool inProgress = false;

    void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (Animator car in animators) car.speed = speed;

        SetVisibleAll(false);
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
            Animator pickedCar = pickCar();

            yield return new WaitForSeconds(1f / speed);

            pickedCar.gameObject.SetActive(false);

            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
        }

        foreach (Animator car in animators) car.gameObject.SetActive(false);

        inProgress = false;
    }

    private List<Animator> carPicker = new List<Animator>();
    Animator pickCar()
    {
        carPicker.Clear();

        foreach (Animator car in animators)
            if (!car.gameObject.activeInHierarchy) carPicker.Add(car);

        if (carPicker.Count == 0) return default;

        Animator pickedCar = carPicker[Random.Range(0, animators.Length)];

        pickedCar.gameObject.SetActive(true);

        return pickedCar;
    }

    void SetVisibleAll(bool isVisible)
    {
        foreach (Animator car in animators) car.gameObject.SetActive(isVisible);
    }
}
