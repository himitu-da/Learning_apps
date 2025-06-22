using Lean.Gui;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnlockWord : MonoBehaviour
{
    DatabaseManager databaseManager;
    DataManager dataManager;
    public LeanButton[] buttons;
    public GameObject[] cards;
    int[] needFaith = CONSTANTSFIND.NEEDFAITH, getKnowledge = CONSTANTSFIND.GETKNOWLEDGE;
    public SetWord setWord;

    // The pool of the next potential words to be unlocked.
    private List<int> unlockableWordPool = new List<int>();
    private HashSet<int> unlockedWordIDs = new HashSet<int>();
    private int currentWordSetIndex = -1;
    private int lastCheckedWordID = 0;
    private const int POOL_SIZE = 50;

    void Start()
    {
        databaseManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DatabaseManager>();
        dataManager = GameObject.FindGameObjectWithTag(CONSTANTS.GAMEMANAGER_TAG).GetComponent<DataManager>();
    }

    public void Set()
    {
        currentWordSetIndex = setWord.GetCurrectWordSetIndex();
        print("現在のワードセットインデックスは" + currentWordSetIndex);

        // Initialize state for the new word set
        lastCheckedWordID = 0;
        unlockedWordIDs = new HashSet<int>(databaseManager.UnlockDao.GetUnlockIDList(currentWordSetIndex));
        unlockableWordPool.Clear();
        
        FillUnlockableWordPool();
        SetupButtons();
        CheckButtonsInteractable();

        foreach (var card in cards)
        {
            card.SetActive(false);
        }
    }

    private void FillUnlockableWordPool()
    {
        int maxID = databaseManager.WordDao.GetAllIDCount(currentWordSetIndex);
        
        while (unlockableWordPool.Count < POOL_SIZE && lastCheckedWordID < maxID)
        {
            lastCheckedWordID++;
            if (!unlockedWordIDs.Contains(lastCheckedWordID))
            {
                unlockableWordPool.Add(lastCheckedWordID);
            }
        }
    }

    private void SetupButtons()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].OnClick.RemoveAllListeners();
            buttons[i].OnClick.AddListener(() => OnUnlockButtonClick(buttonIndex));
        }
    }

    private void OnUnlockButtonClick(int buttonIndex)
    {
        int faithValue = needFaith[buttonIndex];
        int knowledgeValue = getKnowledge[buttonIndex];

        if (dataManager.res.Get(GameResource.Faith) < faithValue)
        {
            print("faithが足りません");
            return;
        }

        if (unlockableWordPool.Count == 0)
        {
            print("すべての単語を開放しました");
            // Double check by trying to fill the pool again
            FillUnlockableWordPool();
            if(unlockableWordPool.Count == 0) {
                 print("本当にすべての単語を開放しました");
                 CheckButtonsInteractable();
                 return;
            }
        }

        dataManager.res.Add(GameResource.Faith, -faithValue);
        dataManager.res.Add(GameResource.Knowledge, knowledgeValue);
        
        UnlockAndDisplayWords(knowledgeValue);

        CheckButtonsInteractable();
        setWord.UpdateButtons();
    }

    private void UnlockAndDisplayWords(int count)
    {
        foreach (var card in cards)
        {
            card.SetActive(false);
        }

        int wordsToUnlockCount = Mathf.Min(count, unlockableWordPool.Count);

        for (int i = 0; i < wordsToUnlockCount; i++)
        {
            int wordID = TakeRandomWordFromPool();
            if (wordID == -1) continue;

            databaseManager.UnlockDao.AddUnlockID(currentWordSetIndex, wordID);
            unlockedWordIDs.Add(wordID); // Keep track of newly unlocked words
            DisplayWordOnCard(i, wordID);
        }
        
        // After unlocking, try to refill the pool for the next click
        FillUnlockableWordPool();
    }

    private int TakeRandomWordFromPool()
    {
        if (unlockableWordPool.Count == 0)
        {
            return -1;
        }
        int randomIndex = Random.Range(0, unlockableWordPool.Count);
        int wordID = unlockableWordPool[randomIndex];
        unlockableWordPool.RemoveAt(randomIndex);
        return wordID;
    }

    private void DisplayWordOnCard(int cardIndex, int wordID)
    {
        if (cardIndex >= cards.Length) return;

        var wordData = databaseManager.WordDao.GetSQLiteWord(currentWordSetIndex, wordID);
        if (wordData == null) return;

        TextMeshProUGUI from = cards[cardIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI to = cards[cardIndex].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI id = cards[cardIndex].transform.GetChild(4).GetComponent<TextMeshProUGUI>();

        from.text = wordData["Vocab"].ToString();
        to.text = wordData["Exp"].ToString();
        id.text = wordData["ID"].ToString();

        cards[cardIndex].SetActive(true);
    }

    public void CheckButtonsInteractable()
    {
        int totalCount = databaseManager.WordDao.GetAllIDCount(currentWordSetIndex);

        for (int i = 0; i < buttons.Length; i++)
        {
            // We check against the pool because that's what's available right now.
            bool canUnlockMore = unlockedWordIDs.Count + getKnowledge[i] <= totalCount;
            bool hasEnoughFaith = dataManager.res.Get(GameResource.Faith) >= needFaith[i];
            
            bool isInteractable = hasEnoughFaith && canUnlockMore;

            buttons[i].interactable = isInteractable;
            buttons[i].gameObject.GetComponentInChildren<Image>().color = isInteractable ? CONSTANTS.BUTTONCOLOR : Color.gray;
        }
    }
}
