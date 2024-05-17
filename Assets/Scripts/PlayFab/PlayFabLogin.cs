using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabLogin : MonoBehaviour
{
    private void Awake()
    {
        Login();
    }

    private void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSucces, OnError);
    }

    private void OnSucces(LoginResult result)
    {
        PlayerDataManager.Instance.Init();
        Debug.Log("Succesful login/account created");
        SceneManager.LoadScene("MainMenu");
    }

    private void OnError(PlayFabError error)
    {
        Debug.Log("Error while login/creating account");
        Debug.Log(error.ToString());
    }
}
