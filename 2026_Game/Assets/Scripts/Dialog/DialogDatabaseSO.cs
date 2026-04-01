using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogDatabaseSO", menuName = "Dialog System/DialogDatabaseSO")]
public class DialogDatabaseSO : ScriptableObject
{
    public List<DialogSO> dialogs = new List<DialogSO>();

    private Dictionary<int, DialogSO> dialogsByld;                              //캐싱을 위한 딕셔너리 사용


    public void Initailize()
    {
        dialogsByld = new Dictionary<int, DialogSO>();

        foreach (var dialog in dialogs)
        {
            if (dialog != null)
            {
                dialogsByld[dialog.id] = dialog;
            }
        }
    }

    public DialogSO GetDialogByld(int id)
    {
        if(dialogsByld == null)
            Initailize();

        if(dialogsByld.TryGetValue(id, out DialogSO dialog))
        {
            return dialog; 
        }

        return null;
    }
}
