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

        /*void FindFirstGenTiles()
        {
            foreach (var _tile in tiles)
                if (_tile.GetComponent<Tile>().openDoors > 1)
                    firstGenTiles.Add(_tile);
                else
                    tilesWithOneWay.Add(_tile);
        }*/

        public void GenerateTiles(int rowsSize, int collsSize)
        {
            _rowsSize = rowsSize;
            _collsSize = collsSize;
            GenerateTiles();
        }

        public void GenerateTiles()
        {
            //FindFirstGenTiles();
            /*var startTile = Instantiate(tilePrefab, grid);
            startTile.OpenRandomDoors();
            spawnTiles.Add(startTile.gameObject);
            AddNewSpawnPosition(startTile.gameObject);
            int generateTiles = 1;

            while (generateTiles < amountTiles || spawnPoints.Count > 0)
            {
                if (spawnPoints.Count == 0)
                    if (generateTiles < amountTiles)
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //change to regenerate
                    else
                        break;

                if ((spawnTiles.Count + spawnPoints.Count) >= amountTiles)
                    secondGenerate = true;

                var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
                var createTile = CreateTile(spawnPoint);
                spawnTiles.Add(createTile);
                spawnPoints.Remove(spawnPoint);
                AddNewSpawnPosition(createTile);
                generateTiles++;
            }*/

            grid.position = new Vector2(-(_collsSize / 2) + 0.5f, -(_rowsSize / 2) + 0.5f);

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

            GenerateMaze();
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

        private void GenerateMaze()
        {
            List<Vector2> directions = new List<Vector2>() {Vector2.left, Vector2.up, Vector2.right, Vector2.down};

            Vector2 startPosition = new Vector2(0, 0);
            Vector2 endPosition = new Vector2(_collsSize - 1, _rowsSize - 1);

            Tile currentTile = tilesGrid[0][0];
            Tile prevTile = null;

            List<Tile> visitedTiles = new List<Tile> ();

            while((Vector2)currentTile.transform.localPosition != endPosition)
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
                        if (currentTile.transform.localPosition.x < _collsSize - 1)
                            direction = Vector2.right;
                        else
                            direction = Vector2.left;
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
