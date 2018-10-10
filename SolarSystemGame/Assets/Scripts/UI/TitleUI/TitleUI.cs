using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{
	public void OnStartClicked()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}
