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
        List<Vector3> spawnPoints = new List<Vector3>();

        bool secondGenerate = false;

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

            List<Tile> _spawnTiles = new List<Tile>();
            foreach (var item in spawnTiles)
            {
                _spawnTiles.Add(item.GetComponent<Tile>());
            }

            
            GenerateMaze();
            MapTiles.Instance.SetMap(_spawnTiles);
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

                Vector2 direction = currentDirections[Random.Range(0, currentDirections.Count)];

                //DoorType type;

                currentTile.OpenDoor(direction);
                prevTile = currentTile;
                currentTile = tilesGrid[(int)(prevTile.transform.localPosition.x + direction.x)][(int)(prevTile.transform.localPosition.y + direction.y)];
                currentTile.OpenDoor(direction * -1);
            }

            CheckMapFull();
        }



        private List<Tile> GetNeighbourList(Tile currentTile)
        {
            List<Tile> neighbourList = new List<Tile>();

            foreach (var door in currentTile.doors)
                foreach (var sptile in spawnTiles)
                    if (sptile.transform.localPosition == door.point.position)
                        neighbourList.Add(sptile);

            return neighbourList;
        }
            
        private void CheckMapFull()
        {
            int countFullOpen = 0;

            foreach(var tile in spawnTiles)
            {
                if (tile.openDoors == 4)
                    countFullOpen++;
            }

            if(countFullOpen > spawnTiles.Count/3)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //change to regenerate

        }
    }
}
