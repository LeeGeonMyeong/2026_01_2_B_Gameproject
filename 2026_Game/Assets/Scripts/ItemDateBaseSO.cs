using System.Collections.Generic;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDateBaseSO", menuName = "Scriptable Objects/ItemDateBaseSO")]
public class ItemDateBaseSO : ScriptableObject
{
    public List<ItemSO> items = new List<ItemSO>();                                         //ItemSO를 리스트로 관리한다.

    //캐싱을 위한 Dictrionary
    private Dictionary<int, ItemSO> itemsByld;                                              //ID로 아이템을 찾기 위한 캐싱   
    private Dictionary<string, ItemSO> itemsByName;                                         //이름으로 아이템 찾기


    public void Initialze()
    {
        itemsByld = new Dictionary<int, ItemSO>();                                          //위에 선언만 했기 때문에 dDivtionary 할당
        itemsByName = new Dictionary<string, ItemSO>();

        foreach(var item in items)
        {
            itemsByld[item.id] = item;
            itemsByName[item.itemName] = item;
        }
    }


    //ID로 아이템 찾기

    public ItemSO GetItemByld(int id)
    {
        if(itemsByld == null)                                                       //캐싱이 되어 있는지 확인하고 아니면 초기화 한다.
        {
            Initialze();    
        }
        if(itemsByld.TryGetValue(id, out ItemSO item))                              //id 값을 찾아서 ItemSO를 리턴한다.
            return item;

        return null;                                                                //없을 경우 NULL
    }

    //이름으로 아이템 찾기

    public ItemSO GetItemBYName(string name)                        
    {
        if(itemsByName == null)
        {
            Initialze();
        }

        if (itemsByName.TryGetValue(name, out ItemSO item))
            return item;

        return null;    
    }

    //타입으로 아이템 필터링

    public List<ItemSO> GetIteByType(ItemType type)
    {
        return items.FindAll(item => item.itemType == type);
    }
}
