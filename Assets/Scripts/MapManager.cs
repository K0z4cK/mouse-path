using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Map
{
    //using Player;
    public class MapManager : SingletonComponent<MapManager>
    {
        List<Tile> map = new List<Tile>();
        [SerializeField] Mouse mouse;
        public Mouse Mouse => mouse;

        [SerializeField] Transform cheese;
        public Transform Cheese => cheese;

        [SerializeField] GameObject startPanel;
        [SerializeField] GameObject pausePanel;

        public GameType GameMode;

        //[SerializeField] Transform player;
        //[SerializeField] Vector3 playerInTile = Vector3.forward; //rand vector 
        List<Tile> playerVisitedTile = new List<Tile>();

        [SerializeField] public UnityEvent<Tile> OnPlayerChangeTile;
        public int visitedTileCount
        {
            get => playerVisitedTile.Count;
        }

        /*public Transform GetPlayerTransform()
        {
            return player;
        }*/

        public void UpdateTileList()
        {
            map = MapTiles.Instance.GetMap();
            OnPlayerChangeTile.AddListener(AddVisitedTile);
            mouse.GetComponent<SpriteRenderer>().enabled = true;
            cheese.GetComponent<SpriteRenderer>().enabled = true;
            ChangeGameMode(GameType.Start);
            //player = PlayerController.Instance.transform;
            //StartCoroutine(TargetPlayer());
        }

        public void StartGame()
        {
            Time.timeScale = 1.0f;
            startPanel.SetActive(false);
            mouse.Init();
            ChangeGameMode(GameType.Game);
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /*IEnumerator TargetPlayer()
        {
            while (true)
            {
                var tile = GetNearestTile(player.position);
                if (tile == null)
                {
                    yield return 0.1f;
                    continue;
                }

                if (tile.centerPoint.position != playerInTile)
                {
                    OnPlayerChangeTile.Invoke(tile);
                    playerInTile = tile.centerPoint.position;
                }
                yield return 0.1f;
            }
        }*/

        public void AddVisitedTile(Tile tile)
        {
            if (!playerVisitedTile.Contains(tile))
                playerVisitedTile.Add(tile);
        }

        public Tile GetNearestTile(Vector3 pos)
        {
            if (map == null)
            {
                print("nomap");
                return null;
            }
            Tile findTile = null;
            float minDist = -1;
            foreach (var tile in map)
            {
                var dist = Vector3.Distance(tile.centerPoint.position, pos);
                if (dist < minDist || minDist == -1)
                {
                    minDist = dist;
                    findTile = tile;
                }
            }
            return findTile;
        }

        public Tile GetMouseTile() => GetNearestTile(mouse.transform.position);
        public Tile GetCheeseTile() => GetNearestTile(cheese.position);

        public Tile GetLastTile() => map[map.Count - 1];

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //change to regenerate
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
            {
                pausePanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        public void ChangeGameMode(GameType type)
        {
            if (GameMode == GameType.Start)
                startPanel.SetActive(false);
            if (type == GameType.Start)
                startPanel.SetActive(true);

            GameMode = type;
        }

        /*public Tile GetPlayerNearestTile()
        {
           return GetNearestTile(player.position);
        }*/
    }
}