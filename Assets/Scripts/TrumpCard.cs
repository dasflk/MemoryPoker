using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrumpCard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_text;
    [SerializeField]
    private Image m_pattern_img;
    [SerializeField]
    private TextMeshProUGUI m_back_number;
    [SerializeField]
    private GameObject m_ban;

    private Sprite m_back_sprite;
    private Sprite m_front_sprite;
    private bool m_is_front;
    private bool m_is_ban;

    private Image m_back_img;
    private Animator m_animator;
    private Canvas m_overlay_canvas;

    private void Awake()
    {
        m_is_front = true;
        m_is_ban = false;
        //m_front_sprite = gameObject.GetComponent<Image>().sprite;
        m_animator = gameObject.GetComponent<Animator>();
        m_back_img = gameObject.GetComponent<Image>();
    }

    public void initCard(int index, int num, Sprite pattern, Sprite back_sprite, Sprite img)
    {
        m_back_number.text = index.ToString();
        m_text.text = (num == 1) ? "A" : num.ToString();
        m_pattern_img.sprite = pattern;
        m_back_sprite = back_sprite;
        m_back_img.sprite = img;
        m_front_sprite = img;
        m_ban.SetActive(false);

        if (m_overlay_canvas != null)
        {
            Destroy(m_overlay_canvas);
            m_overlay_canvas = null;
        }
    }

    public void animateFlip()
    {
        Debug.Log("animate flip !");
        m_animator.SetTrigger("flip");
    }

    public void setBan()
    {
        m_is_ban = true;
    }

    public Sprite getFrontSprite()
    {
        return m_front_sprite;
    }

    public void animateSelect(int sort)
    {
        // 2) 렌더 우선순위 승격 (자기 자신에 Canvas 붙이기)
        if (m_overlay_canvas == null)
            m_overlay_canvas = gameObject.AddComponent<Canvas>();

        m_overlay_canvas.overrideSorting = true;

        // 부모 Canvas 기준으로 더 높은 sortingOrder 부여
        m_overlay_canvas.sortingOrder = sort;

        Debug.Log("animate ban !");
        m_animator.SetTrigger("ban");
    }

    public void animateOpen(int sort)
    {
        // 2) 렌더 우선순위 승격 (자기 자신에 Canvas 붙이기)
        if (m_overlay_canvas == null)
            m_overlay_canvas = gameObject.AddComponent<Canvas>();

        m_overlay_canvas.overrideSorting = true;

        // 부모 Canvas 기준으로 더 높은 sortingOrder 부여
        m_overlay_canvas.sortingOrder = sort;

        Debug.Log("animate open !");
        m_animator.SetTrigger("open");
    }

    public bool isFront()
    {
        return m_is_front;
    }

    public void banCard()
    {
        m_ban.transform.parent.gameObject.GetComponent<HorizontalLayoutGroup>().enabled = false;

        RectTransform rectA = m_back_number.GetComponent<RectTransform>();
        RectTransform rectB = m_ban.gameObject.GetComponent<RectTransform>();

        rectB.anchoredPosition = rectA.anchoredPosition;

        rectB.sizeDelta = rectA.sizeDelta;

        rectB.anchorMin = rectA.anchorMin;
        rectB.anchorMax = rectA.anchorMax;
        rectB.pivot = rectA.pivot;

        m_ban.SetActive(true);
    }

    public void OnFlip()
    {
        m_is_front = !m_is_front;
        if (m_is_front == true)
        {
            //m_text.gameObject.transform.parent.gameObject.SetActive(true);
            //m_pattern_img.gameObject.transform.parent.gameObject.SetActive(true);

            m_back_number.gameObject.transform.parent.gameObject.SetActive(false);
            m_back_img.sprite = m_front_sprite;
        }
        else
        {
            //m_text.gameObject.transform.parent.gameObject.SetActive(false);
            //m_pattern_img.transform.parent.gameObject.gameObject.SetActive(false);

            m_back_number.gameObject.transform.parent.gameObject.SetActive(true);
            m_back_img.sprite = m_back_sprite;
        }
    }

    public void OnBan()
    {
        Debug.Log("card OnBan");
        if (m_is_ban == true)
        {
            //m_back_img.color = new Color32(70, 70, 70, 255);
            banCard();
            if (m_overlay_canvas != null)
                m_overlay_canvas.sortingOrder = 0;
        }
    }

    public void OnOpen()
    {
        m_is_front = true;
        if (m_overlay_canvas == null)
            m_overlay_canvas.sortingOrder = 0;
    }
}
