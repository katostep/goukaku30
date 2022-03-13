using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPaper : MonoBehaviour
{

    public Sprite goukaku;
    [SerializeField,Tooltip("index0が1の画像")]
    public Sprite[] distanceSprites;
    private SpriteRenderer m_SpriteRender;
    private SpriteRenderer _SpriteRender {
        get {
            if(m_SpriteRender == null) {
                m_SpriteRender = GetComponent<SpriteRenderer>();
            }
            return m_SpriteRender;
        }
    }
    public SpriteRenderer Render
    {
        get { return _SpriteRender; }
    }
    public void SetGokaku()
    {
        _SpriteRender.sprite = goukaku;
    }

    public void SetDistance(int distance)
    {
        int index = distance - 1;
        //index = Mathf.Clamp(index, 0, 1);
        _SpriteRender.sprite = this.distanceSprites[index];
    }
}
