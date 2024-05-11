
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    using Path;
    public class MapGenerator : MonoBehaviour
    {
        //[SerializeField] List<GameObject> tiles;
        [SerializeField] Tile tilePrefab;

        [SerializeField] Transform grid;
        [SerializeField] int _rowsSize = 10;
        [SerializeField] int _collsSize = 16;

        List<Tile> spawnTiles = new List<Tile>();
        Tile[][] tilesGrid;

        private void Start()
        {
            GenerateTiles();
        }

        public void GenerateTiles(int rowsSize, int collsSize)
        {
            _rowsSize = rowsSize;
            _collsSize = collsSize;
            GenerateTiles();
        }

        public void GenerateTiles()
        {
            grid.position = new Vector2(-(_collsSize / 2) , -(_rowsSize / 2) );

            tilesGrid = new Tile[_collsSize][];
            for(int x = 0; x < _collsSize; x++)
            {
                tilesGrid[x] = new Tile[_rowsSize];
                for (int y = 0; y < _rowsSize; y++)
                {
                    var newTile = Instantiate(tilePrefab, grid);
                    newTile.OpenRandomDoors();
                    //var neighbours = GetNeighbourList(newTile);
                    CloseEdgeDoors(newTile, x, y);
                    newTile.transform.localPosition = new Vector2(x, y);
                    tilesGrid[x][y] = newTile;
                    spawnTiles.Add(newTile);
                }
            }

            SpawnGameObjects();
        }

        private void CloseEdgeDoors(Tile tile, int xPos, int yPos)
        {
            List<DoorType> types = new List<DoorType>();
            if(xPos == 0)
                types.Add(DoorType.Left);
            if(yPos == 0)
                types.Add(DoorType.Down);
            if(xPos == _collsSize - 1)
                types.Add(DoorType.Right);
            if (yPos == _rowsSize - 1)
                types.Add(DoorType.Up);

            tile.CloseDoors(types);
        }

        private void SpawnGameObjects()
        {
            Transform mouse = MapManager.Instance.Mouse.transform;
            Transform cheese = MapManager.Instance.Cheese.transform;

            Vector2 randomPosition = tilesGrid[Random.Range(0, _collsSize)][Random.Range(0, _rowsSize)].transform.position;

            mouse.position = randomPosition;
            randomPosition = tilesGrid[Random.Range(0, _collsSize)][ Random.Range(0, _rowsSize)].transform.position;
            cheese.position = randomPosition;
            GenerateMaze(mouse.position, cheese.position);
        }

        private void GenerateMaze(Vector2 startPosition, Vector2 endPosition)
        {

            List<Vector2> directions = new List<Vector2>() {Vector2.left, Vector2.up, Vector2.right, Vector2.down};

            Tile endTile = spawnTiles.Find(x => (Vector2)x.centerPoint.position == endPosition);


            Tile currentTile = spawnTiles.Find(x => (Vector2)x.centerPoint.position == startPosition);
            Tile prevTile = null;

            List<Tile> visitedTiles = new List<Tile> ();

            while(currentTile.transform.localPosition != endTile.transform.localPosition)
            {
                List<Vector2> currentDirections = new List<Vector2>(directions);

                if (currentTile.transform.localPosition.x == 0)
                    currentDirections.Remove(Vector2.left);
                if (currentTile.transform.localPosition.y == 0)
                    currentDirections.Remove(Vector2.down);
                if (currentTile.transform.localPosition.x == _collsSize - 1)
                    currentDirections.Remove(Vector2.right);
                if (currentTile.transform.localPosition.y == _rowsSize - 1)
                    currentDirections.Remove(Vector2.up);

                if (prevTile != null)
                    currentDirections.Remove(currentTile.transform.localPosition - prevTile.transform.localPosition);

                bool isNewTile = false;
                Vector2 direction = new Vector2();
                int i = 0;
                while (!isNewTile)
                {
                    i++;
                    direction = currentDirections[Random.Range(0, currentDirections.Count)];
                    isNewTile = !visitedTiles.Contains(tilesGrid[(int)(currentTile.transform.localPosition.x + direction.x)][(int)(currentTile.transform.localPosition.y + direction.y)]);
                    if (i == 4)
                    {
                        break;
                    }
                }
                currentTile.OpenDoor(direction);
                prevTile = currentTile;
                currentTile = tilesGrid[(int)(prevTile.transform.localPosition.x + direction.x)][(int)(prevTile.transform.localPosition.y + direction.y)];
                currentTile.OpenDoor(direction * -1);
                visitedTiles.Add(currentTile);
            }
            CloseNeighboursDoors();
            if (CheckMapFull())
            {
                
                MapTiles.Instance.SetMap(spawnTiles);
            }
        }

        private void CloseNeighboursDoors()
        {
            foreach(Tile tile in spawnTiles)
            {
                var neighbours = GetNeighbourList(tile);
                foreach(var door in tile.doors)
                {
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.transform.position == door.point.position && !door.hasWay)
                        {
                            neighbour.CloseDoor(tile.transform.localPosition - neighbour.transform.localPosition);
                        }
                    }
                }
            }
        }

        private List<Tile> GetNeighbourList(Tile currentTile)
        {
            List<Tile> neighbourList = new List<Tile>();

            foreach (var door in currentTile.doors)
                foreach (var sptile in spawnTiles)
                    if (sptile.transform.position == door.point.position)
                        neighbourList.Add(sptile);

            return neighbourList;
        }
            
        private bool CheckMapFull()
        {
            int countFullOpen = 0;

            foreach(var tile in spawnTiles)
            {
                if (tile.openDoors == 4)
                    countFullOpen++;
            }

            if (countFullOpen > spawnTiles.Count / 4)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //change to regenerate
                return false;
            }
            return true;
        }
    }
}
