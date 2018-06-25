using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZCC.File;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using ExtensionClass;

public enum Difficulty {
    Easy,
    Medium,
    Hard
}

public static class WordsResource{

    public static Dictionary<string, string> WordsDic = null;
    private static List<string> Words = null;
    private static int count = 0;

    private static readonly System.Random random = new System.Random();

    public static void Init() {
        //string path = Application.streamingAssetsPath + "/" + "E2Cdictionary.txt";
        //WWW www = new WWW(path);
        //yield return www;
        string words = E2CDictionarySource.Source;
        //string words = www.text;
        //string words = FileHelper.ReadInStreamingAssets("", "E2Cdictionary.txt");
        WordsDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(words);
        Words = WordsDic.Keys.ToList();
        count = Words.Count;
        Debug.Log("初始化完成");
    }

   
    public static void _Init() {
        string words = FileHelper.ReadInStreamingAssets("", "E2Cdictionary.txt");
        WordsDic = JsonConvert.DeserializeObject<Dictionary<string, string>>(words);
        Words = WordsDic.Keys.ToList();
        count = Words.Count;
        Debug.Log("初始化完成");
    }

    /// <summary>随机获取一个单词 </summary>
    public static string GetWord() {
        int index = random.Next(0, count);
        return Words[index]/*.Remove(0,1)*/;
    }

    /// <summary>根据难度随机获取一个单词 </summary>
    public static string GetWord(Difficulty difficulty) {
        int min = 0;
        int max = 0;
        switch (difficulty) {
            case Difficulty.Easy: min = 2; max = 6; break;
            case Difficulty.Medium: min = 7; max = 11; break;
            case Difficulty.Hard: min = 12; max = 16; break;
            default:throw new System.Exception();
        }
        while (true) {
            string word = GetWord();
            if (word.Length >= min && word.Length <= max) {
                return word/*.ToUpper()*/;
            }
        }
    }

    

}
