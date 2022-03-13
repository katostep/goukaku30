using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class RankingPanel : MonoBehaviour
{
    /*
     * PlayFabのランキングは降順にできないため、100000からタイム*100を引いた数字をスコアとする
     * なので値をとってくる時は、100000から引いた後100で割る
     * */

    private const int Magic_Number = 100000;

    public Text[] userNameTexts;
    public Text[] scoreTexts;
    public Text highScoreText;
    public Text scoreText;
    public InputField usernameField;
    public Button submitButton;

    bool m_Inited = false;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (m_Inited) return;
        m_Inited = true;
        submitButton.interactable = false;
        submitButton.onClick.AddListener(SubmitButton_Clicked);
        usernameField.onValueChanged.AddListener(Username_ValueChanged);

        Login();
    }

    public void Login()
    {
        string id = PlayerPrefs.GetString("ID_Key", null);
        bool shouldCreateAccount = string.IsNullOrEmpty(id);
        if (shouldCreateAccount)
        {
            id = CreateNewId();
            
        }
        
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest
            {
                CustomId = id,
                CreateAccount = shouldCreateAccount
            },
            result => {
                // 成功時の処理
                Debug.Log("Login successfully");
                PlayerPrefs.SetString("ID_Key", id);
            },
            error => {
            // 失敗時の処理
            Debug.LogError(error.GenerateErrorReport());
            }
        );


    }

    // 一意のIdを生成する
    string CreateNewId()
    {
        return System.Guid.NewGuid().ToString();
    }
    private float m_Time;
    private void SubmitButton_Clicked()
    {
        string input = usernameField.text;
        if (string.IsNullOrEmpty(input)) return;
        PlayerPrefs.SetString("Name_Key", input);

        submitButton.interactable = false;

        SetUserName(usernameField.text);
        int score = Magic_Number - Mathf.RoundToInt(m_Time * 100);
        SendPlayScore(score);
    }

    private void Username_ValueChanged(string value)
    {
        if (m_UpdateHighScoreFlag == false) return;

        if (value.Length > 2)
        {
            submitButton.interactable = true;
        }

        if(value.Length <= 2)
        {
            submitButton.interactable = false;
        }
    }
    private bool m_UpdateHighScoreFlag = false;

    public void SetRanking(float resultTime)
    {
        m_Time = resultTime;

        float highScore = PlayerPrefs.GetFloat("High_Score_Key", Magic_Number);
        m_UpdateHighScoreFlag = highScore > resultTime;
        usernameField.interactable = m_UpdateHighScoreFlag;
        submitButton.interactable = m_UpdateHighScoreFlag;
        //ハイスコアより低かったら登録
        if (m_UpdateHighScoreFlag)
        {
            PlayerPrefs.SetFloat("High_Score_Key", resultTime);
            highScore = resultTime;

        }
        string score = Util.GetTimeScore(resultTime);
        string highSt = Util.GetTimeScore(highScore);
        this.highScoreText.text = highSt;
        this.scoreText.text = score;
        this.UpdateRanking();
        string name = PlayerPrefs.GetString("Name_Key", "");
        this.usernameField.text = name;


        int result = Mathf.RoundToInt(resultTime * 100);
        if(result > Magic_Number)
        {
            submitButton.interactable = false;
            usernameField.interactable = false;
        }
    }

    private void SetUserName(string userName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnSuccess, OnError);

        void OnSuccess(UpdateUserTitleDisplayNameResult result)
        {
            Debug.Log("success!Name");
        }

        void OnError(PlayFabError error)
        {
            Debug.Log($"{error.Error}");
        }
    }

    private void SendPlayScore(int score)
    {
        var statisticUpdate = new StatisticUpdate
        {
            // 統計情報名を指定します。
            StatisticName = "Goukaku30",
            Value = score,
        };
        
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                statisticUpdate
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnSuccess, OnError);

        void OnSuccess(UpdatePlayerStatisticsResult result)
        {
            submitButton.interactable = false;
            usernameField.interactable = false;
            
            Invoke("InvokeRanking", 1f);
            Debug.Log("Send Success");
        }

        void OnError(PlayFabError error)
        {
            Debug.Log($"{error.Error}");
            submitButton.interactable = true;
        }
    }

    private void InvokeRanking()
    {
        this.UpdateRanking();
    }
    private void UpdateRanking()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Goukaku30", 
            StartPosition = 0, 
            MaxResultsCount = 5 
        };

        try
        {
            PlayFabClientAPI.GetLeaderboard(request, OnSuccess, OnError);
        }
        catch { }
        

        void OnSuccess(GetLeaderboardResult leaderboardResult)
        {
            foreach (var item in leaderboardResult.Leaderboard)
            {
                int index = item.Position;
                if (index > userNameTexts.Length - 1) break;

                userNameTexts[index].text = item.DisplayName;
                int val = item.StatValue;
                float score = (float)(Magic_Number - val) / 100f;
                scoreTexts[index].text = Util.GetTimeScore(score);
            }
        }

        void OnError(PlayFabError error)
        {
            Debug.Log($"{error.Error}");
        }
    }
}
