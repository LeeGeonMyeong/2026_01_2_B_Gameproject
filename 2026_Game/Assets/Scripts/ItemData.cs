using UnityEngine;
using System;

[Serializable]
public class ItemData
{
    public int id;
    public string Itemname;
    public string description;
    public string nameEng;
    public string ItemTypeString;

    [NonSerialized]
    public ItemType itemType;
    public int price;
    public int power;
    public int level;
    public bool isStackable;
    public string iconPath;


    public void InitalizeEnums()
    {
        if(Enum.TryParse(ItemTypeString,out ItemType parsedType))
        {
            itemType = parsedType;
        }
        else
        {
            Debug.LogError($"아이템 '{Itemname} 에 유효하지 않은 아이템 타입 :{ItemTypeString}");

            itemType = ItemType.Consumable;
        }
    }

}
