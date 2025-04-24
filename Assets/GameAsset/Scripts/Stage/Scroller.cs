using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Scroller : MonoBehaviour
{
        public int topPadding;
        public int bottomPadding;
        public Row rowPrefab;
        public Action<Row> OnActiveRow;

        private ScrollRect _scrollView;
        private RectTransform _content;
        private Dictionary<int, Row> _activeRows;
        private List<int> _releaseKey;
        private ObjectPool<Row> _rowPool;
        private int _totalRows;
        private float _rowWidth;
        private float _rowHeight;
        private float _contentHeight;

        // Start is called before the first frame update
        void Awake()
        {
                _scrollView = GetComponent<ScrollRect>();
                _rowWidth = rowPrefab.GetComponent<RectTransform>().rect.width;
                _rowHeight = rowPrefab.GetComponent<RectTransform>().rect.height;
                _content = _scrollView.content;
                _activeRows = new Dictionary<int, Row>();
                _releaseKey = new List<int>();
                _rowPool = new ObjectPool<Row>(() => Instantiate(rowPrefab, _content));
                _scrollView.onValueChanged.AddListener(OnScroll);
        }

        public void Load(int totalElements)
        {
                _totalRows = (int) Mathf.Ceil((float) totalElements / rowPrefab.elements.Count);
                _contentHeight = _totalRows * _rowHeight + bottomPadding + topPadding;
                _content.sizeDelta = new Vector2(_rowWidth, _contentHeight);
                
                ReleaseRow();
                UpdateView();
        }

        void ReleaseRow()
        {
                _releaseKey.Clear();
                
                foreach (var pair in _activeRows)
                {
                        pair.Value.Index = -1;
                        pair.Value.gameObject.SetActive(false);
                        _releaseKey.Add(pair.Key);
                        _rowPool.Release(pair.Value);
                }

                foreach (var key in _releaseKey)
                {
                        _activeRows.Remove(key);
                }
        }
        
        void OnScroll(Vector2 coor)
        {
                UpdateView();
                StopScroll();
        }

        void StopScroll()
        {
                if (_scrollView.velocity.sqrMagnitude < 0.1f)
                {
                        _scrollView.velocity = Vector3.zero;
                }
        }
        
        void UpdateView()
        {
                var startY = -_content.localPosition.y;
                int startRow = (int) Mathf.Max(0, (startY - bottomPadding) / _rowHeight);
                int endRow = (int) Mathf.Min(_totalRows - 1, (startY + _scrollView.viewport.rect.height - bottomPadding) / _rowHeight);

                // Return row to pool
                _releaseKey.Clear();
                foreach (var pair in _activeRows)
                {
                        if (pair.Value.Index >= startRow && pair.Value.Index <= endRow) continue;
                        
                        pair.Value.Index = -1;
                        pair.Value.gameObject.SetActive(false);
                        _releaseKey.Add(pair.Key);
                        _rowPool.Release(pair.Value);
                }
                foreach (var key in _releaseKey)
                {
                        _activeRows.Remove(key);
                }

                // Active row
                for (int idx = startRow; idx <= endRow; idx++)
                {
                        if (_activeRows.ContainsKey(idx)) continue;
                        
                        var row = _rowPool.Get();
                        row.gameObject.SetActive(true);
                        float posY = bottomPadding + (idx + 0.5f) * _rowHeight;
                        row.transform.localPosition = new Vector2(0, posY);
                        row.Index = idx;
                        row.gameObject.name = idx.ToString();
                        row.transform.SetSiblingIndex(idx - startRow);
                        OnActiveRow?.Invoke(row);
                        _activeRows.Add(idx, row);
                }
        }
}