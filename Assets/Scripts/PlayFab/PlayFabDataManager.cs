using Map;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFabDataManager : SingletonComponent<PlayFabDataManager>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void SetUserData()
    {
        PlayerData playerData = PlayerDataManager.Instance.PlayerData;
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            
            Data = new Dictionary<string, string>() {
                    {"PlayerData", JsonUtility.ToJson(playerData)}
            }
        },
            result => Debug.Log("Successfully updated user data"),
            error =>
            {
                Debug.Log("Got error setting user data");
                Debug.Log(error.GenerateErrorReport());
            }); 
    }

    public void GetUserData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnUserDataRecieved, (error) =>
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }
    private void OnUserDataRecieved(GetUserDataResult result)
    {
        if (result.Data != null&& result.Data.ContainsKey("PlayerData"))
        {
            PlayerDataManager.Instance.SetPlayerData(JsonUtility.FromJson<PlayerData>(result.Data["PlayerData"].Value));
        }
        
    }

    private void OnApplicationQuit()
    {
        SetUserData();
    }
}


