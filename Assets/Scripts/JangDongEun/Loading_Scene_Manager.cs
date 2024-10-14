using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading_Scene_Manager : MonoBehaviour
{
    [SerializeField] private Image BackGround = null;
    [SerializeField] private Image LoadingImage = null;
    [SerializeField] private Image BlackOutImage = null;

    [SerializeField] private Camera mainCam = null;
    [SerializeField] private Transform[] mainCampos = null; // 배열로 받기

    [SerializeField] private AudioSource bgmAudioSource = null;

    private bool IsGameStarted = false;
    private int currentCameraIndex = 0;
    private bool isCameraMoving = false;
    private bool isFadeIn = false;

    private void Start()
    {
        StartCoroutine(LoadingSceneCoroutine());
        BlackOutImage.enabled = false;
    }

    private void Update()
    {
        if (IsGameStarted && !isCameraMoving && currentCameraIndex < mainCampos.Length)
        {
            StartCoroutine(MoveCameraToNextPosition());
        }
    }

    private IEnumerator LoadingSceneCoroutine()
    {
        if (!IsGameStarted)
        {
            IsGameStarted = true;

            // BackGround 페이드 아웃
            StartCoroutine(FadeOutBackground(2.0f));

            // LoadingImage 깜빡이기
            StartCoroutine(BlinkLoadingImage());
        }
        yield return null;
    }

    private IEnumerator FadeOutBackground(float duration)
    {
        Color startColor = BackGround.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            BackGround.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        BackGround.color = endColor;
        BackGround.enabled = false;
    }

    private IEnumerator BlinkLoadingImage()
    {
        while (true)
        {
            LoadingImage.enabled = !LoadingImage.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveCameraToNextPosition()
    {
        isCameraMoving = true;
        yield return StartCoroutine(MoveCamera(mainCampos[currentCameraIndex].position, mainCampos[currentCameraIndex].rotation.eulerAngles, 2.0f));

        currentCameraIndex++;

        if (currentCameraIndex >= mainCampos.Length)
        {
            // 모든 카메라 이동이 끝나면 블랙아웃 페이드 인 및 BGM 페이드 아웃
            StartCoroutine(FadeInBlackOutImage(2.0f));

            //yield return new WaitForSeconds(2.0f);


            StartCoroutine(FadeOutBGM(2.0f));

           

            
        }

        isCameraMoving = false;

    }

    private IEnumerator MoveCamera(Vector3 targetPosition, Vector3 targetRotation, float duration)
    {
        Vector3 startPosition = mainCam.transform.position;
        Quaternion startRotation = mainCam.transform.rotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            mainCam.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCam.transform.rotation = Quaternion.Slerp(startRotation, targetQuaternion, t);
            yield return null;
        }

        mainCam.transform.position = targetPosition;
        mainCam.transform.rotation = targetQuaternion;
    }

    private IEnumerator FadeInBlackOutImage(float duration)
    {
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);
        float elapsedTime = 0f;

        BlackOutImage.enabled = true;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            BlackOutImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        BlackOutImage.color = endColor;
    }

    private IEnumerator FadeOutBGM(float duration)
    {
        float startVolume = bgmAudioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, Mathf.Clamp01(elapsedTime / duration));
            yield return null;
        }

        bgmAudioSource.volume = 0;
        bgmAudioSource.Stop();

        SceneManager.LoadScene("Scene_LoginGame");
    }
}
