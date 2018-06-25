using ExtensionClass;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamingManager : MonoBehaviour {

    public static GamingManager Instance { get; private set; } = null;

    public Color ItemDefaultColor;
    public Color ItemSelectedColor;
    public GameObject ItemParentNode;
    private const float width = 1080;

    private List<Item> selectedItems = new List<Item>();
    public Difficulty difficulty = Difficulty.Easy;
    private string currentWord;

    public GameObject GetWordParentNode;
    private List<GameObject> WordItems = new List<GameObject>();

    public GameObject ResultPanel;
    public Text ResultWord;
    public Text ResultDetail;

    public Text ScoreText;
    private int score = 0;
    public Text DifficultyText;

    public Text TipText;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        InitGame();
    }

    public bool Draging { get; private set; } = false;

    [ContextMenu("InitGame")]
    public void InitGame() {

        ResetGame();

        List<Transform> childTransform = new List<Transform>();
        foreach(Transform child in ItemParentNode.transform) {
            childTransform.Add(child);
        }
        childTransform.ForEach(transform => DestroyImmediate(transform.gameObject));

        childTransform.Clear();
        foreach(Transform child in GetWordParentNode.transform) {
            childTransform.Add(child);
        }
        childTransform.ForEach(transform => DestroyImmediate(transform.gameObject));

        WordItems.Clear();

        //初始化新数据
        currentWord = WordsResource.GetWord(difficulty);
        Debug.Log(currentWord);
        char[,] matrix = WordsFactory.GetMatrix(currentWord);
        GameObject itemPrefeb = Resources.Load<GameObject>("Item");
        int edge = matrix.GetLength(0);
        for (int i = 0; i < edge; i++) {
            for (int j = 0; j < edge; j++) {
                GameObject newItem = Instantiate(itemPrefeb, ItemParentNode.transform);
                newItem.GetComponent<Item>().Init(matrix[i, j], new Pointer(i, j));
            }
        }
        float sizeLength = (width - 2 * 20 - (edge - 1) * 10) / edge;
        ItemParentNode.GetComponent<GridLayoutGroup>().cellSize = new Vector2(sizeLength, sizeLength);

        //初始化单词显示
        GameObject wordItemPrefeb = Resources.Load<GameObject>("WordItem");
        for (int i=0; i <currentWord.Length; i++) {
            GameObject newItem = Instantiate(wordItemPrefeb, GetWordParentNode.transform);
            newItem.transform.Find("Text").GetComponent<Text>().text = "";
            WordItems.Add(newItem);
        }

        TipText.text = currentWord;
        ResultWord.text = currentWord;
        ResultDetail.text = WordsResource.WordsDic[currentWord];

    }

    /// <summary>开始拖动</summary>
    public void StartDrag(Item item) {
        Draging = true;
        selectedItems.Add(item);
        WordItems[selectedItems.Count - 1].transform.Find("Text").GetComponent<Text>().text = item.letter.ToString().ToUpper();
    }

    /// <summary>拖动途径 </summary>
    public void CoverDarg(Item item) {
        
        selectedItems.Add(item);
        WordItems[selectedItems.Count - 1].transform.Find("Text").GetComponent<Text>().text = item.letter.ToString().ToUpper();
        if (selectedItems.Count == WordItems.Count) {
            EndDrag();
        }
    }

    /// <summary>结束拖动 </summary>
    public void EndDrag() {
        if (!Draging) {
            return;
        }
        Debug.Log("EndDrag");
        Draging = false;
        //游戏条件判定
        string word = "";
        foreach(Item item in selectedItems) {
            word += item.letter;
        }
        if(word== currentWord) {
            //游戏胜利 
            Debug.Log("本局胜利");
            ResultPanel.SetActive(true);
            if (difficulty == Difficulty.Easy) {
                score += 1;
            }
            else if (difficulty == Difficulty.Medium) {
                score += 2;
            }
            else if (difficulty == Difficulty.Hard) {
                score += 3;
            }
            ScoreText.text = score.ToString();
        }
        else {
            //游戏失败
            Debug.Log("本局失败");
            ResetGame();
        }
        
    }

    /// <summary>重置当前界面 相当于重新当前游戏</summary>
    private void ResetGame() {
        foreach(Item item in selectedItems) {
            item.SetDefault();
        }
        foreach(GameObject g in WordItems) {
            g.transform.Find("Text").GetComponent<Text>().text = "";
        }
        selectedItems.Clear();
        Draging = false;
    }

    public ToggleGroup DifficultyToggleGroup;
    public void OnChangeDifficulty(bool selected) {
        if (!selected) {
            return;
        }
        string toggleName = DifficultyToggleGroup.ActiveToggles().First().name;
        if (toggleName == "Easy") {
            difficulty = Difficulty.Easy;
            DifficultyText.text = "简单";
        }
        else if(toggleName == "Medium") {
            difficulty = Difficulty.Medium;
            DifficultyText.text = "中等";
        }
        else if (toggleName == "Hard") {
            difficulty = Difficulty.Hard;
            DifficultyText.text = "困难";
        }
        InitGame();
    }


    public void ReturnMainPage() {
        SceneManager.LoadScene(0);
    }

}
