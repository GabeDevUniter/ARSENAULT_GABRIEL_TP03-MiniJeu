using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Transform doorTransform;

    [SerializeField]
    private float orientation = 90f;

    [SerializeField]
    private float speed = 25f;

    private bool isMoving = false;

    private bool currentState = false; // false = Closed ; true = Open

    public void Open()
    {
        if (!isMoving && !currentState)
            StartCoroutine(DoorMove(doorTransform.rotation.y, doorTransform.rotation.y - orientation));
    }

    public void Close()
    {
        if (!isMoving && currentState)
            StartCoroutine(DoorMove(doorTransform.rotation.y, doorTransform.rotation.y + orientation));
    }

    public void Toggle()
    {
        if(!isMoving)
        {
            if (currentState) Close();
            else Open();
        }
    }

    IEnumerator DoorMove(float start, float end)
    {
        isMoving = true;

        while(doorTransform.rotation.y <= start + (end - start))
        {
            float elapsed = 0f;

            while (elapsed <= 1f)
            {
                doorTransform.Rotate(Vector3.up, speed * Time.deltaTime);

                elapsed += Time.deltaTime;

                yield return null;
            }
        }

        doorTransform.rotation = new Quaternion(doorTransform.rotation.x, end, doorTransform.rotation.z, doorTransform.rotation.w);

        isMoving = false;
    }
}
