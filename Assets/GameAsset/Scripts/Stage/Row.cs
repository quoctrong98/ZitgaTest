using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
        public List<Element> elements;
        public Transform lineH;

        public int Index { get; set; }

        public void Load(EData[] datas)
        {
                int startElementIdx = Index * elements.Count;

                if (startElementIdx + elements.Count < datas.Length)
                {
                        lineH.gameObject.SetActive(true);
                }
                else
                {
                        lineH.gameObject.SetActive(false);
                }
               
                lineH.localPosition = Index % 2 == 0 ? new Vector2(170,  lineH.localPosition.y) : new Vector2(-170, lineH.localPosition.y);

                for (int idx = 0; idx < elements.Count; idx++)
                {
                        var diff = Index % 2 == 0 ? idx : elements.Count - idx - 1;
                        
                        if (startElementIdx + diff >= datas.Length)
                        {
                                elements[idx].gameObject.SetActive(false);
                        }
                        else
                        {
                                elements[idx].gameObject.SetActive(true);
                                elements[idx].Load(startElementIdx + diff, datas[startElementIdx + diff]);
                        }
                }
        }
}