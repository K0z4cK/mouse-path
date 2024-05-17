using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject difficultyPanel;
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Speed"))
            PlayerPrefs.SetFloat("Speed", 0.2f);
        //inputField.text = PlayerPrefs.GetFloat("Speed").ToString();
    }

    public void OpenDifficultyPanel()
    {
        difficultyPanel.SetActive(true);
    }

    public void StartGame(int difficulty) 
    {
        /*inputField.text = inputField.text.Replace('.', ',');
        float newSpeed = float.Parse(inputField.text);
        print(newSpeed);
        PlayerPrefs.SetFloat("Speed", newSpeed);*/
        PlayerPrefs.SetInt("Difficulty", difficulty);
        SceneManager.LoadScene("Game"); 
    }

    public void Exit() => Application.Quit();
}
