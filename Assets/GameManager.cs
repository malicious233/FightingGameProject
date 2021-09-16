using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public void RestartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }


}
