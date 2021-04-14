using System.Collections;
using UnityEngine;


/// <summary>
/// Automatically destroys a particle effect when finished
/// </summary>
public class FX_AutoDestruct : MonoBehaviour
{
    private float time;

    private ParticleSystem particle;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();

        time = particle.main.duration;

        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
