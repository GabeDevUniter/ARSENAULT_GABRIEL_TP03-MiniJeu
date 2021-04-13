using UnityEngine;

/// <summary>
/// Patrol point for the NPC. He heads to this point and heads to the next one
/// </summary>
public class PatrolPoint : MonoBehaviour
{
    public float delay;
    public bool turnToPoint = false; // Si le NPC doit tourner à la même orientation du patrol point
    public PatrolPoint next;

    private void OnDrawGizmos()
    {
        if (next != null)
        {
            Vector3 half = Vector3.Lerp(transform.position, next.transform.position, 0.5f);

            Gizmos.color = Color.red; // Début de la route
            Gizmos.DrawLine(transform.position, half);

            Gizmos.color = Color.yellow; // Fin de la route
            Gizmos.DrawLine(half, next.transform.position);
        }
    }
}
