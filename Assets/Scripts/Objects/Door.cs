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
            StartCoroutine(DoorMove(doorTransform.rotation.eulerAngles, GetOrientation(-1)));
    }

    public void Close()
    {
        if (!isMoving && currentState)
            StartCoroutine(DoorMove(doorTransform.rotation.eulerAngles, GetOrientation(1)));
    }

    public void Toggle()
    {
        if(!isMoving)
        {
            if (currentState) Close();
            else Open();
        }
    }

    private Vector3 GetOrientation(int direction)
    {
        Vector3 currentAngles = doorTransform.rotation.eulerAngles;

        currentAngles.y += orientation * direction;

        return currentAngles;
    }

    IEnumerator DoorMove(Vector3 start, Vector3 end)
    {
        isMoving = true;

        float duration = (end.y - start.y) / speed;

        float direction = duration / Mathf.Abs(duration);

        duration = Mathf.Abs(duration);

        float elapsed = 0f;

        while(elapsed <= duration)
        {
            doorTransform.Rotate(new Vector3(0f, direction * speed * Time.deltaTime, 0f));

            elapsed += Time.deltaTime;

            yield return null;
        }

        doorTransform.Rotate(new Vector3(0f, end.y - doorTransform.rotation.eulerAngles.y, 0f));

        currentState = !currentState;

        isMoving = false;
    }
}
