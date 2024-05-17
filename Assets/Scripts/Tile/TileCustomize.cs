using Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileCustomize : SingletonComponent<TileCustomize>
{
    [SerializeField] GameObject customizePanel;

    [Header("Buttons")]
    [SerializeField] Button leftButton;
    [SerializeField] Button topButton;
    [SerializeField] Button rightButton;
    [SerializeField] Button botButton;

    [Header("Walls")]
    [SerializeField] Image leftWall;
    [SerializeField] Image topWall;
    [SerializeField] Image rightWall;
    [SerializeField] Image botWall;

    private Tile _tile;

    private GameType _prevGameType;

    private void Awake()
    {
        leftButton.onClick.AddListener(delegate { ChangeDoorWay(DoorType.Left, leftButton); });
        topButton.onClick.AddListener(delegate { ChangeDoorWay(DoorType.Up, topButton); });
        rightButton.onClick.AddListener(delegate { ChangeDoorWay(DoorType.Right, rightButton); });
        botButton.onClick.AddListener(delegate { ChangeDoorWay(DoorType.Down, botButton); });

    }

    public void ShowPanel(Tile tile)
    {
        _prevGameType = MapManager.Instance.GameMode;
        MapManager.Instance.ChangeGameMode(GameType.Customization);
        Time.timeScale = 0f;

        _tile = tile;

        foreach(var door in _tile.doors)
        {
            if(door.type == DoorType.Left)
            {
                if (door.hasWay)
                {
                    leftButton.GetComponentInChildren<TMP_Text>().text = "Close";
                    leftWall.enabled = false;
                }
                else
                {
                    leftButton.GetComponentInChildren<TMP_Text>().text = "Open";
                    leftWall.enabled = true;
                }
            }
            if (door.type == DoorType.Up)
            {
                if (door.hasWay)
                {
                    topButton.GetComponentInChildren<TMP_Text>().text = "Close";
                    topWall.enabled = false;
                }
                else
                {
                    topButton.GetComponentInChildren<TMP_Text>().text = "Open";
                    topWall.enabled = true;
                }
            }
            if (door.type == DoorType.Right)
            {
                if (door.hasWay)
                {
                    rightButton.GetComponentInChildren<TMP_Text>().text = "Close";
                    rightWall.enabled = false;
                }
                else
                {
                    rightButton.GetComponentInChildren<TMP_Text>().text = "Open";
                    rightWall.enabled = true;
                }
            }
            if (door.type == DoorType.Down)
            {
                if (door.hasWay)
                {
                    botButton.GetComponentInChildren<TMP_Text>().text = "Close";
                    botWall.enabled = false;
                }
                else
                {
                    botButton.GetComponentInChildren<TMP_Text>().text = "Open";
                    botWall.enabled = true;
                }
            }
        }
        customizePanel.SetActive(true);

    }

    public void ChangeDoorWay(DoorType type, Button btn )
    {
        var door = _tile.doors.Find(x => x.type == type);
        bool isOpen = !door.hasWay;

        door.hasWay = isOpen;
        if (isOpen)
            door.sprite.SetActive(false);
        else
            door.sprite.SetActive(true);

        var neighbour = MapManager.Instance.GetNearestTile(door.point.position);
        DoorType neighbourType = DoorType.None;
        switch (type)
        {
            case DoorType.Left:
                neighbourType = DoorType.Right;
                if (isOpen)
                    leftWall.enabled = false;
                else
                    leftWall.enabled = true;
                break;

            case DoorType.Up:
                neighbourType = DoorType.Down;
                if (isOpen)
                    topWall.enabled = false;
                else
                    topWall.enabled = true; break;

            case DoorType.Right:
                neighbourType = DoorType.Left;
                if (isOpen)
                    rightWall.enabled = false;
                else
                    rightWall.enabled = true;
                break;

            case DoorType.Down:
                neighbourType = DoorType.Up;
                if (isOpen)
                    botWall.enabled = false;
                else
                    botWall.enabled = true;
                break;
            default:
                break;
        }

        var neighbourDoor = neighbour.doors.Find(x => x.type == neighbourType);
        neighbourDoor.hasWay = isOpen;
        if (isOpen)
            neighbourDoor.sprite.SetActive(false);
        else
            neighbourDoor.sprite.SetActive(true);
        if(isOpen)
            btn.GetComponentInChildren<TMP_Text>().text = "Close";
        else
            btn.GetComponentInChildren<TMP_Text>().text = "Open";
    }

    public void ClosePanel()
    {
        customizePanel.SetActive(false);

        MapManager.Instance.ChangeGameMode(_prevGameType);
        if (_prevGameType == GameType.Game)
            Time.timeScale = 1.0f;
    }
    
}
