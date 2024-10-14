using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EventsManager : MonoBehaviour
{
    [SerializeField] private Image FlashBangImage;
    [SerializeField] private AudioSource flashbang;

    private bool isFading = false;

    private void Awake()
    {
        flashbang.clip = Resources.Load<AudioClip>("Flashbang");
        if (flashbang.clip == null)
        {
            Debug.LogError("Flashbang audio clip not found!");
        }

        FlashBangImage.gameObject.SetActive(false);
        FlashBangImage.color = new Color(FlashBangImage.color.r, FlashBangImage.color.g, FlashBangImage.color.b, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isFading)
        {
            StartCoroutine(FadeInAndOutFlashbang());
        }
    }

    private IEnumerator FadeInAndOutFlashbang()
    {
        isFading = true;
        FlashBangImage.gameObject.SetActive(true);

        // 사운드를 즉시 재생
        if (flashbang.clip != null)
        {
            flashbang.Play();
        }
        else
        {
            Debug.LogError("Flashbang audio clip is not set correctly.");
        }

        // Fade in the image over 1 second
        float fadeInDuration = 1.0f;
        float fadeOutDuration = 1.0f;
        float currentTime = 0.0f;

        while (currentTime < fadeInDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(currentTime / fadeInDuration);
            FlashBangImage.color = new Color(FlashBangImage.color.r, FlashBangImage.color.g, FlashBangImage.color.b, alpha);
            yield return null;
        }

        // 사운드 클립의 재생 길이를 기다림
        yield return new WaitForSeconds(flashbang.clip.length);

        // Fade out the image over 1 second
        currentTime = 0.0f;
        while (currentTime < fadeOutDuration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1 - currentTime / fadeOutDuration);
            FlashBangImage.color = new Color(FlashBangImage.color.r, FlashBangImage.color.g, FlashBangImage.color.b, alpha);
            yield return null;
        }

        FlashBangImage.gameObject.SetActive(false);
        isFading = false;
    }
}
