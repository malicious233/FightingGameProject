using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainmenuManager : MonoBehaviour
{
    public void LoadDuelMode()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadPracticeMode()
    {
        SceneManager.LoadScene("TrainingScene");
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
