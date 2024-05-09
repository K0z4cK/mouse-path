using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Map
{
    public enum DoorType { None, Left, Right, Up, Down }
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

    public class Tile : MonoBehaviour
    {
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
            int countDoorsToOpen = Random.Range(0, doors.Count+1);

            List<Door> doorsToOpen = new List<Door>(doors);
            doorsToOpen.Shuffle();

            for (int i = 0; i < countDoorsToOpen; i++)
            {
                doorsToOpen[i].sprite.SetActive(false);
                doorsToOpen[i].hasWay = true;
            }
        }

        public void OpenDoors(List<DoorType> doorTypes)
        {
            foreach (var door in doors)
            {
                if (doorTypes.Contains(door.type))
                {
                    door.hasWay = true;
                    door.sprite.SetActive(false);
                }
            }
        }

        public void OpenDoor(DoorType doorType)
        {
            foreach (var door in doors)
            {
                if (door.type == doorType)
                {
                    door.hasWay = true;
                    door.sprite.SetActive(false);
                }
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
    }
}