using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private GameObject desktopControls;
    [SerializeField] private GameObject mobileControls;
    [SerializeField] private PlayerInput input;

    public void Start()
    {
        input.DeactivateInput();
        if (Application.isMobilePlatform)
        {
            mobileControls.SetActive(true);
            desktopControls.SetActive(false);
        }
        else
        {
            mobileControls.SetActive(false);
            desktopControls.SetActive(true);
        }
    }

    public void StartGame()
    {
        sceneLoader.LoadLevel();
    }

    public void QuitGame()
    {
        Debug.Log("QUIT GAME");
        Application.Quit();
    }
}
