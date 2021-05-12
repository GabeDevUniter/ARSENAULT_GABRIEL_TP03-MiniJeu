using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Triggerable
{
    [Header("Settings")]
    [SerializeField]
    private Transform doorTransform;

    [SerializeField]
    private float orientation = 90f;

    [SerializeField]
    private float speed = 25f;

    [SerializeField]
    private bool lockOnMove = true; // Will lock when the door either opens or closes

    public bool locked = false;

    private bool isMoving = false;

    private bool currentState = false; // false = Closed ; true = Open

    private Vector3 originAngles;

    private OcclusionPortal portal;

    private void Awake()
    {
        originAngles = doorTransform.rotation.eulerAngles;

        portal = GetComponent<OcclusionPortal>();
    }

    public void Open()
    {
        if (locked) return;

        if (!isMoving && !currentState)
            StartCoroutine(DoorMove(doorTransform.rotation.eulerAngles, GetOrientation(1)));
    }

    public void Close()
    {
        if (locked) return;

        if (!isMoving && currentState)
            StartCoroutine(DoorMove(doorTransform.rotation.eulerAngles, GetOrientation(1)));
    }

    public void Toggle()
    {
        if (locked) return;

        if(!isMoving)
        {
            if (currentState) Close();
            else Open();
        }
    }

    private Vector3 GetOrientation(int direction)
    {
        Vector3 currentAngles = doorTransform.rotation.eulerAngles;

        currentAngles.y -= currentState ? currentAngles.y - originAngles.y : orientation * direction;

        return currentAngles;
    }

    IEnumerator DoorMove(Vector3 start, Vector3 end)
    {
        isMoving = true;

        if (portal != null && !currentState) portal.open = !currentState; // Open portal if initially closed

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

        if (portal != null && currentState) portal.open = !currentState; // Close portal if finally closed

        currentState = !currentState;

        locked = lockOnMove;

        isMoving = false;
    }
}
