using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main class for any explosion in the game.
/// </summary>
public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float Damage = 35f;

    [SerializeField]
    private float Radius = 3f;

    [SerializeField]
    private float Force = 1f;

    [SerializeField]
    private GameObject ExplosionEffect;

    [SerializeField]
    private bool hasEffects = true;

    [SerializeField]
    private bool isDamageable = false;

    [SerializeField]
    private float health = 10f;

    private bool isExploding = false;

    public void RemoveHealth(float value)
    {
        if (!isDamageable || isExploding) return;

        health -= value;

        if (health <= 0f) Explode();
    }

    public void Explode()
    {
        isExploding = true;

        if (hasEffects)
        {
            if(ExplosionEffect != null) Instantiate(ExplosionEffect, transform.position, new Quaternion(0, 0, 0, 0));
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, Radius);

        List<int> knownGrunts = new List<int>();

        foreach(Collider collider in colliders)
        {
            // Lerp to measure the damage inflicted to the object.
            float lerp = Vector3.Distance(transform.position, collider.transform.position) / Radius;

            ////////////////////////////////////////////////////////////////////////////

            // Grunt Detection
            Grunt grunt = collider.GetComponentInParent<Grunt>();

            if (grunt != null)
            {
                bool isDuplicate = false;

                foreach (int _grunt in knownGrunts)
                {
                    if (grunt.GetInstanceID() == _grunt)
                    {
                        isDuplicate = true;
                        break;
                    }
                }

                if (!isDuplicate)
                {
                    grunt.RemoveHealth(Mathf.Lerp(1f, Damage, lerp));

                    knownGrunts.Add(grunt.GetInstanceID());
                }

                if(grunt.IsDead)
                {
                    foreach (Rigidbody bone in grunt.Ragdoll)
                    {
                        bone.AddExplosionForce(Force, transform.position, Radius, 1.5f, ForceMode.Impulse);
                    }
                }
                


                continue;
            }

            ////////////////////////////////////////////////////////////////////////////

            // Explosion detection
            Explosion explosion = collider.GetComponent<Explosion>();

            if(explosion != null)
            {
                explosion.RemoveHealth(Mathf.Lerp(1f, Damage, lerp));

                continue;
            }

            ////////////////////////////////////////////////////////////////////////////

            // Player detection
            Player player = collider.GetComponent<Player>();

            if(player != null)
            {
                player.RemoveHealth(Mathf.Lerp(1f, Damage, lerp));

                continue;
            }

            ////////////////////////////////////////////////////////////////////////////
        }

        Destroy(gameObject);
    }



#if true
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
#endif
}
