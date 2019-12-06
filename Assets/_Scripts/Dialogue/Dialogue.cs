using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : ScriptableObject
{
    public string nomenclature;
    public string broadcaster_1;
    public string broadcaster_2;
    public List<Row> rowList = new List<Row>();
}
