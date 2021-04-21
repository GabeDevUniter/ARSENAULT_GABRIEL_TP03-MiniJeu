using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractTypes { Weapon, Door}

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    private Collider interactCollider;

    [SerializeField]
    private InteractTypes interactType;

    [SerializeField]
    private float distance = 50f;

    private Camera mainCam;

    private GameObject interactGameObject;

    void Awake()
    {
        mainCam = Camera.main;

        interactGameObject = interactCollider.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(interactGameObject.transform.position, GameManager.singleton.PlayerHead) <= distance)
        {

        }
    }
}
