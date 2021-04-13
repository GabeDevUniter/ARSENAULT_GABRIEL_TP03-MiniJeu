using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GUI message that displays which item has been picked up
/// by the player (e.g.: "Ramassé M4A1")
/// </summary>
public class Message : MonoBehaviour
{
    [SerializeField]
    private float holdTime = 3f;

    [SerializeField]
    private float fadeTime = 1.5f;

    private Text Text;

    private bool isDisplayed = false;

    private void Awake()
    {
        Text = GetComponent<Text>();
    }

    public void Display(string text)
    {
        if (isDisplayed) return;

        isDisplayed = true;

        Text.text = text;

        StartCoroutine(HoldAndFade());
    }

    private IEnumerator HoldAndFade()
    {
        yield return new WaitForSeconds(holdTime);

        Color oldColor = Text.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);

        float elapsed = 0f;

        while(elapsed < fadeTime)
        {
            Text.color = Color.Lerp(oldColor, newColor, elapsed / fadeTime);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
