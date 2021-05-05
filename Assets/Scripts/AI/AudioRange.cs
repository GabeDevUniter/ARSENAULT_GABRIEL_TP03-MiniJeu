using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRange : MonoBehaviour
{
    [SerializeField]
    private float radius = 5f;


    private Grunt grunt;

    private Collider[] colliders;

    void Awake()
    {
        grunt = GetComponent<Grunt>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q)) Trigger();
    }

    public void Trigger()
    {
        colliders = Physics.OverlapSphere(transform.position, radius);

        foreach(Collider collider in colliders)
        {
            Grunt collided = collider.GetComponentInParent<Grunt>();
            
            if(collided != null && collided != grunt)
            {
                detected.Add(collided.EyeTransform.position);
                if(Physics.Linecast(grunt.EyeTransform.position, collided.EyeTransform.position, out RaycastHit hit, LayerMask.NameToLayer("NPC")))
                {
                    Debug.Log(hit.collider);
                }
                else
                {
                    collided._SetState(typeof(AlertState));
                }
            }
        }
    }

    List<Vector3> detected = new List<Vector3>();
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);

        #if false
        foreach(Vector3 detect in detected)
        {
            if (Physics.Linecast(grunt.EyeTransform.position, detect, LayerMask.NameToLayer("NPC")))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawLine(grunt.EyeTransform.position, detect);
        }
        #endif
    }
}
