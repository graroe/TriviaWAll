using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    [SerializeField] float secondsToFadeIn;
    [SerializeField] float secondsToFadeOut;

    Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
    }
    public void fadeIn()
    {
        StartCoroutine(FadeImage(false));
    }

    public void fadeout()
    {
        StartCoroutine(FadeImage(true));
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime / secondsToFadeOut)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime/secondsToFadeIn)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }
}

