using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Map
{
    //using Player;
    public class MapManager : SingletonComponent<MapManager>
    {
        List<Tile> map = new List<Tile>();
        [SerializeField] Transform player;
        [SerializeField] Vector3 playerInTile = Vector3.forward; //rand vector 
        List<Tile> playerVisitedTile = new List<Tile>();

        [SerializeField] public UnityEvent<Tile> OnPlayerChangeTile;
        public int visitedTileCount
        {
            get => playerVisitedTile.Count;
        }

        public Transform GetPlayerTransform()
        {
            return player;
        }

        public void UpdateTileList()
        {
            map = MapTiles.Instance.GetMap();
            OnPlayerChangeTile.AddListener(AddVisitedTile);
            //player = PlayerController.Instance.transform;
            StartCoroutine(TargetPlayer());
        }

        IEnumerator TargetPlayer()
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

        public Tile GetPlayerNearestTile()
        {
           return GetNearestTile(player.position);
        }
    }
}