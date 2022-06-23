using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataReader : MonoBehaviour
{
    // public List<LevelData> ReadCSVFile(string filename)
    // {
    //     int currentLineNumber = 0;
    //     int columnCount = 0;
    //     TextAsset txt = Resources.Load<TextAsset>(filename);
    //     string[] lines = txt.text.Split('\n');
    //     foreach (string line in lines)
    //     {
    //         if (line == "")
    //         {
    //             continue;
    //         }
    //
    //         string[] lineSplitted = line.Split(',');
    //         currentLineNumber++;
    //         
    //         if (currentLineNumber == 1)
    //         {
    //             columnCount = lineSplitted.Count();
    //             continue;
    //         }
    //         int rewardCount = int.Parse(lineSplitted[0]);
    //         int numOfRounds = int.Parse(lineSplitted[1]);
    //         List<CardPickerData> cardPickerDatas = new List<CardPickerData>();
    //         cardPickerDatas.Clear();
    //         for (int j = 0; j < numOfRounds; j++)
    //         {
    //             CharacterName character1 = CharacterName.Defult, character2 = CharacterName.Defult;
    //             int count = 0;
    //             for (int i = 0; i < 3; i++)
    //             {
    //                 int coefficient = j * 3 + 2 + i;
    //                 if (coefficient % 3 == 2)
    //                 {
    //                     character1 = CharacterStringToName(lineSplitted[coefficient]);
    //                 }
    //                 if (coefficient % 3 == 0)
    //                 {
    //                     character2 = CharacterStringToName(lineSplitted[coefficient]);
    //                 }
    //                 if (coefficient % 3 == 1)
    //                 {
    //                     count = int.Parse(lineSplitted[coefficient]);
    //                 }
    //             }
    //             CardPickerData cardPickerData = new CardPickerData(character1, character2, count);
    //             cardPickerDatas.Add(cardPickerData);
    //         }
    //         // rewardCount %= 1000;
    //         data.Add((rewardCount, numOfRounds, cardPickerDatas));
    //     }
    //     return data;
    // }
}
