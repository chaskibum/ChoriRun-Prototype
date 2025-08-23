using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Dan.Main;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] string nameToAdd;
    [SerializeField] int ScoreToAdd;
    [SerializeField] string Scores;
    [SerializeField] string Names;
    [SerializeField] List<int> ScoreList = new();
    [SerializeField] List<string> NameLists = new();
    [SerializeField] List<TMP_Text> NamesListText;

    [SerializeField] private List<TextMeshProUGUI> names;
    [SerializeField] private List<TextMeshProUGUI> scores;

    private string publicLeaderboardKey = "b4b5364321b1d6af6c914a51c2d352f3bb04ca52ef1c2193625ecc6a0f01cb79";

    public void GetLeaderboard()
    {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, (msg) =>
        {
            int loopLength = (msg.Length < names.Count ? msg.Length : names.Count);
            for (int i = 0; i < loopLength; i++)
            {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        });
    }

    public void SetLeaderboardEntry(string username, int score)
    {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, (_) => { GetLeaderboard(); });
        LeaderboardCreator.ResetPlayer();
    }

    public void SubmitScore()
    {
        print("...");
        SetLeaderboardEntry(nameToAdd, ScoreToAdd);
    }

    void Start()
    {
        /*Scores = PlayerPrefs.GetString("ScoreList");
        Names = PlayerPrefs.GetString("NamesList");
        SetLeaderboard();
        UpdateTextUI();*/

        GetLeaderboard();
    }

    /*
    void SetLeaderboard()
    {
        if (Scores != "")
        {
            List<string> ScoreListString = Scores.Split(",").ToList();
            foreach (string Score in ScoreListString)
            {
                ScoreList.Add(int.Parse(Score));
            }
        }
        if (Names != "")
        {
            List<string> NameList = Names.Split(",").ToList();
            foreach (string Name in NameList)
            {
                NameLists.Add(Name);
            }
        }
    }*/

    /*public void UpdateTextUI()
    {
        for (int i = 0; i < ScoreList.Count; i++)
        {
            string name = "" + NameLists[i] + " :";
            string score = " " + ScoreList[i] + " ";
            NamesListText[i].text = name + score;
        }
    }

    public void AddNameAndScoreToLeaderboard(string Name, int Score)
    {
        bool isTopFull = ScoreList.Count >= 10;
        for (int i = 0; i < ScoreList.Count; i++)
        {
            if (Score >= ScoreList[i])
            {
                ScoreList.Insert(i, Score);
                NameLists.Insert(i, Name);
                break;
            }
        }
        if (ScoreList.Count == 0)
        {
            ScoreList.Insert(0, Score);
            NameLists.Insert(0, Name);
        }
        else if (Score < ScoreList[ScoreList.Count - 1])
        {
            ScoreList.Insert(ScoreList.Count, Score);
            NameLists.Insert(NameLists.Count, Name);
        }
        if (isTopFull)
        {
            ScoreList.RemoveAt(ScoreList.Count - 1);
            NameLists.RemoveAt(NameLists.Count - 1);
        }

        UpdateTextUI();*/
    // SaveLeadearboardsValues();
    // SetLeaderboardEntry(Name, Score);
    // }

// Update is called once per frame
    /*
     void Update()
     {
         if (Input.GetKeyDown(KeyCode.H))
         {
             AddNameAndScoreToLeaderboard(ScoreToAdd, nametoAdd);
         }
         if (Input.GetKeyDown(KeyCode.J))
         {
             SaveLeadearboardsValues();
         }
         if (Input.GetKeyDown(KeyCode.LeftAlt))
         {
             PlayerPrefs.DeleteAll();
         }
    }
    */

    /*void SaveLeadearboardsValues()
    {
        Names = string.Join(",", NameLists);
        Scores = string.Join(",", ScoreList);
        PlayerPrefs.SetString("NamesList", Names);
        PlayerPrefs.SetString("ScoreList", Scores);
        PlayerPrefs.Save();
    }*/
}