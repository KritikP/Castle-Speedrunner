using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject gameoverMenu;
    [SerializeField] private GameObject deathText;
    [SerializeField] private GameObject victoryText;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject OnScreenControls;
    [SerializeField] private Player_Data playerData;
    private SceneLoader sceneLoader;
    private float canvasWidth;
    private float canvasHeight;

    private void Start()
    {
        canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        sceneLoader = FindObjectOfType<SceneLoader>();

        if (Application.isMobilePlatform)
        {
            OnScreenControls.SetActive(true);
        }
        else
        {
            OnScreenControls.SetActive(false);
        }
    }

    public void OpenGameOverMenu()
    {
        StopAllCoroutines();
        if (shop.activeInHierarchy)
        {
            gameoverMenu.SetActive(true);
            gameoverMenu.transform.localPosition = new Vector3(-canvasWidth, gameoverMenu.transform.localPosition.y, gameoverMenu.transform.localPosition.z);
            StartCoroutine(SlideUI(shop, new Vector3(canvasWidth * 2, 0, 0), false));
            StartCoroutine(SlideUI(gameoverMenu, Vector3.zero, true));
        }
        else
        {
            gameoverMenu.SetActive(true);
            shop.SetActive(false);
            gameoverMenu.transform.localPosition = new Vector3(gameoverMenu.transform.localPosition.x, -canvasHeight, gameoverMenu.transform.localPosition.z);
            StartCoroutine(SlideUI(gameoverMenu, Vector3.zero, true, 1f));
        }
        
    }

    private IEnumerator SlideUI(GameObject element, Vector3 targetPosition, bool active)
    {
        float smoothTime = 0.3f;
        Vector3 velocity = Vector3.zero;
        while (Mathf.Abs(targetPosition.magnitude - element.transform.localPosition.magnitude) > 1f)
        {
            element.transform.localPosition = Vector3.SmoothDamp(element.transform.localPosition, targetPosition, ref velocity, smoothTime);
            yield return new WaitForEndOfFrame();
        }
        element.SetActive(active);
    }

    private IEnumerator SlideUI(GameObject element, Vector3 targetPosition, bool active, float delayTime)
    {
        if(delayTime >= 0)
            yield return new WaitForSeconds(delayTime);
        float smoothTime = 0.3f;
        Vector3 velocity = Vector3.zero;
        while (Mathf.Abs(targetPosition.magnitude - element.transform.localPosition.magnitude) > 1f)
        {
            element.transform.localPosition = Vector3.SmoothDamp(element.transform.localPosition, targetPosition, ref velocity, smoothTime);
            yield return new WaitForEndOfFrame();
        }
        element.SetActive(active);
    }

    public void OpenShop()
    {
        StopAllCoroutines();
        shop.SetActive(true);
        shop.transform.localPosition = new Vector3(canvasWidth, shop.transform.localPosition.y, shop.transform.localPosition.z);
        StartCoroutine(SlideUI(gameoverMenu, new Vector3(-canvasWidth * 2, 0, 0), false));
        StartCoroutine(SlideUI(shop, Vector3.zero, true));
    }

    public void PlayerDeath()
    {
        OpenGameOverMenu();
    }

    public void Victory()
    {
        OpenGameOverMenu();
        deathText.SetActive(false);
        victoryText.SetActive(true);
    }

    public void TryAgain()
    {
        sceneLoader.LoadLevel();
    }

    public void ReturnToMenu()
    {
        sceneLoader.LoadMenu();
    }

    public void PauseGame()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        
    }
}
