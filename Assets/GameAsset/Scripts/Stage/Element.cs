using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Element : MonoBehaviour
{
        public TextMeshProUGUI text;
        public Transform starParent;
        public GameObject lockGO;

        public int Index { get; set; }
        private List<Transform> stars;
        
        // Start is called before the first frame update
        void Awake()
        {
                stars = new List<Transform>();
                for (int idx = 0; idx < starParent.childCount; idx++)
                {
                        stars.Add(starParent.GetChild(idx));
                }
        }

        public void Load(int index, EData data)
        {
                Index = index;
                text.text = index ==  0 ? "Tutorial" : index.ToString();
                lockGO.SetActive(data.locking);
                starParent.gameObject.SetActive(!data.locking);
                
                if (data.locking) return;
                for (int idx = 0; idx < stars.Count; idx++)
                {
                        stars[idx].gameObject.SetActive(idx <= data.star);
                }
        }

        public void OnClick()
        {
                GameManager.Instance.LoadStage(Index + 1);
        }
}