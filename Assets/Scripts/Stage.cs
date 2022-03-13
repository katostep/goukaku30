using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stage : MonoBehaviour
{
    private const float Tile_Interval_X = 1.0f;
    private const float Tile_Interval_Y = 1.5f;

    public GameObject tilePrefab;
    public GameObject touchProtect;
    private List<Futo> m_FutoList = new List<Futo>();

    private Vector2Int m_AnswerPos;

    private int m_Id = 0;

    public delegate void StageDelegate(int id);
    public event StageDelegate OnClearStage = delegate { };

    private void Awake()
    {
        
        
    }

    private void Start()
    {
        Destroy(tilePrefab.gameObject);
    }
    public void Init(int id)
    {
        m_Id = id;
        CreateTiles();
    }

    public void SetEnable(bool enable)
    {
        //Debug.Log("Enable");
        touchProtect.SetActive(!enable);
    }

    //正解以外を崩壊させる
    public void PlayCollapse(float duration)
    {
        int Tile_Wid = Settings.Instance.Tile_Wid;
        int Tile_Hei = Settings.Instance.Tile_Hei;

        for (int y = 0; y < Tile_Hei; y++)
        {
            for (int x = 0; x < Tile_Wid; x++)
            {
                Futo futo = m_FutoList[x + (y * Tile_Wid)];

                if (m_AnswerPos.x == x && m_AnswerPos.y == y)
                {
                    futo.Correct(duration);
                }
                else
                {
                    futo.Collapse(duration);
                }
            }
        }
    }

    private void CreateTiles()
    {
        int Tile_Wid = Settings.Instance.Tile_Wid;
        int Tile_Hei = Settings.Instance.Tile_Hei;

        for (int y = 0; y < Tile_Hei; y++)
        {
            for (int x = 0; x < Tile_Wid; x++)
            {
                //this.tilePrefab.GetComponent<BoxCollider2D>().enabled = true;
                var obj = Instantiate(this.tilePrefab, transform, false);
                var futo = obj.GetComponent<Futo>();
                m_FutoList.Add(futo);
                //Field_Type type = (Field_Type)fieldMap.fieldTypes[x + (y * width)];

                futo.position = new Vector2Int(x, y);
                var half = new Vector2(((float)Tile_Wid * 0.5f - 0.5f) * Tile_Interval_X, ((float)Tile_Hei * 0.5f - 0.5f) * Tile_Interval_Y);
                futo.transform.localPosition = new Vector2(x * Tile_Interval_X, y * Tile_Interval_Y) - half;
                futo.OnTapped += Futo_OnTapped;
               //futo.enabled = true;
            }
        }
        RefleshTiles();
        //なんかInstantiateしてすぐDestroyするのはダメっぽい
        //Destroy(tilePrefab.gameObject);
        //tilePrefab.gameObject.SetActive(false);
    }

    public void Reflesh()
    {
        transform.localPosition = Vector3.zero;
        transform.rotation = Quaternion.identity;

        int Tile_Wid = Settings.Instance.Tile_Wid;
        int Tile_Hei = Settings.Instance.Tile_Hei;

        for (int y = 0; y < Tile_Hei; y++)
        {
            for (int x = 0; x < Tile_Wid; x++)
            {
                Futo futo = m_FutoList[x + (y * Tile_Wid)];
                futo.Reflesh();
            }
        }
        RefleshTiles();
    }

    private void Futo_OnTapped(Vector2Int pos)
    {
        int dist = GetDistance(pos);
        if(dist == 0)
        {
            OnClearStage(m_Id);
        }
    }



    //抽選して、合格と距離をセット
    private void RefleshTiles()
    {
        int Tile_Wid = Settings.Instance.Tile_Wid;
        int Tile_Hei = Settings.Instance.Tile_Hei;

        m_AnswerPos = Lot();
        for (int y = 0; y < Tile_Hei; y++)
        {
            for (int x = 0; x < Tile_Wid; x++)
            {
                Futo futo = m_FutoList[x + (y * Tile_Wid)];
                //futo.futoGroup.transform.localPosition = Vector3.zero;
                futo.SetDistance(GetDistance(futo.position));
            }
        }
    }

    private int GetDistance(Vector2Int pos)
    {
        int xDist = Mathf.Abs(m_AnswerPos.x - pos.x);
        int yDist = Mathf.Abs(m_AnswerPos.y - pos.y);
        return xDist + yDist;
    }

    //抽選
    private Vector2Int Lot()
    {
        int Tile_Wid = Settings.Instance.Tile_Wid;
        int Tile_Hei = Settings.Instance.Tile_Hei;

        int x = UnityEngine.Random.Range(0, Tile_Wid);
        int y = UnityEngine.Random.Range(0, Tile_Hei);
        return new Vector2Int(x, y);
    }
    /*
    private GameObject GetTouchProtect()
    {
        var touchProtect = transform.Find("TouchProtect");
        return touchProtect.gameObject;
    }*/
}
