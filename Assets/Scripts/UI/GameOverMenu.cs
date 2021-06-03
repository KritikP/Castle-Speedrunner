using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject shop;

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenShop()
    {
        shop.SetActive(true);
        gameObject.SetActive(false);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(1);
    }

}
