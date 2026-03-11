using UnityEngine;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ItemDataLoader : MonoBehaviour
{
    [SerializeField]
    private string jsonFileName = "items";

    private List<ItemData> itemList = new List<ItemData>();


    private void Start()
    {
        LoadItemData();
    }



    //한글 인코딩을 위한 핼퍼 함수

    private string EncodeKorean(string text)
    {
        if (string.IsNullOrEmpty(text)) return " ";     //텍스트가 NULL 값이면 함수를 끝낸다.
        byte[] bytes = Encoding.Default.GetBytes(text); //string을 Byte 배열로 변환한 후
        return Encoding.UTF8.GetString(bytes);          //인코딩을 UTF8로 바꾼다.
    }

    void LoadItemData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile != null)
        {
            byte[] bytes = Encoding.Default.GetBytes(jsonFile.text);
            string currnetText = Encoding.UTF8.GetString(bytes);

            itemList = JsonConvert.DeserializeObject<List<ItemData>>(currnetText);

            Debug.Log($"로드된 아이템 수 : {itemList.Count}");

            foreach (var item in itemList)
            {
                Debug.Log($"아이템 : {EncodeKorean(item.Itemname)}, 설명 : {EncodeKorean(item.description)}");
            }
        }
        else
        {
            Debug.LogError($"JSON 파일을 찾을 수 없습니다. : {jsonFileName}");
        }


    }
}

