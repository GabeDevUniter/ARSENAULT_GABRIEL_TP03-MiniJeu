using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [Header("Standard Settings")]
    [SerializeField]
    private bool startOn = true;

    [SerializeField]
    private bool startFadeOut = true;

    [SerializeField]
    private float startDelay = 1f;

    [Header("Fade Duration")]
    [SerializeField]
    private float fadeIn = 1f;

    public float FadeInDuration { get { return fadeIn; } }

    [SerializeField]
    private float fadeOut = 1f;

    public float FadeOutDuration { get { return fadeOut; } }

    private Animator Animator;

    void Awake()
    {
        Animator = GetComponent<Animator>();

        Animator.SetFloat("Start On", 60f * System.Convert.ToInt32(!startOn));

        Image fade = GetComponent<Image>();

        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 255f * System.Convert.ToInt32(startOn));

        if (startFadeOut) StartCoroutine(StartFade());
    }

    IEnumerator StartFade()
    {
        yield return new WaitForSeconds(startDelay);

        FadeOut();
    }

    public void FadeIn()
    {
        Animator.SetFloat("Speed", 1f / fadeIn);
        Animator.SetTrigger("Fade In");
    }

    public void FadeOut()
    {
        Animator.SetFloat("Speed", 1f / fadeOut);
        Animator.SetTrigger("Fade Out");
    }
}
