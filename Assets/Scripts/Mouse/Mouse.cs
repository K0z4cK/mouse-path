using Map;
using Map.Path;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class Mouse : MonoBehaviour
{
    public PlayerData playerData;

    private Vector3 _startPosition;
    private Vector3 _poi;
    private List<Vector3> _path = new List<Vector3>();
    
    private float _mouseSpeed = 0.2f;
    private float _playerSpeed = 5f;

    public int PlayerSeconds { get; set; }
    public int MouseSeconds { get; set; }

    private Rigidbody2D _rigidbody2D;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Init()
    {
        playerData = PlayerDataManager.Instance.PlayerData;
        //PathFinding.Instance.GetVectorPath(MapManager.Instance.GetMouseTile(), MapManager.Instance.GetCheeseTile());
        //_mouseSpeed = PlayerPrefs.GetFloat("Speed");
        _poi = MapManager.Instance.GetCheeseTile().centerPoint.position;
        _startPosition = MapManager.Instance.GetMouseTile().centerPoint.position;
        PathFinding.Instance.SetPathStart(MapManager.Instance.GetMouseTile(), MapManager.Instance.GetCheeseTile());
        //SetSearchingPath();
        //transform.DOPath(path.ToArray(), path.Count * 0.5f);
        PlayerMove();
        //StartCoroutine(AiMoveCoroutine());
        /*path = PathFinding.Instance.GetVectorPath(MapManager.Instance.GetNearestTile(transform.position), MapManager.Instance.GetLastTile());
        if (path != null)
            transform.DOPath(path.ToArray(), 10f);
        else
            print("path null");*/
    }

    void PlayerMove()
    {
        MapManager.Instance.PlayStartAnimation();
        StartCoroutine(PlayerMoveCoroutine());
    }

    IEnumerator PlayerMoveCoroutine()
    {
        yield return new WaitForSeconds(3f);
        MapManager.Instance.StartTimer();
        while (!IsPositionNear())
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(x * _playerSpeed, y * _playerSpeed, 0f);
            _rigidbody2D.velocity = move;
            yield return null;
        }
        _rigidbody2D.velocity = Vector2.zero;
        PlayerSeconds = MapManager.Instance.StopTimer();
        playerData.playthrougsCount++;
        playerData.totalSeconds += PlayerSeconds;
        playerData.averageSeconds = playerData.totalSeconds / playerData.playthrougsCount;

        yield return new WaitForSeconds(1f);
        PlayAiMove();
    }
    
    private bool IsPositionNear()
    {
        //print(_rigidbody2D.velocity.magnitude);
        if (Vector3.Distance(transform.position, _poi) < 0.2f)
            return true; else return false;
    }

    void PlayAiMove()
    {
        int difficulty = PlayerPrefs.GetInt("Difficulty");
        transform.position = _startPosition;
        
        if (difficulty == 1)
            StartCoroutine(AiMoveCoroutine());
        else
            Invoke(nameof(PlayAPath), 3f);

        MapManager.Instance.PlayStartAnimation();
    }

    void PlayAPath()
    {
        MapManager.Instance.StartTimer();
        var path = PathFinding.Instance.GetVectorPath(MapManager.Instance.GetMouseTile(), MapManager.Instance.GetCheeseTile());
        if (path != null)
            transform.DOPath(path.ToArray(), path.Count * 0.2f).SetEase(Ease.Linear);
        else
            print("path null");

        StartCoroutine(APathCor());
    }

    IEnumerator APathCor()
    {
        yield return new WaitUntil(() => transform.position == _poi);
        MouseSeconds = MapManager.Instance.StopTimer();
        if (PlayerSeconds >= MouseSeconds)
        {
            playerData.hardWinCount++;
            playerData.totalWinCount++;
        }
        else
        {
            playerData.hardLoseCount++;
            playerData.totalLoseCount++;
        }

        PlayerDataManager.Instance.SetPlayerData(playerData);
        PlayFabDataManager.Instance.SetUserData();
        yield return new WaitForSeconds(1f);
        MapManager.Instance.OpenEndPanel();
    }

    IEnumerator AiMoveCoroutine()
    {
        yield return new WaitForSeconds(3f);       
        MapManager.Instance.StartTimer();
        while (transform.position != _poi)
        {
            Vector3 nextPos = PathFinding.Instance.GetNextNode(MapManager.Instance.GetNearestTile(transform.position).centerPoint.position).tile.centerPoint.position;
            transform.DOMove(nextPos, _mouseSpeed).SetEase(Ease.Linear);
            yield return new WaitForSeconds(_mouseSpeed); 
        }
        MouseSeconds = MapManager.Instance.StopTimer();
        if (PlayerSeconds >= MouseSeconds)
        {
            playerData.normalWinCount++;
            playerData.totalWinCount++;
        }
        else
        {
            playerData.normalLoseCount++;
            playerData.totalLoseCount++;
        }

        PlayerDataManager.Instance.SetPlayerData(playerData);
        PlayFabDataManager.Instance.SetUserData();
        print(PlayerDataManager.Instance.PlayerData.totalSeconds);
        yield return new WaitForSeconds(1f);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        MapManager.Instance.OpenEndPanel();
    }
}

public struct PlayerData 
{
    public int averageSeconds;
    public int totalSeconds;
    public int playthrougsCount;
    public int normalWinCount;
    public int normalLoseCount;
    public int hardWinCount;
    public int hardLoseCount;
    public int totalWinCount;
    public int totalLoseCount;
    
}
