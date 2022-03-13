using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Futo : MonoBehaviour
{
    public SpriteRenderer closeFuto;
    public SpriteRenderer[] openFuto;
    public SpriteGroup futoGroup;
    public ResultPaper resultPaper;
    [HideInInspector]
    public Vector2Int position;


    public delegate void FutoDelegate(Vector2Int pos);
    public event FutoDelegate OnTapped = delegate { };

    private bool m_IsOpen = false;
    private void Awake()
    {
        OpenFuto(false);
    }

    private void OpenFuto(bool open)
    {
        m_IsOpen = open;
        closeFuto.gameObject.SetActive(!open);
        foreach (var futo in openFuto)
        {
            futo.gameObject.SetActive(open);
        }
    }

    public void TapFuto()
    {
        if(m_IsOpen == false)
        {
            SoundManager.Instance.PlaySE(SE.open_futo);
        }
        
        OpenFuto(true);
        futoGroup.Fadeout(1f);
        //futoGroup.GetComponent<Rigidbody2D>().gravityScale = 1f;
        //GameController.Instance.Tapped(position);
        OnTapped(position);
    }

    //正解
    public void Correct(float duration)
    {
        SoundManager.Instance.PlaySE(SE.goukaku);
        foreach (var futo in openFuto)
        {
            futo.gameObject.SetActive(false);
        }
        resultPaper.GetComponent<SpriteRenderer>().sortingOrder = 100;
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.4f, duration));
        seq.Join(resultPaper.Render.DOFade(0f, duration));
        //seq.Join(transform.DOMove(Vector3.zero, duration));
        seq.Play();
    }
    //崩壊
    public void Collapse(float duration)
    {
        float degreeX = Random.Range(-180f, 180f);
        float degreeY = Random.Range(-180f, 180f);
        float degreeZ = Random.Range(-180f, 180f);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOLocalMoveY(-10f, duration));
        seq.Join(transform.DORotate(new Vector3(degreeX, degreeY, degreeZ), duration));
        seq.Play();
    }
    
    public void Reflesh()
    {
        futoGroup.Reflesh();
        OpenFuto(false);
       // futoGroup.GetComponent<Rigidbody2D>().gravityScale = 0f;
        //futoGroup.transform.localPosition = Vector3.zero;
        
    }

    public void SetDistance(int distance)
    {
        if(distance == 0)//0は合格
        {
            resultPaper.SetGokaku();
        }
        else
        {
            resultPaper.SetDistance(distance);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
