using UnityEngine;

public struct EData
{
        public int star;
        public bool locking;
}

public class Controller : MonoBehaviour
{
        public Scroller _scroller;
        
        private EData[] _eData;
        
        void Awake()
        {
                _scroller.OnActiveRow = OnActiveRow;
        }

        void LoadScroller()
        {
                _scroller.Load(_eData.Length);
        }

        public void Init(StageData[] stageDatas, int unlockedStage)
        {
                InitData(stageDatas, unlockedStage);
                LoadScroller();
        }
        
        public void InitData(StageData[] stageDatas, int unlockedStage)
        {
                _eData = new EData[stageDatas.Length];

                for (int idx = 0; idx < stageDatas.Length; idx++)
                {
                        _eData[idx].star = stageDatas[idx].star;
                        
                        if (idx < unlockedStage)
                        {
                                _eData[idx].locking = false;
                        }
                        else
                        {
                                _eData[idx].locking = true;
                        }
                }
        }

        void OnActiveRow(Row row)
        {
                row.Load(_eData);
        }
}