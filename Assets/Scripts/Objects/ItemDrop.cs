using System.Collections;
using UnityEngine;

/// <summary>
/// Pickable items dropped by an enemy or left in the level.
/// </summary>
/// <remarks>Unfortunately, you can't visually know which item is placed in the scene, you need
/// to click on the Game Object or play the game to find out</remarks>
public class ItemDrop : MonoBehaviour
{
    [SerializeField]
    private GameObject Item;

    [SerializeField]
    private Transform FloatPoint;

    private AudioSource SoundEffect;

    private bool isPicked = false;

    private void Awake()
    {
        Item = Instantiate(Item, FloatPoint);

        SoundEffect = GetComponent<AudioSource>();

        // Teleport the item to the ground if it's floating, really useful for items dropped by NPCs
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 100f, 11))
        {
            transform.position = hit.point;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        FloatPoint.Rotate(0f, 50f * Time.deltaTime, 0f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isPicked) return;

        if(other.CompareTag("Player"))
        {
            ////////////////////////////////////////////////////////////////////////////

            // Weapon
            WeaponLogic weaponLogic = Item.GetComponent<WeaponLogic>();

            if(weaponLogic != null)
            {
                weaponLogic.GiveToPlayer();

                StartCoroutine(PickItem(weaponLogic.Name));

                return;
            }

            ////////////////////////////////////////////////////////////////////////////

            // Medkit
            Medkit medkit = Item.GetComponent<Medkit>();

            if(medkit != null)
            {
                if (GameManager.singleton.PlayerLogic.Health >= GameManager.singleton.PlayerLogic.MaxHealth) return;

                medkit.Heal();

                StartCoroutine(PickItem(medkit.Name));

                return;
            }

            ////////////////////////////////////////////////////////////////////////////
        }
    }

    /// <summary>
    /// Coroutine to play the sound effect and make sure it finishes before the Game Object is destroyed.
    /// </summary>
    IEnumerator PickItem(string itemName)
    {
        isPicked = true;

        GameManager.singleton.GUI.sendMessage($"Ramassé {itemName}");

        if(SoundEffect != null)
        {
            SoundEffect.Play();

            yield return new WaitForSeconds(SoundEffect.clip.length);
        }

        Destroy(gameObject);
    }
}
