using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAnimation : MonoBehaviour
{
    [Header("Delay")]
    [SerializeField]
    private float startDelay = 3f;

    [Header("Title")]
    [SerializeField]
    private Animator Title;

    private float titleInterval = 1f;

    [Header("Buttons")]
    [SerializeField]
    private Animator[] Buttons;

    [SerializeField]
    private float buttonInterval = 0.5f;

    GraphicRaycaster gr;

    void Awake()
    {
        gr = GetComponent<GraphicRaycaster>();

        gr.enabled = false;

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        yield return new WaitForSeconds(startDelay);

        Title.SetTrigger("Start");

        yield return new WaitForSeconds(titleInterval);

        for(int i = 0; i < Buttons.Length; i += 2) // +2 Because texts are separated from the button sprite
        {
            Buttons[i].SetTrigger("Start");
            Buttons[i+1].SetTrigger("Start");

            yield return new WaitForSeconds(buttonInterval);
        }

        gr.enabled = true;;
    }
    
}
