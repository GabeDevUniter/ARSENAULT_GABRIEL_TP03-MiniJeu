using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Exit the level
/// </summary>
public class ExitLevel : MonoBehaviour
{
    [SerializeField]
    private Fade fade;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.singleton.IsGameEnded) StartCoroutine(EndLevel());
    }

    IEnumerator EndLevel()
    {
        if(fade != null)
        {
            fade.FadeIn();

            yield return new WaitForSeconds(fade.FadeInDuration + 2f);
        }

        GameManager.singleton.ChangeLevel("Main");
    }
}
