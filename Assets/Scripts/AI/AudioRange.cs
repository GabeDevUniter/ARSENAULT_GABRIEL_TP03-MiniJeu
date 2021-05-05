using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType { NPC, Weapon }

public class AudioRange : MonoBehaviour
{
    [SerializeField]
    private float range = 5f;

    [SerializeField]
    private AudioType audioType;

    private Grunt grunt;

    private Collider[] colliders;

    private Vector3 startPosition;

    void Awake()
    {
        switch(audioType)
        {
            case AudioType.NPC: grunt = GetComponent<Grunt>(); startPosition = grunt.EyeTransform.position; break;

            default: startPosition = transform.position; break;
        }
    }

    public void Trigger()
    {
        colliders = Physics.OverlapSphere(transform.position, range);

        foreach(Collider collider in colliders)
        {
            Grunt collided = collider.GetComponentInParent<Grunt>();
            
            if(collided != null)
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);

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
