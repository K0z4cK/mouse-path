using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Map {
    public class MapTiles : SingletonComponent<MapTiles>
    {
        [SerializeField] public UnityEvent OnMapUpdate;

        [SerializeField] private List<Tile> map = new List<Tile>();

        internal void SetMap(List<Tile> map)
        {
            this.map = map;
            OnMapUpdate.Invoke();
        }

        public List<Tile> GetMap() => map;


    }
}
