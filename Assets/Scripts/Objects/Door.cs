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
            StartCoroutine(DoorMove2(doorTransform.rotation, new Quaternion(doorTransform.rotation.x, doorTransform.rotation.y - orientation * Mathf.Deg2Rad, 0f, 0f)));
        //StartCoroutine(DoorMove(doorTransform.rotation.eulerAngles.y, doorTransform.rotation.eulerAngles.y - orientation));
    }

    public void Close()
    {
        if (!isMoving && currentState)
            StartCoroutine(DoorMove2(doorTransform.rotation, new Quaternion(doorTransform.rotation.x, doorTransform.rotation.y + orientation * Mathf.Deg2Rad, 0f, 0f)));
        //StartCoroutine(DoorMove(doorTransform.rotation.eulerAngles.y, doorTransform.rotation.eulerAngles.y + orientation));
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

        float direction = (end - start) / Mathf.Abs(end - start);

        Debug.Log(doorTransform.rotation.y);
        Debug.Log(end * Mathf.Deg2Rad);
        Debug.Log(doorTransform.rotation.y + end * Mathf.Deg2Rad);

        Debug.Log(doorTransform.rotation.eulerAngles.y);
        Debug.Log(start + (end - start));
        while(doorTransform.rotation.eulerAngles.y >= start && doorTransform.rotation.eulerAngles.y <= end)
        {
            float elapsed = 0f;

            while (elapsed <= 1f)
            {
                doorTransform.Rotate(new Vector3(0f, speed * Time.deltaTime * direction, 0f));

                elapsed += Time.deltaTime;

                Debug.Log("In elapsed");

                yield return null;
            }

            Debug.Log("In progress");
        }

        Quaternion endResult = doorTransform.rotation;
        endResult.eulerAngles = new Vector3(0f, end, 0f);

        //doorTransform.Rotate(Vector3.up, end - doorTransform.rotation.y);
        
        currentState = !currentState;

        isMoving = false;
    }

    IEnumerator DoorMove2(Quaternion start, Quaternion end)
    {
        isMoving = true;

        Debug.Log(start);
        Debug.Log(end);

        float duration = (end.y - start.y) / speed;

        float elapsed = 0f;

        while(elapsed <= duration)
        {
            doorTransform.rotation = Quaternion.Lerp(start, end, elapsed / duration);

            yield return null;
        }

        doorTransform.rotation = end;

        currentState = !currentState;

        isMoving = false;
    }
}
