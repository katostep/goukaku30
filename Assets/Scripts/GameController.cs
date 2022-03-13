using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameController : SingletonMonoBehaviour<GameController>
{
    public Text stageText;
    public Text maxStageText;
    public Text[] stopWatchText;
    public Text pressSpaceText;
    public RankingPanel rankingPanel;
    public Button rankingCloseButton;

    public Stage stagePrefab;
    
    private int m_CurrentStageId = 0;
    private Game_State m_State = Game_State.waiting_space;

    private List<Stage> m_StageList = new List<Stage>();

    private void Awake()
    {
        for(int i = 0; i < Settings.Instance.Stage_Count; i++)
        {
            var stage = Instantiate(stagePrefab) as Stage;
            m_StageList.Add(stage);
            stage.Init(i);
            stage.OnClearStage += Stage_OnClearStage;
            stage.gameObject.SetActive(false);
        }
        stageText.text = "1";
        maxStageText.text = Settings.Instance.Stage_Count.ToString();
        rankingCloseButton.onClick.AddListener(rankingClose_Clicked);
    }

    private void rankingClose_Clicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    private void Start()
    {
        Destroy(stagePrefab.gameObject);
        rankingPanel.gameObject.SetActive(false);
    }

    private float m_Time = 0f;

    private void Update()
    {
        if(m_State == Game_State.waiting_space && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0)))
        {
            m_State = Game_State.gaming;
            pressSpaceText.gameObject.SetActive(false);
            var currentStage = m_StageList[m_CurrentStageId];
            currentStage.gameObject.SetActive(true);
            currentStage.SetEnable(true);
        }

        if (m_State == Game_State.gaming)
        {
            m_Time += Time.deltaTime;

            TimeSpan ts = TimeSpan.FromSeconds((double)m_Time);
            string timeText = ts.ToString(@"mm\:ss\.f");
            
            int c = 0;
            foreach(char t in timeText)
            {
                stopWatchText[c].text = t.ToString();
                c++;
            }

            if(ts.Hours >= 1)
            {
                //GameOver
                SceneManager.LoadScene("GameScene");
            }
        }

        if(m_State == Game_State.ranking)
        {
            //閉じるボタン押したらおわり
        }

    }

    private void Reflesh()
    {
        for(int i = 0; i < m_StageList.Count; i++)
        {
            var stage = m_StageList[i];
            stage.Reflesh();
            
            if(i == 0)
            {
                stage.gameObject.SetActive(true);
            }
            else
            {
                stage.gameObject.SetActive(false);
            }
        }
        m_CurrentStageId = 0;
    }

    private void Stage_OnClearStage(int id)
    {

        var currentStage = m_StageList[m_CurrentStageId];
        //触れなくする
        currentStage.SetEnable(false);
        //一番手前に表示
        currentStage.gameObject.layer = LayerMask.NameToLayer("UI");
        m_CurrentStageId++;

        if (m_StageList.Count <= m_CurrentStageId)
        {
            //終了
            rankingPanel.gameObject.SetActive(true);
            currentStage.gameObject.SetActive(false);
            
            this.rankingPanel.SetRanking(m_Time);
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlaySE(SE.ranking);
            m_State = Game_State.ranking;
            return;
        }
        var next = m_StageList[m_CurrentStageId];
        next.gameObject.SetActive(true);
        next.transform.localScale = Vector3.one * 0.3f;

        float duration = 1f;
        Sequence sequence = DOTween.Sequence();
        //合格がめくれるのを待つ
        sequence.AppendCallback(() =>
        {
            //currentStage.transform.DOScale(1.4f, 0.8f);
            currentStage.PlayCollapse(1f);
        });
        sequence.AppendInterval(0.8f);
        sequence.Append(currentStage.transform.DOMove(new Vector3(10f, -15f, 0f), duration));
        sequence.Join(currentStage.transform.DORotate(new Vector3(0, 0, 30), duration));
        sequence.Join(next.transform.DOScale(1f, duration));
        sequence.OnComplete(() =>
        {
            currentStage.gameObject.SetActive(false);
            stageText.text = (m_CurrentStageId + 1).ToString();

        });
        sequence.Play();
    }


}

public struct FutoResult
{
    public int distance;
    public FutoResult(int dis)
    {
        this.distance = dis;
    }
}

public enum Game_State
{
    waiting_space = 0,
    gaming,
    ranking
}