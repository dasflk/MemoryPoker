using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_name_input;
    [SerializeField]
    private TMP_InputField m_deck_input;
    [SerializeField]
    private TextMeshProUGUI m_deck_input_text;
    [SerializeField]
    private GameObject m_deck_edit_btn;
    [SerializeField]
    private GameObject m_deck_ok_btn;
    [SerializeField]
    private GameObject m_result_panel;
    [SerializeField]
    private GameObject m_character_panel;
    [SerializeField]
    private Image[] m_result_image;

    public void init()
	{
        m_deck_input.text = "";
        m_deck_input_text.text = "";
        setBtnState(true);
        OnDeckEdit();
        showResult(false);
    }

	public string getDeckString()
    {
        return m_deck_input_text.text;
    }

    public string getName()
    {
        return m_name_input.text;
    }

    public void showResult(bool is_true)
    {
        m_character_panel.SetActive(!is_true);
        m_result_panel.SetActive(is_true);
    }

    public void setResultSprite(int index, Sprite sprite)
    {
        m_result_image[index].sprite = sprite;
    }

    public void setBtnState(bool is_enable)
    {
        m_deck_ok_btn.GetComponent<Button>().enabled = is_enable;
        m_deck_edit_btn.GetComponent<Button>().enabled = is_enable;
    }

    public void OnDeckEdit()
    {
        m_deck_input_text.gameObject.SetActive(false);
        m_deck_edit_btn.SetActive(false);

        m_deck_input.gameObject.SetActive(true);
        m_deck_ok_btn.SetActive(true);
    }
    public void OnDeckOk()
    {
        if (m_deck_input.text != "" ||
            string.IsNullOrWhiteSpace(m_deck_input.text) == false)
        {
            if (IsValidDeck(m_deck_input.text) == false)
                return;
        }

        m_deck_input_text.text = m_deck_input.text;

        m_deck_input_text.gameObject.SetActive(true);
        m_deck_edit_btn.SetActive(true);

        m_deck_input.gameObject.SetActive(false);
        m_deck_ok_btn.SetActive(false);
    }

    public bool IsValidDeck(string deck)
    {
        if (string.IsNullOrWhiteSpace(deck))
            return false;

        // 1. 쉼표(,)로 분리
        string[] parts = deck.Split(',');

        // 최소 1개 이상 숫자가 있어야 함
        if (parts.Length < 1 || parts.Length > 3)
            return false;

        foreach (var part in parts)
        {
            // 2. 공백 제거
            string trimmed = part.Trim();

            // 3. 숫자인지 확인
            if (!int.TryParse(trimmed, out int num))
                return false;

            // 4. 범위 확인 (1 ~ 15)
            if (num < 1 || num > 15)
                return false;
        }

        return true;
    }
}
