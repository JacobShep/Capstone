using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayGame : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnDisable()
    {
        Debug.Log(inputText.text.ToString());
        PlayerPrefs.SetString("username", inputText.text);
        PlayerPrefs.Save();
    }
}
