using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MazeGenerator : MonoBehaviour
{
        private MazeData mazeData;

        private Vector2Int[] FrontierDirection = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

        // Generate perfect maze using prim algorithm
        public MazeData GenerateMaze(int width, int height)
        {
                mazeData = new MazeData
                {
                        cells = new CellType[height, width],
                        startPoint = new Vector2Int(0, 0)
                };
                List<Vector2Int> vistedCells = new List<Vector2Int>();
                vistedCells.Add(mazeData.startPoint);
                mazeData.SetCell(mazeData.startPoint, CellType.Floor);

                List<Frontier> frontierList = new List<Frontier>();
                foreach (var direction in FrontierDirection)
                {
                        var frontier = new Frontier
                        {
                                coor = mazeData.startPoint + direction * 2,
                                direction = direction
                        };

                        if (IsValidFrontier(mazeData, frontier))
                        {
                                frontierList.Add(frontier);
                        }
                }

                while (frontierList.Count > 0)
                {
                        var randomIndex = Random.Range(0, frontierList.Count);
                        var frontier = frontierList[randomIndex];
                        frontierList.RemoveAt(randomIndex);

                        if (vistedCells.Contains(frontier.coor)) continue;

                        vistedCells.Add(frontier.coor);

                        if (IsValidFrontier(mazeData, frontier))
                        {
                                Vector2Int inBetween = frontier.coor - frontier.direction;
                                mazeData.SetCell(inBetween, CellType.Floor);
                                mazeData.SetCell(frontier.coor, CellType.Floor);

                                foreach (var direction in FrontierDirection)
                                {
                                        var newFrontier = new Frontier
                                        {
                                                coor = frontier.coor + direction * 2,
                                                direction = direction
                                        };

                                        if (IsValidFrontier(mazeData, newFrontier))
                                        {
                                                frontierList.Add(newFrontier);
                                        }
                                }
                        }
                }

                // Set endpoint
                do
                {
                        var randomX = Random.Range(0, mazeData.cells.GetLength(1));
                        var randomY = Random.Range(0, mazeData.cells.GetLength(1));

                        mazeData.endPoint = new Vector2Int(randomX, randomY);
                } while (mazeData.GetCell(mazeData.endPoint) == CellType.Wall || mazeData.endPoint == Vector2Int.zero || mazeData.endPoint.x % 2 != 0 || mazeData.endPoint.y % 2 != 0);

                return mazeData;
        }

        bool IsValidFrontier(MazeData maze, Frontier frontier)
        {
                return maze.IsValidCell(frontier.coor) && maze.IsWall(frontier.coor);
        }
}

public struct MazeData
{
        public CellType[,] cells;
        public Vector2Int startPoint;
        public Vector2Int endPoint;

        public int GetWidth()
        {
                return cells.GetLength(1);
        }

        public int GetHeight()
        {
                return cells.GetLength(0);
        }

        public CellType GetCell(Vector2Int coor)
        {
                if (IsValidCell(coor))
                {
                        return cells[coor.y, coor.x];
                }

                return CellType.Wall;
        }

        public void SetCell(Vector2Int coor, CellType type)
        {
                cells[coor.y, coor.x] = type;
        }

        public bool IsValidCell(Vector2Int coor)
        {
                return coor.x >= 0 && coor.x < GetWidth() && coor.y >= 0 && coor.y < GetHeight();
        }

        public bool IsPath(Vector2Int coor)
        {
                return IsValidCell(coor) && cells[(int)coor.y, (int)coor.x] == CellType.Floor;
        }

        public bool IsWall(Vector2Int coor)
        {
                return IsValidCell(coor) && cells[(int)coor.y, (int)coor.x] == CellType.Wall;
        }

        public CellType GetCell(int x, int y)
        {
                if (IsValidCell(x, y))
                {
                        return cells[y, x];
                }

                return CellType.Wall;
        }

        public void SetCell(int x, int y, CellType type)
        {
                cells[y, x] = type;
        }

        public bool IsValidCell(int x, int y)
        {
                return x >= 0 && x < GetWidth() && y >= 0 && y < GetHeight();
        }

        public bool IsPath(int x, int y)
        {
                return IsValidCell(x, y) && cells[y, x] == CellType.Floor;
        }

        public bool IsWall(int x, int y)
        {
                return IsValidCell(x, y) && cells[y, x] == CellType.Wall;
        }
}

public enum CellType
{
        Wall = 0,
        Floor = 1
}

public struct Frontier
{
        public Vector2Int coor;
        public Vector2Int direction;
}