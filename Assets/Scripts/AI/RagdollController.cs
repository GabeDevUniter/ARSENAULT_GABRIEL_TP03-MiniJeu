using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField]
    private float cooldownTime = 5f;

    private Rigidbody[] ragdoll;

    public Rigidbody[] Ragdoll { get { return ragdoll; } }

    private Collider[] colliders;


    void Awake()
    {
        ragdoll = GetComponentsInChildren<Rigidbody>();

        colliders = GetComponentsInChildren<Collider>();
    }

    public void SetRagdoll(bool mode)
    {
        foreach (Rigidbody bone in ragdoll)
        {
            if(bone != null) bone.isKinematic = !mode;
        }
    }

    public void SetRagdoll(bool mode, bool collision)
    {
        SetRagdoll(mode);

        foreach(Collider collider in colliders)
        {
            if(collider != null) collider.enabled = collision;
        }
    }

    public void startCooldown()
    {
        if (isCooling) return;

        StartCoroutine(cooldown());
    }

    bool isCooling = false;
    IEnumerator cooldown()
    {
        isCooling = true;

        yield return new WaitForSeconds(cooldownTime);

        SetRagdoll(false, false);

        isCooling = false;
    }
}
