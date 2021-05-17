using System.Collections;
using UnityEngine;

/// <summary>
/// Medkit.
/// </summary>
public class Medkit : MonoBehaviour
{
    [SerializeField]
    private float Health;

    public string Name { get { return "Trousse de soins."; } }

    private AudioSource sfx;

    private void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    public void Heal()
    {
        if (GameManager.singleton.PlayerLogic.Health == GameManager.singleton.PlayerLogic.MaxHealth) return;

        GameManager.singleton.PlayerLogic.AddHealth(Health);

        StartCoroutine(PlaySound());
    }

    IEnumerator PlaySound()
    {
        if (sfx == null) Destroy(gameObject);

        sfx.Play();

        foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
        {
            Destroy(renderer);
        }

        Destroy(GetComponent<BoxCollider>());

        yield return new WaitForSeconds(sfx.clip.length);

        Destroy(gameObject);
    }
}
