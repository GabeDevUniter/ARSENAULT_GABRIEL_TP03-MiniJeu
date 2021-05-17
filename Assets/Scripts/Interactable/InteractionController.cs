using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InteractTypes { Weapon, Medkit, Door }

/// <summary>
/// Main script for interactable items. This includes opening a door or picking
/// up an item on the ground by looking at it and pressing E
/// </summary>
public class InteractionController : MonoBehaviour
{
    #region Fields and properties
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

    private Trigger[] triggers; // Used to trigger the triggers reacting to interactions

    #endregion

    void Awake()
    {
        interactGameObject = interactCollider.gameObject;

        mainCam = Camera.main;

        triggers = GetComponents<Trigger>();

        
    }

    void Update()
    {
        if (GameManager.singleton == null) return;
        
        if (isInteractable && Vector3.Distance(interactGameObject.transform.position, GameManager.singleton.PlayerHead) <= distance)
        {
            // Code to add UI text to the interaction's origin


            if(Input.GetKeyDown(KeyCode.E))
            {
                // Fetch the first interactable item on the center of the screen
                Ray mouseRay = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

                if(Physics.Raycast(mouseRay, out RaycastHit hit) && hit.collider == interactCollider)
                {
                    // Provide special code for each
                    switch (interactType)
                    {
                        case InteractTypes.Door:

                            GetComponent<Door>().Toggle();

                            break;

                        case InteractTypes.Weapon:

                            GetComponent<WeaponLogic>().GiveToPlayer();

                            Destroy(gameObject);

                            return;

                        case InteractTypes.Medkit:

                            GetComponent<Medkit>().Heal();

                            return;
                    }

                    // Trigger every local trigger that reacts to interaction
                    foreach (Trigger trigger in triggers) trigger.setTrigger(TriggerCondition.Interaction);

                    StartCoroutine(Cooldown());
                }
            }
        }

        
    }

    /// <summary>
    /// Wait a certain amount of time before being interacted again
    /// </summary>
    /// <returns></returns>
    IEnumerator Cooldown()
    {
        isInteractable = false;

        yield return new WaitForSeconds(delay);

        isInteractable = true;
    }
}
