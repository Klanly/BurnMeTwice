using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Clue", menuName = "Clue", order = 1)]
public class ScriptableObjectClueClass : ScriptableObject
{
    public string clueName;
    public Sprite clueImage;
    public string clueDescription;
}
