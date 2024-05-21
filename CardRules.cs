using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

public class CardRules : MonoBehaviour
{
    public List<string> suitValues;
    public List<string> cardValues;
    public List<string> combinations;

    [ContextMenu("Export Card Rules")]
    public void ExportCardRules()
    {
        string filePath = EditorUtility.SaveFilePanel("Export Card Rules", "", "card_rules", "xml");
        if (string.IsNullOrEmpty(filePath))
            return;

        CardRulesData data = new CardRulesData
        {
            SuitValues = suitValues.ToArray(),
            CardValues = cardValues.ToArray(),
            Combinations = combinations.ToArray()
        };

        XmlSerializer serializer = new XmlSerializer(typeof(CardRulesData));
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializer.Serialize(stream, data);
        }

        Debug.Log("Card rules exported successfully.");
    }

    [ContextMenu("Import Card Rules")]
    public void ImportCardRules()
    {
        string filePath = EditorUtility.OpenFilePanel("Import Card Rules", "", "xml");
        if (string.IsNullOrEmpty(filePath))
            return;

        XmlSerializer serializer = new XmlSerializer(typeof(CardRulesData));
        using (FileStream stream = new FileStream(filePath, FileMode.Open))
        {
            CardRulesData data = serializer.Deserialize(stream) as CardRulesData;
            if (data != null)
            {
                suitValues = new List<string>(data.SuitValues);
                cardValues = new List<string>(data.CardValues);
                combinations = new List<string>(data.Combinations);
            }
        }

        Debug.Log("Card rules imported successfully.");
    }
}

[System.Serializable]
public class CardRulesData
{
    public string[] SuitValues;
    public string[] CardValues;
    public string[] Combinations;
}
