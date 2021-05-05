using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InteractTypes { Weapon, Door }

public class InteractionController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private Collider interactCollider;

    public bool ColliderEnabled { get { return interactCollider.enabled; } set { interactCollider.enabled = value; } }

    [SerializeField]
    private InteractTypes interactType;

    public bool isInteractable = true;

    [SerializeField]
    private float distance = 2f;

    [SerializeField]
    private float delay = 2f;

    private GameObject interactGameObject;

    private Camera mainCam;

    void Awake()
    {
        interactGameObject = interactCollider.gameObject;

        mainCam = Camera.main;
    }


    void Update()
    {
        if(isInteractable && Vector3.Distance(interactGameObject.transform.position, GameManager.singleton.PlayerHead) <= distance)
        {
            // Code to add UI text to the interaction's origin


            if(Input.GetKeyDown(KeyCode.E))
            {
                Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(mouseRay, out RaycastHit hit) && hit.collider == interactCollider)
                {
                    switch (interactType)
                    {
                        case InteractTypes.Door:

                            GetComponent<Door>().Toggle();

                            break;

                        case InteractTypes.Weapon:

                            GetComponent<WeaponLogic>().GiveToPlayer();

                            Destroy(gameObject);

                            return;
                    }

                    StartCoroutine(Cooldown());
                }
            }
        }

        
    }

    IEnumerator Cooldown()
    {
        isInteractable = false;

        yield return new WaitForSeconds(delay);

        isInteractable = true;
    }
}
