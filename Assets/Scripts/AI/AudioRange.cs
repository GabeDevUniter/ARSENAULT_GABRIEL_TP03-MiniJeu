using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType { NPC, Weapon }


/// <summary>
/// Makes the NPCs react to sound from either a weapon or another NPC
/// </summary>
public class AudioRange : MonoBehaviour
{
    [SerializeField]
    private float range = 5f;

    [SerializeField]
    private AudioType audioType;

    private Grunt grunt;

    private Collider[] colliders;

    private Vector3 startPosition
    {
        get
        {
            switch (audioType)
            {
                case AudioType.NPC: return grunt.EyeTransform.position;

                default: return transform.position;
            }
        }
    }

    void Awake()
    {
        grunt = GetComponent<Grunt>();
    }

    public void Trigger()
    {
        colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {
            Grunt collided = collider.GetComponentInParent<Grunt>();

            if (collided != null)
            {
                foundGrunts.Add(collided.EyeTransform.position);

                if (collided != grunt && !Physics.Linecast(startPosition, collided.EyeTransform.position, gameObject.layer))
                {
                    collided._SetState(typeof(AlertState));
                }
            }
        }
    }

    List<Vector3> foundGrunts = new List<Vector3>();
    private void OnDrawGizmos()
    {
#if false
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
#endif

#if false
        foreach(Vector3 detect in foundGrunts)
        {
            if (Physics.Linecast(startPosition, detect, gameObject.layer))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawLine(startPosition, detect);
        }
#endif
    }
}
