using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogSO", menuName = "Dialog System/DialogSO")]
public class DialogSO : ScriptableObject
{
    public int id;
    public string characterName;
    public string text;
    public int nextld;


    public List<DialogChoiceSo> choices = new List<DialogChoiceSo>();
    public Sprite portrait;

    public string portraitPath;
}
