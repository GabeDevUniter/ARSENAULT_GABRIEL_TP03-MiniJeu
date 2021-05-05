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
                if(Physics.Linecast(transform.position, collided.transform.position, out RaycastHit hit))
                {
                    Debug.Log(hit.collider);
                }
            }
        }
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
