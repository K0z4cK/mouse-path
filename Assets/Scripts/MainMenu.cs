using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Speed"))
            PlayerPrefs.SetFloat("Speed", 0.2f);
        inputField.text = PlayerPrefs.GetFloat("Speed").ToString();
    }

    public void StartGame() 
    {
        inputField.text = inputField.text.Replace('.', ',');
        float newSpeed = float.Parse(inputField.text);
        print(newSpeed);
        PlayerPrefs.SetFloat("Speed", newSpeed);
        SceneManager.LoadScene("Game"); 
    }

    public void Exit() => Application.Quit();
}
