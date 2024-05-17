using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : SingletonComponent<PlayerDataManager>
{
    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    public void Init()
    {
        DontDestroyOnLoad(this);
        PlayFabDataManager.Instance.GetUserData();
    }

    public void SetPlayerData(PlayerData playerData) => _playerData = playerData;
}
