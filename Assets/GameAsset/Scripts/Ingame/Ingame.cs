using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Ingame : MonoBehaviour
{
        public GameObject floorPrefab;
        public GameObject wallPrefab;
        public GameObject pathPrefab;
        public Transform maze;
        public Transform bug;
        public Transform endPoint;

        private RectTransform _floorRect;
        private RectTransform _wallRect;
        private MazeData _mazeData;
        private Transform[,] _cellTransforms;
        private List<Vector2Int> _path;
        private bool _drawedPath;

        void Awake()
        {
                _floorRect = floorPrefab.GetComponent<RectTransform>();
                _wallRect = wallPrefab.GetComponent<RectTransform>();
        }

        public void LoadStage(MazeData data)
        {
                _mazeData = data;
                DestroyMaze();
                LoadMaze();
                Reset();
        }
        
        public void OnClickFindPath()
        {
                if (_drawedPath) return;
                
                _drawedPath = true;
                _path = PathFinding.FindPath(_mazeData);
                DrawPath(_path);
        }

        public void OnClickRun()
        {
                OnClickFindPath();
                
                Vector3[] path3D = new Vector3[_path.Count];
                for (int idx = 0; idx < _path.Count; idx++)
                {
                        path3D[idx] = GetCellPos(_path[idx]);
                }

                bug.localPosition = path3D[0];
                DOTween.Kill(bug);
                bug.DOLocalPath(path3D, (float) _path.Count / 10, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear).SetLookAt(0.01f);
        }

        void Reset()
        {
                _drawedPath = false;
                _path = null;
                DOTween.Kill(bug);
                bug.localEulerAngles = Vector3.zero;
                bug.localPosition = GetCellPos(_mazeData.startPoint);
                endPoint.localPosition = GetCellPos(_mazeData.endPoint);
        }

        void DestroyMaze()
        {
                for (int idx = maze.childCount - 1; idx >= 0; idx--)
                {
                        Destroy(maze.GetChild(idx).gameObject);
                }
        }

        void LoadMaze()
        {
                _cellTransforms = new Transform[_mazeData.GetHeight(), _mazeData.GetWidth()];

                var startPosX = -_floorRect.rect.width * (_mazeData.GetWidth() + 1) / 4 - _wallRect.rect.width * (_mazeData.GetWidth() - 1) / 4;
                var startPosY = _floorRect.rect.height * (_mazeData.GetHeight() + 1) / 4 + _wallRect.rect.height * (_mazeData.GetHeight() - 1) / 4;

                for (int y = 0; y < _mazeData.GetHeight(); y++)
                {
                        for (int x = 0; x < _mazeData.GetWidth(); x++)
                        {
                                var cellGo = _mazeData.GetCell(x, y) == CellType.Floor ? Instantiate(floorPrefab, maze.transform) : Instantiate(wallPrefab, maze.transform);
                                var cell = cellGo.GetComponent<RectTransform>();
                                cell.name = x + "_" + y;
                                var width = x % 2 == 0 ? _floorRect.rect.width : _wallRect.rect.width;
                                var height = y % 2 == 0 ? _floorRect.rect.height : _wallRect.rect.height;
                                cell.sizeDelta = new Vector2(width, height);
                                var posX = startPosX + _floorRect.rect.width * (x + 1) / 2 + _wallRect.rect.width * x / 2;
                                var posY = startPosY - _floorRect.rect.height * (y + 1) / 2 - _wallRect.rect.height * y / 2;
                                cell.localPosition = new Vector2(posX, posY);
                                _cellTransforms[y, x] = cellGo.transform;
                        }
                }
        }

        Transform GetCell(Vector2Int coor)
        {
                if (_mazeData.IsValidCell(coor))
                {
                        return _cellTransforms[coor.y, coor.x];
                }

                return null;
        }
        
        Vector2 GetCellPos(int x, int y)
        {
                return _cellTransforms[y, x].localPosition;
        }

        Vector2 GetCellPos(Vector2Int coor)
        {
                return _cellTransforms[coor.y, coor.x].localPosition;
        }
        
        void DrawPath(List<Vector2Int> path)
        {
                // Create segment
                var segments = new List<Vector2Int>();
                segments.Add(path[0]);
                Vector2 prevDir = path[1] - path[0];
                for (int idx = 2; idx < path.Count; idx++)
                {
                        Vector2 currentDir = path[idx] - path[idx - 1];
                        if (currentDir != prevDir)
                        {
                                segments.Add(path[idx - 1]);
                        }
                        prevDir = currentDir;
                }
                segments.Add(path[^1]);
                
                // Draw segment
                for (int idx = 0; idx < segments.Count - 1; idx++)
                {
                        var pathRect = Instantiate(pathPrefab, maze).GetComponent<RectTransform>();
                        var curPos = GetCellPos(segments[idx]);
                        var nextPos = GetCellPos(segments[idx + 1]);
                        var width = segments[idx + 1].y == segments[idx].y ? Mathf.Abs(nextPos.x - curPos.x) + pathRect.rect.width  : pathRect.rect.width;
                        var height = segments[idx + 1].x == segments[idx].x ? Mathf.Abs(nextPos.y - curPos.y) + pathRect.rect.height : pathRect.rect.height;
                        var x = segments[idx + 1].x == segments[idx].x ? curPos.x : (curPos.x + nextPos.x) / 2;
                        var y = segments[idx + 1].y == segments[idx].y ? curPos.y : (curPos.y + nextPos.y) / 2;
                         pathRect.sizeDelta = new Vector2(width, height);
                        pathRect.localPosition = new Vector2(x, y);
                }
        }
}