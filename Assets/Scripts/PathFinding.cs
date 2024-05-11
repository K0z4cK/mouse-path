using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Map
{
    namespace Path
    {
        public class PathFinding : SingletonComponent<PathFinding>
        {
            private List<PathNode> visitedList = new List<PathNode>();

            private List<PathNode> openList = new List<PathNode>();
            private List<PathNode> closedList = new List<PathNode>();
            List<PathNode> map = new List<PathNode>();
            bool haveMap = false;

            private PathNode _endNode;

            public void UpdateRoomList()
            {
                foreach (var item in MapTiles.Instance.GetMap())
                    map.Add(new PathNode(item));
                haveMap = true;
            }

            public List<Vector3> GetVectorPath(Tile startNode , Tile endNode)
            {
                PathNode roomS = null;
                PathNode roomE = null;

                if (startNode == null || endNode == null)
                    return null;

                foreach (var item in map)
                {
                    if (item.tile.centerPoint.position == endNode.centerPoint.position) 
                        roomE = item;
                    if (item.tile.centerPoint.position == startNode.centerPoint.position)
                        roomS = item;
                }

                if (roomS == null || roomE == null)
                    return null;
                
                var path = FindPath(roomS, roomE);
                if (path == null)
                    return null;
                else
                {
                    List<Vector3> vectorPath = new List<Vector3>();
                    foreach (PathNode pathNode in path)
                    {
                        vectorPath.Add(pathNode.tile.centerPoint.position);
                    }
                    return vectorPath;
                }
            }

            public void SetPathStart(Tile startTile, Tile endTile)
            {
                PathNode startNode = null;
                PathNode endNode = null;

                if (startTile == null || endTile == null)
                    return;

                foreach (var item in map)
                {
                    if (item.tile.centerPoint.position == endTile.centerPoint.position)
                        endNode = item;
                    if (item.tile.centerPoint.position == startTile.centerPoint.position)
                        startNode = item;
                }

                if (startNode == null || endNode == null)
                    return;

                if (startNode == null || endNode == null || !haveMap)
                {
                    return;
                }

                foreach (var room in map)
                {
                    room.gCost = int.MaxValue;
                    room.CalculateFCost();
                    room.cameFromNode = null;
                }

                _endNode = endNode;
                
                startNode.gCost = 0;
                startNode.hCost = CalculateDistanceCost(startNode, endNode);
                startNode.CalculateFCost();
                visitedList.Add(startNode);
            }

            public PathNode GetNextNode(Vector3 position)
            {
                PathNode currentNode = map.Find(x => x.tile.centerPoint.position == position);
                visitedList.Add(currentNode);

                var neighbours = GetNeighbourList(currentNode);

                PathNode bestNode = null;

                List<PathNode> visitedNeighbours = new List<PathNode>();

                foreach (PathNode neighbourNode in neighbours)
                {
                    if (visitedList.Contains(neighbourNode))
                    {
                        visitedNeighbours.Add(neighbourNode);
                        continue;
                    }
                    if (!neighbourNode.isWalkable)
                    {
                        visitedList.Add(neighbourNode);
                        continue;
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                    if (tentativeGCost <= neighbourNode.gCost)
                    {                    
                        neighbourNode.cameFromNode = currentNode;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = CalculateDistanceCost(neighbourNode, _endNode); neighbourNode.CalculateFCost();

                        if(bestNode == null)
                            bestNode = neighbourNode;

                        /*if (bestNode.gCost < neighbourNode.gCost)
                            bestNode = neighbourNode;*/

                        if (bestNode.fCost > neighbourNode.fCost)
                            bestNode = neighbourNode;
                    }
                }

                if (bestNode != null)
                    return bestNode;

                /*foreach (PathNode neighbourNode in neighbours)
                {
                    if (!neighbourNode.isWalkable)
                    {
                        visitedList.Add(neighbourNode);
                        continue;
                    }

                    if (bestNode == null)
                        bestNode = currentNode.cameFromNode;

                    //print("second foreach: " + tentativeGCost + " to " + neighbourNode.gCost);
                    if (bestNode.hCost > neighbourNode.hCost)
                    {
                        bestNode = neighbourNode;
                    }
                }
                if (bestNode != null)
                    return bestNode;*/

                return currentNode.cameFromNode;
            }


            private List<PathNode> FindPath(PathNode startNode, PathNode endNode)
            {
                if (startNode == null || endNode == null || !haveMap)
                {
                    return null;
                }

                openList = new List<PathNode> { startNode };
                closedList = new List<PathNode>();

                foreach (var room in map)
                {
                    room.gCost = int.MaxValue;
                    room.CalculateFCost();
                    room.cameFromNode = null;
                }

                startNode.gCost = 0;
                startNode.hCost = CalculateDistanceCost(startNode, endNode);
                startNode.CalculateFCost();

                startNode.tile.ShowCosts(startNode.fCost, startNode.gCost, startNode.hCost);

                while (openList.Count > 0)
                {
                    PathNode currentNode = GetLowestFCostNode(openList);
                    if (currentNode == endNode)
                    {
                        // Reached final node
                        return CalculatePath(endNode);

                    }

                    openList.Remove(currentNode);
                    closedList.Add(currentNode);

                    foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
                    {
                        if (closedList.Contains(neighbourNode)) continue;
                        if (!neighbourNode.isWalkable)
                        {
                            closedList.Add(neighbourNode);
                            continue;
                        }

                        int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                        if (tentativeGCost < neighbourNode.gCost)
                        {
                            neighbourNode.cameFromNode = currentNode;
                            neighbourNode.gCost = tentativeGCost;
                            neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode); neighbourNode.CalculateFCost();

                            neighbourNode.tile.ShowCosts(neighbourNode.fCost, neighbourNode.gCost, neighbourNode.hCost);

                            if (!openList.Contains(neighbourNode))
                            {
                                openList.Add(neighbourNode);
                            }
                        }
                    }
                }
                // Out of nodes on the openList
                return null;
            }

            private List<PathNode> GetNeighbourList(PathNode currentNode)
            {
                List<PathNode> neighbourList = new List<PathNode>();

                if (currentNode.neighbors != null)
                    return currentNode.neighbors;

                foreach (var door in currentNode.tile.doors)
                    foreach (var pathNode in map)
                        if (pathNode.tile.transform.position == door.point.position && door.hasWay)
                            neighbourList.Add(pathNode);
                currentNode.neighbors = neighbourList;
                return neighbourList;
            }
            private List<PathNode> CalculatePath(PathNode endNode)
            {
                List<PathNode> path = new List<PathNode>();
                path.Add(endNode);
                PathNode currentNode = endNode;
                while (currentNode.cameFromNode != null)
                {
                    path.Add(currentNode.cameFromNode);
                    currentNode = currentNode.cameFromNode;
                }
                path.Reverse();
                return path;
            }

            private int CalculateDistanceCost(PathNode first, PathNode second)
            {
                return (int)Vector2.Distance(first.tile.centerPoint.position, second.tile.centerPoint.position);
            }

            private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
            {
                PathNode lowestFCostNode = pathNodeList[0];
                for (int i = 1; i < pathNodeList.Count; i++)
                {
                    if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                    {
                        lowestFCostNode = pathNodeList[i];
                    }
                }
                return lowestFCostNode;
            }

        }
    }
}