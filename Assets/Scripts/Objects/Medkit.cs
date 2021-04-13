using UnityEngine;

/// <summary>
/// Medkit.
/// </summary>
public class Medkit : MonoBehaviour
{
    [SerializeField]
    private float Health;

    public string Name { get { return "Trousse de soins."; } }

    public void Heal()
    {
        GameManager.singleton.PlayerLogic.AddHealth(Health);
    }
}
