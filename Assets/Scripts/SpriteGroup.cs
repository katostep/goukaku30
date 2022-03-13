using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteGroup : MonoBehaviour
{
    List<SpriteRenderer> m_Sprites = new List<SpriteRenderer>();

    private void Awake()
    {
        var sprites = transform.GetComponentsInChildren<SpriteRenderer>();
        if(sprites != null && sprites.Length > 0)
        {
            m_Sprites.AddRange(sprites);
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
    public void SetOrders(int order)
    {
        m_Sprites.ForEach(a => a.sortingOrder = order);
    }
    public void Fadeout(float duration)
    {
        m_Sprites.ForEach(a => { a.DOFade(0f, duration);a.transform.DOLocalMoveY(-30f, duration); });
    }
    public void Fadein(float duration,bool animation = true)
    {
        foreach (var sp in m_Sprites)
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b,1f);
            sp.transform.DOKill();
            sp.transform.localPosition = Vector3.zero;
        }
        if (animation == false)
        {

        }
    }
    public void Reflesh()
    {
        foreach (var sp in m_Sprites)
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, 1f);
            sp.transform.localPosition = Vector3.zero;
        }
    }
    public void SetAllAlpha(float alpha)
    {
        foreach (var sp in m_Sprites)
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, alpha);
        }
    }
}
