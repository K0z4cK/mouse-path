using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        [SerializeField] Animator startText;
        [SerializeField] TMP_Text timer;
        private int seconds = 0;
        Coroutine timerCor;

        [SerializeField] GameObject endPanel;
        [SerializeField] TMP_Text playerTime;
        [SerializeField] TMP_Text mouseTime;
        public GameType GameMode;

        List<Tile> playerVisitedTile = new List<Tile>();

        [SerializeField] public UnityEvent<Tile> OnPlayerChangeTile;
        public int visitedTileCount
        {
            get => playerVisitedTile.Count;
        }

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

        public void StartTimer()
        {
            timer.enabled = true;
            seconds = 0;
            timer.text = "0:00";
            timerCor = StartCoroutine(TimerCoroutine());
        }

        private IEnumerator TimerCoroutine()
        {
            while (true) { 
                yield return new WaitForSeconds(1f);
                seconds++;
                if (seconds < 60)
                    if(seconds < 10)
                        timer.text = "0:" + "0"+seconds;
                    else
                        timer.text = "0:" + seconds;
                else
                {
                    int minutes = seconds / 60;
                    int secondsLeft = seconds - minutes * 60;
                    if (secondsLeft < 10)
                        timer.text = minutes + ":" + "0" + secondsLeft;
                    else
                        timer.text = minutes + ":" + secondsLeft;
                }
            }
        }

        public int StopTimer()
        {
            StopCoroutine(timerCor);
            return seconds;
        }

        public void PlayStartAnimation() => startText.SetTrigger("Play");
        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        public void MeinMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void OpenEndPanel()
        {
            timer.enabled = false;
            endPanel.SetActive(true);
            int playerSeconds = mouse.PlayerSeconds;
            int mouseSeconds = mouse.MouseSeconds;

            playerTime.text = "Player Time:\n";
            mouseTime.text = "Mouse AI Time:\n";

            if (playerSeconds < 60)
                if (playerSeconds < 10)
                    playerTime.text += "0:" + "0" + playerSeconds;
                else
                    playerTime.text += "0:" + playerSeconds;
            else
            {
                int minutes = playerSeconds / 60;
                int secondsLeft = playerSeconds - minutes * 60;
                if (secondsLeft < 10)
                    playerTime.text += minutes + ":" + "0" + secondsLeft;
                else
                    playerTime.text += minutes + ":" + secondsLeft;
            }

            if (mouseSeconds < 60)
                if (mouseSeconds < 10)
                    mouseTime.text += "0:" + "0" + mouseSeconds;
                else
                    mouseTime.text += "0:" + mouseSeconds;
            else
            {
                int minutes = mouseSeconds / 60;
                int secondsLeft = mouseSeconds - minutes * 60;
                if (secondsLeft < 10)
                    mouseTime.text += minutes + ":" + "0" + secondsLeft;
                else
                    mouseTime.text += minutes + ":" + secondsLeft;
            }

        }

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