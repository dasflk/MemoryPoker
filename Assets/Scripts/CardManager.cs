using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField]
    private int m_ban_num; // 밴 카드 수
    [SerializeField]
    private float m_result_flip_term;
    [SerializeField]
    private float m_result_player_term;
    [SerializeField]
    private string[] m_fail_msg;

    // UI
    [SerializeField]
    private GameObject m_board;
    [SerializeField]
    private GameObject m_start_btn;
    [SerializeField]
    private GameObject m_ban_btn;
    [SerializeField]
    private GameObject m_show_dwon_btn;
    [SerializeField]
    private GameObject m_regame_btn;

    // 카드
    [SerializeField]
    private GameObject m_card_prefab;
    [SerializeField]
    private List<Sprite> m_card_sprites;
    [SerializeField]
    private Sprite m_card_back_sprite;
    [SerializeField]
    private int m_card_num; // 카드 수
    [SerializeField]
    private Sprite[] m_card_pack1;
    [SerializeField]
    private Sprite[] m_card_pack2;
    [SerializeField]
    private Sprite[] m_card_pack3;
    [SerializeField]
    private Sprite[] m_card_pack4;

    // 타이머
    [SerializeField]
    private TextMeshProUGUI m_timer_text;
    [SerializeField]
    private int m_flip_timer;

    // 플레이어
    [SerializeField]
    private List<Player> m_players;

    // 메시지
    [SerializeField]
    private TextMeshProUGUI m_message_text;
    [SerializeField]
    float m_display_sec;
    [SerializeField]
    float m_fade_sec;

    private Coroutine m_message_coroutine;
    private Coroutine m_timer_coroutine;
    private List<TrumpCard> m_cards;
    private Dictionary<Player, List<int>> m_player_decks;
    private List<int> m_ban_card_index;
    private int m_canvas_sort;

    // Start is called before the first frame update
    void Start()
    {
        m_cards = new();
        m_player_decks = new();
        m_ban_card_index = new();
        m_canvas_sort = 1;
    }

	private void init()
	{
        m_ban_btn.SetActive(false);
        m_show_dwon_btn.SetActive(false);
        m_canvas_sort = 1;
    }

	public void OnClickToBan()
    {
        Debug.Log("click ban !");
        m_ban_btn.SetActive(false);

        if (m_ban_card_index.Count != 0)
            return;

        HashSet<int> rand_nums = new();
        while (rand_nums.Count < m_ban_num)
        {
            int rand_num = Random.Range(0, m_card_num);
            rand_nums.Add(rand_num);
        }

        m_ban_card_index = rand_nums.ToList();

        StartCoroutine(BanRoutine());
    }

    public void OnStart()
    {
        m_start_btn.SetActive(false);

        // 타이머
        m_timer_text.gameObject.SetActive(true);
        StartTimer(m_flip_timer);

        // 중복체크
        HashSet<(int, int)> duplicate = new();
        // 1 ~ 36 중에 숫자 랜덤 뽑기
        while (duplicate.Count < m_card_num)
        {
            // 카드 번호 A ~ 9
            int rand_num = Random.Range(1, 10);
            // 카드 문양 네 종류 (하트, 다이아, 클로버, 스페이드)
            int rand_pattern = Random.Range(0, 4);

            duplicate.Add((rand_num, rand_pattern));
        }

        int index = 0;
		foreach (var rand_value in duplicate)
		{
            TrumpCard card = Instantiate(m_card_prefab, m_board.transform).GetComponent<TrumpCard>();
            Debug.Log("rand : " + rand_value.Item1.ToString() + "pattern_num : " + rand_value.Item2.ToString());

            Sprite sprite = null;
            if (rand_value.Item2 == 0)
                sprite = m_card_pack1[rand_value.Item1 - 1];
            else if (rand_value.Item2 == 1)
                sprite = m_card_pack2[rand_value.Item1 - 1];
            else if (rand_value.Item2 == 2)
                sprite = m_card_pack3[rand_value.Item1 - 1];
            else if (rand_value.Item2 == 3)
                sprite = m_card_pack4[rand_value.Item1 - 1];

            card.initCard(index + 1, rand_value.Item1, m_card_sprites[rand_value.Item2], m_card_back_sprite, sprite);
            m_cards.Add(card);
              
            index++;
		}
    }

    public void flipAllCards()
    {
		for (int i = 0; i < m_cards.Count; i++)
		{
            m_cards[i].animateFlip();
        }
    }

    public void OnShowDown()
    {
        Debug.Log("click show_down !");
        bool is_fail = false;
        bool empty = true;
        string fail_message = "";
        
        for (int i = 0; i < m_players.Count; i++)
		{
            Player cur_player = m_players[i];
            string input_deck = cur_player.getDeckString();

            if (input_deck == "")
                continue;

            string[] input_decks = input_deck.Split(',')
                     .Select(s => s.Trim())
                     .ToArray();

            // 덱 3가지 선택 체크
            if (input_decks.Length != 3)
            {
                fail_message = m_fail_msg[0] /*+"플레이어"*/ + (i + 1).ToString() + m_fail_msg[01]/*" 입력 형식 오류(N,N,N)"*/;
                is_fail = true;
                break;
            }

            // 덱 정수 변환
            List<int> decks = new List<int>();
            for (int j = 0; j < input_decks.Length; j++)
			{
                if (int.TryParse(input_decks[j], out int result))
                {
                    Debug.Log($"변환 성공: {result}");

                    if (result < 1 || result > m_card_num)
                    {
                        fail_message = m_fail_msg[2] /*"1 ~ "*/ + m_card_num.ToString() + m_fail_msg[3]/*"번의 카드만 입력 가능합니다"*/;
                        is_fail = true;
                        break;
                    }

                    if (m_ban_card_index.Contains(result - 1))
                    {
                        fail_message = result.ToString() + m_fail_msg[4]/*"번은 금지된 카드입니다"*/;
                        is_fail = true;
                        break;
                    }

                    empty = false;
                    decks.Add(result);
                }
                else
                {
                    Debug.Log("변환 실패");
                    fail_message = m_fail_msg[5]/*"잘못된 형식 입니다(숫자,숫자,숫자)"*/;
                    is_fail = true;
                    return;
                }
            }

            if (is_fail)
                break;

            m_player_decks.Add(cur_player, decks);
        }

        if (empty == true && is_fail == false)
        {
            fail_message = m_fail_msg[6]/*"카드 번호를 입력한 플레이어가 없습니다"*/;
            is_fail = true;
        }

        if (is_fail)
        {
            ShowMessage(fail_message);
            m_player_decks.Clear();
            return;
        }

		foreach (var player in m_players)
		{
            player.setBtnState(false);
        }

        m_show_dwon_btn.SetActive(false);
        StartCoroutine(openRoutine());
    }

    public void OnRegame()
    {
        m_player_decks.Clear();
        m_ban_card_index.Clear();
        foreach (var item in m_cards)
		{
            Destroy(item.gameObject);
        }
        m_cards.Clear();
        m_canvas_sort = 1;

		foreach (var player in m_players)
		{
            player.init();
        }
        m_regame_btn.SetActive(false);
        m_start_btn.SetActive(true);
    }

    public void StartTimer(float time)
    {
        if (m_timer_coroutine != null)
            StopCoroutine(m_timer_coroutine);

        m_timer_coroutine = StartCoroutine(TimerRoutine(time));
    }

    IEnumerator openRoutine()
    {
		foreach (var player_deck in m_player_decks)
		{
            player_deck.Key.GetComponent<Animator>().SetBool("select", true);

            for (int i = 0; i < player_deck.Value.Count; i++)
			{
                if (m_cards[player_deck.Value[i] - 1].isFront() == false)
                {
                    m_cards[player_deck.Value[i] - 1].animateOpen(m_canvas_sort);
                }
                else
                {
                    m_cards[player_deck.Value[i] - 1].animateSelect(m_canvas_sort);
                }

                player_deck.Key.setResultSprite(i, m_cards[player_deck.Value[i] - 1].getFrontSprite());
                m_canvas_sort++;

                yield return new WaitForSeconds(m_result_flip_term); // 1초 대기
            }

            player_deck.Key.GetComponent<Animator>().SetBool("select", false);
            yield return new WaitForSeconds(m_result_player_term); // 1초 대기
            player_deck.Key.showResult(true);
        }

		foreach (var card in m_cards)
		{
            if (card.isFront() == false)
            {
                card.animateFlip();
            }
        }

        m_regame_btn.SetActive(true);
    }

    IEnumerator BanRoutine()
    {
        for (int i = 0; i < m_ban_card_index.Count; i++)
        {
            m_cards[m_ban_card_index[i]].setBan();
            m_cards[m_ban_card_index[i]].animateSelect(m_canvas_sort);
            m_canvas_sort++;
            yield return new WaitForSeconds(1f); // 1초 대기
        }
        yield return new WaitForSeconds(1f);
        m_show_dwon_btn.SetActive(true);
    }

    private IEnumerator TimerRoutine(float time)
    {
        float remaining = time;
        m_timer_text.color = Color.black;

        while (remaining > 0f)
        {
            int seconds = Mathf.CeilToInt(remaining);
            m_timer_text.text = FormatTime(seconds);

            // 5초 이하로 남으면 색상 변경
            if (remaining <= 5f)
                m_timer_text.color = Color.red;

            remaining -= Time.deltaTime;
            yield return null;
        }

        // 0초 처리
        m_timer_text.text = "00:00";
        m_timer_text.color = Color.red;
        yield return null;

        m_timer_text.gameObject.SetActive(false);
        flipAllCards();

        yield return new WaitForSeconds(2.0f);

        // 플립 끝나면 벤 활성화
        m_ban_btn.SetActive(true);
    }

    private string FormatTime(int seconds)
    {
        int min = seconds / 60;
        int sec = seconds % 60;
        return $"{min:00}:{sec:00}";
    }

    public void ShowMessage(string message)
    {
        if (m_message_coroutine != null)
            StopCoroutine(m_message_coroutine);

        m_message_coroutine = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        m_message_text.text = message;
        Color baseColor = m_message_text.color;
        Color transparent = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        // 초기 투명도 0으로 설정
        m_message_text.color = transparent;

        // 페이드 인
        float t = 0f;
        while (t < m_fade_sec)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / m_fade_sec);
            m_message_text.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        // 대기 (표시 유지)
        yield return new WaitForSeconds(m_display_sec - (m_fade_sec * 2));

        // 페이드 아웃
        t = 0f;
        while (t < m_fade_sec)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - (t / m_fade_sec));
            m_message_text.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }

        // 완전히 투명하게 설정 후 종료
        m_message_text.color = transparent;
        m_message_text.text = string.Empty;
        m_message_coroutine = null;
    }
}
