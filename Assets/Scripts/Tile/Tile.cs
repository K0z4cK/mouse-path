using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DoorType { None, Left, Right, Up, Down }
namespace Map
{
    [System.Serializable]
    class Door
    {
        public DoorType type;
        public bool hasWay;
        public Transform point;
        public GameObject sprite;

        public override string ToString()
        {
            return "Pos: " + point.position.ToString() + " Type: " + type.ToString() + " HasWay: " + hasWay.ToString(); ;
        }
        public static DoorType operator !(Door door)
        {
            switch (door.type)
            {
                case DoorType.Left:
                    return DoorType.Right;
                case DoorType.Right:
                    return DoorType.Left;
                case DoorType.Up:
                    return DoorType.Down;
                case DoorType.Down:
                    return DoorType.Up;
                default:
                    return DoorType.None;
            }
        }
    }

    public class Tile : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] internal TMP_Text fCostText;
        [SerializeField] internal TMP_Text gCostText;
        [SerializeField] internal TMP_Text hCostText;

        [SerializeField] internal List<Door> doors;

        int _openDoors = -1;

        Tile() {}

        internal int openDoors
        {
            get
            {
                if (_openDoors >= 0)
                    return _openDoors;

                int openDoors = 0;
                foreach (var item in doors)
                {
                    if (item.hasWay)
                        openDoors++;
                }
                _openDoors = openDoors;
                return _openDoors;
            }
            set => _openDoors = value;
        }

        public Transform centerPoint;

        public void OpenRandomDoors()
        {
            int countDoorsToOpen = Random.Range(2, doors.Count + 1);

            List<Door> doorsToOpen = new List<Door>(doors);
            doorsToOpen.Shuffle();

            for (int i = 0; i < countDoorsToOpen; i++)
            {
                doorsToOpen[i].sprite.SetActive(false);
                doorsToOpen[i].hasWay = true;
            }
        }
        public void OpenDoor(Vector2 direction)
        {
            foreach (var door in doors)
            {
                if ((Vector2)door.point.localPosition == direction)
                {
                    door.hasWay = true;
                    door.sprite.SetActive(false);
                }
            }
        }


        public void CloseDoors(List<DoorType> doorTypes)
        {
            foreach (var door in doors)
            {
                if (doorTypes.Contains(door.type))
                {
                    door.hasWay = false;
                    door.sprite.SetActive(true);
                }
            }
        }

        public void CloseDoor(Vector2 direction)
        {
            foreach (var door in doors)
            {
                if ((Vector2)door.point.localPosition == direction)
                {
                    door.hasWay = false;
                    door.sprite.SetActive(true);
                }
            }
        }

        public override string ToString()
        {
            return name + " " + transform.position.ToString();
        }

        internal bool HasDoor(DoorType doorT)
        {
            foreach (var item in doors)
                if (item.hasWay && item.type == doorT)
                    return true;

            return false;
        }

        public void ShowCosts(int fCost, int gCost, int hCost)
        {
            fCostText.text = fCost.ToString();
            gCostText.text = gCost.ToString();
            hCostText.text = hCost.ToString();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            TileCustomize.Instance.ShowPanel(this);
        }

        /*private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current. && MapManager.Instance.GameMode != GameType.Customization)
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider.gameObject == gameObject)
                    TileCustomize.Instance.ShowPanel(this);
            }
        }*/
    }
}