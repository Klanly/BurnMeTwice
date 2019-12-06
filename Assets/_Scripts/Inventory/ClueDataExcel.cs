using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueDataExcel : MonoBehaviour
{

    public TextAsset clueExcel;
    public List<ClueRow> clueRowList = new List<ClueRow>();
    PersistentData persistentData;

    // Start is called before the first frame update
    void Start()
    {
        persistentData = FindObjectOfType<PersistentData>();
        string[] fila = clueExcel.text.Split(new char[] { '\n' });

        //Save the rows
        for (int i = 1; i < fila.Length - 1; i++)
        {
            string[] columna = fila[i].Split(new char[] { ';' });
            if (columna[0] != "")
            {
                ClueRow row = new ClueRow();
                row.nomenclature = columna[0];
                row.clueNameEs = columna[1];
                row.clueNameEn = columna[2];
                row.clueDescriptionEs = columna[3];
                row.clueDescriptionEn = columna[4];

                clueRowList.Add(row);
            }
        }
    }

   
    #region LoadClueData
    public string LoadDescriptionInClue(string nomenclature)
    {
        string descriptionText="";

        foreach (ClueRow c in clueRowList)
        {
            if (nomenclature == c.nomenclature)
            {
                switch (persistentData.language)
                {
                    case PersistentData.Langague.Spanish:
                        descriptionText = c.clueDescriptionEs;
                        break;

                    case PersistentData.Langague.English:
                        descriptionText = c.clueDescriptionEn;
                        break;

                }
                break;
              
            }

        }

        return descriptionText;
    }

    public string LoadNameInClue(string nomenclature)
    {
        string nameText = "";

        foreach (ClueRow c in clueRowList)
        {
            if (nomenclature == c.nomenclature)
            {
                switch (persistentData.language)
                {
                    case PersistentData.Langague.Spanish:
                       nameText = c.clueNameEs;
                        break;

                    case PersistentData.Langague.English:
                        nameText = c.clueNameEn;
                        break;

                }
                break;

            }

        }

        return nameText;
    }
    #endregion
}
