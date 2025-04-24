using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
        public Controller menu;
        public Ingame ingame;
        public MazeGenerator mazeGenerator;
        
        public static GameManager Instance { get; private set; }
        
        // Data
        private bool firstLogin;
        private Dictionary<int, MazeData> mazeDatas;
        private StageData[] stageDatas;
        private int unlockedStage;

        private void Awake()
        {
                Application.targetFrameRate = 60;
                
                Instance = this;
                LoadData();
        }

        void LoadData()
        {
                firstLogin = ES3.Load("FirstLogin", true);
                mazeDatas =  ES3.Load("MazeDatas", new Dictionary<int, MazeData>());
                stageDatas = ES3.Load("StageDatas", new StageData[1000]);
                unlockedStage = ES3.Load("UnlockedStage", 1);
                ES3.Save("FirstLogin", false);
                
                if (!firstLogin) return;
                RandomStageData();
        }

        void ClearMazeData()
        {
                mazeDatas.Clear();
                ES3.Save("MazeDatas", mazeDatas);
        }

        void RandomStageData()
        {
                unlockedStage = Random.Range(1, stageDatas.Length);

                for (int idx = 0; idx < stageDatas.Length; idx++)
                {
                        if (idx < unlockedStage)
                        {
                                stageDatas[idx].star = Random.Range(1, 3);
                        }
                        else
                        {
                                stageDatas[idx].star = 0;
                        }
                }
                
                ES3.Save("UnlockedStage", unlockedStage);
                ES3.Save("StageDatas", stageDatas);
        }
        
        void Start()
        {
                menu.Init(stageDatas, unlockedStage);
        }

        MazeData GetMazeData(int stage)
        {
                var index = stage - 1;
                if (!mazeDatas.ContainsKey(index))
                {
                        var mazeData = mazeGenerator.GenerateMaze(19, 25);
                        mazeDatas.Add(index, mazeData);
                        
                        ES3.Save("MazeDatas", mazeDatas);
                }
                
                return mazeDatas[index];
        }

        public void LoadStage(int stage)
        {
                menu.gameObject.SetActive(false);
                ingame.gameObject.SetActive(true);
                ingame.LoadStage(GetMazeData(stage));
        }

        public void OnClickMenu()
        {
                menu.gameObject.SetActive(true);
                ingame.gameObject.SetActive(false);
        }

        public void OnClickReset()
        {
                ClearMazeData();
                RandomStageData();
                menu.Init(stageDatas, unlockedStage);
        }
}

public struct StageData
{
        public int star;
}