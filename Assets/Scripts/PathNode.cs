using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Map
{
    namespace Path
    {
        public class PathNode
        {
            internal Tile tile;

            public int gCost;
            public int hCost;
            public int fCost;

            public bool isWalkable;
            public PathNode cameFromNode;
            public List<PathNode> neighbors;

            public PathNode(Tile tile)
            {
                this.tile = tile;
                isWalkable = true;
            }

            public void CalculateFCost()
            {
                fCost = gCost + hCost;
            }

            public void SetIsWalkable(bool isWalkable)
            {
                this.isWalkable = isWalkable;
            }

            public override string ToString()
            {
                return tile.name + ";" + tile.centerPoint.position;
            }

        }
    }
}