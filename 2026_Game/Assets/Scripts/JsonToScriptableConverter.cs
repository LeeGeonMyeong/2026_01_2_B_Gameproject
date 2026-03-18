#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Collections;



public class JsonToScriptableConverter : EditorWindow
{

    private string jsonFilePath = "";                                           //JSON 파일 경로 문자열 값
    private string outputFolder = "Assets/ScriptableObjects/Items";             //출력 SO 파일 경로 값
    private bool createDatabase = true;                                         //데이터 베이스 활용 여부 체크 값

    [MenuItem("Tools/JSON to Scriptable Objects")]

    public static void ShowWiondow()
    {
        GetWindow<JsonToScriptableConverter>("JSON to Scriptable Objects");
    }


    void OnGUI()
    {
        GUILayout.Label("JSON to Scriptable object Converter", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(GUILayout.Button("Select JSON File"))
        {
            jsonFilePath = EditorUtility.OpenFilePanel("Select JSON File", "", "json");
        }

        EditorGUILayout.LabelField("Selected File : ", jsonFilePath);
        EditorGUILayout.Space();
        outputFolder = EditorGUILayout.TextField("Output Folder : ", outputFolder);
        createDatabase = EditorGUILayout.Toggle("Create Database Asset", createDatabase);
        EditorGUILayout.Space();

        if(GUILayout.Button("Convert to Scriptable Objects"))
        {
            if(string.IsNullOrEmpty(jsonFilePath))
            {
                EditorUtility.DisplayDialog("Error", "Pease Select a JSON file first", "OK");
                return;
            }
            ConvertJsonToScriptableObjects();
        }
    }


    private void ConvertJsonToScriptableObjects()                            //JSON 파일을 ScriptableObuect 파일로 변환 시켜주는 함수
    {
        //폴더 생성
        if (!Directory.Exists(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
        }

        //JSON 파일 읽기
        string jsonText = File.ReadAllText(jsonFilePath);

        try
        {
            //JSON 피싱
            List<ItemData> itemDataList = JsonConvert.DeserializeObject<List<ItemData>>(jsonText);

            List<ItemSO> createdItems = new List<ItemSO>();             //ItemSO 리스트 생성

            //각 아이템을 데이터 스크립터블 오브젝트로 변환
            foreach (ItemData itemData in itemDataList)
            {
                ItemSO itemSO = ScriptableObject.CreateInstance<ItemSO>();                  //ItemSO 파일을 생성

                //데이터 복사
                itemSO.id = itemData.id;
                itemSO.itemName = itemData.itemName;
                itemSO.nameEng = itemData.nameEng;
                itemSO.description = itemData.description;

                //열거형 변환
                if(System.Enum.TryParse(itemData.itemTypeString, out ItemType parsedType))
                {
                    itemSO.itemType = parsedType;
                }
                else
                {
                    Debug.LogWarning($"아이템 {itemData.itemName}의 유효하지 않은 타입 : {itemData.itemTypeString}");
                }

                itemSO.price = itemData.price;
                itemSO.power = itemData.power;
                itemSO.level = itemData.level;
                itemSO.isStackable = itemData.isStackable;

                //아이콘 로드 (경로가 있는 경우)
                if (!string.IsNullOrEmpty(itemData.iconPath))           //아이콘 경로가 있는지 확인한다.
                {
                    itemSO.icon = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Resouces/{itemData.iconPath}.png");

                    if (itemSO.icon == null)
                    {
                        Debug.LogWarning($"아이템{itemData.nameEng} 의 아이큰을 찾을 수 없습니다. : {itemData.iconPath}");
                    }

                    //스크립터블 오브젝트 저장 - ID 를 4자리 숫자로 포맷팅
                    string assetPath = $"{outputFolder}/Item_{itemData.id.ToString("D4")} {itemData.nameEng}.asset";
                    AssetDatabase.CreateAsset(itemSO, assetPath);


                    //셋 이름 저장
                    itemSO.name = $"Item_{itemData.id.ToString("D4")} + {itemData.nameEng}";
                    createdItems.Add(itemSO);

                    EditorUtility.SetDirty(itemSO);

                    //데이터베이스
                    if(createDatabase && createdItems.Count > 0 )
                    {
                        ItemDateBaseSO dataBase = ScriptableObject.CreateInstance<ItemDateBaseSO>();
                        dataBase.items = createdItems;

                        AssetDatabase.CreateAsset(dataBase, $"{outputFolder}/ItemDatabase.asset");
                        EditorUtility.SetDirty(dataBase);
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    EditorUtility.DisplayDialog("Sucess", $"Created {createdItems.Count} scriptable objects!", "OK");
                }
            }
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Failed to Convert JSON : {e.Message}", "OK");
            Debug.LogError($"JSON 변환 오류 : {e}");
        }
    }
}

#endif 