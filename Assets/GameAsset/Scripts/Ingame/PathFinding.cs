using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
        public Vector2Int coor;
        public Node parent;
        public int gCost;
        public int hCost;

        public int fCost
        {
                get { return gCost + hCost; }
        }

        public Node(Vector2Int coor)
        {
                this.coor = coor;
                parent = null;
                gCost = 0;
                hCost = 0;
        }
}

public class PathFinding : MonoBehaviour
{
        private static Vector2Int[] Direction = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        
        public static List<Vector2Int> FindPath(MazeData mazeData)
        {
                var path = new List<Vector2Int>();

                Dictionary<Vector2Int, Node> unVistedNodes = new Dictionary<Vector2Int, Node>();
                HashSet<Vector2Int> visitedCoor = new HashSet<Vector2Int>();

                Node startNode = new Node(mazeData.startPoint);
                startNode.hCost = CalculateHeuristic(mazeData.startPoint, mazeData.endPoint);
                unVistedNodes.Add(startNode.coor, startNode);

                while (unVistedNodes.Count > 0)
                {
                        Node currentNode = GetNodeWithLowestFCost(unVistedNodes);
                        
                        if (currentNode.coor == mazeData.endPoint)
                        {
                                return ReconstructPath(currentNode);
                        }
                        
                        unVistedNodes.Remove(currentNode.coor);
                        visitedCoor.Add(currentNode.coor);
                        
                        foreach (var direction in Direction)
                        {
                                var neighborCoor = currentNode.coor + direction;
                                if(ShouldSkipCoor(neighborCoor, mazeData, visitedCoor)) continue;
                                
                                int newGCost = currentNode.gCost + 1;
                                
                                Node neighborNode = FindNodeInList(unVistedNodes, neighborCoor);
                                
                                if (neighborNode == null || newGCost < neighborNode.gCost)
                                {
                                        if (neighborNode == null)
                                        {
                                                neighborNode = new Node(neighborCoor);
                                                unVistedNodes.Add(neighborCoor, neighborNode);
                                        }
                                        
                                        neighborNode.gCost = newGCost;
                                        neighborNode.hCost = CalculateHeuristic(neighborCoor, mazeData.endPoint);
                                        neighborNode.parent = currentNode;
                                }
                        }
                }

                return path;
        }

        static bool ShouldSkipCoor(Vector2Int coor, MazeData mazeData, HashSet<Vector2Int> visitedCoor)
        {
                return !mazeData.IsValidCell(coor) || mazeData.IsWall(coor) || visitedCoor.Contains(coor);
        }
        
        static Node GetNodeWithLowestFCost(Dictionary<Vector2Int, Node> nodeDictionary)
        {
                Node lowestCostNode = null;
                int lowestFCost = int.MaxValue;
                int lowestHCost = int.MaxValue;
    
                foreach (var pair in nodeDictionary)
                {
                        Node node = pair.Value;
        
                        if (node.fCost < lowestFCost || 
                            (node.fCost == lowestFCost && node.hCost < lowestHCost))
                        {
                                lowestFCost = node.fCost;
                                lowestHCost = node.hCost;
                                lowestCostNode = node;
                        }
                }
    
                return lowestCostNode;
        }
        
        private static Node FindNodeInList(Dictionary<Vector2Int, Node> nodeDictionary, Vector2Int coor)
        {
                if (nodeDictionary.TryGetValue(coor, out Node node))
                {
                        return node;
                }
                
                return null;
        }

        static int CalculateHeuristic(Vector2Int a, Vector2Int b)
        {
                return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }

        static List<Vector2Int> ReconstructPath(Node goalNode)
        {
                List<Vector2Int> path = new List<Vector2Int>();
                Node currentNode = goalNode;

                while (currentNode != null)
                {
                        path.Add(currentNode.coor);
                        currentNode = currentNode.parent;
                }

                path.Reverse();
                return path;
        }
}