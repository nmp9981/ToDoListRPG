using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화이트리스트 편집 UI
/// 필요한 UI 구조:
/// - InputField_Process (TMP_InputField) : 프로세스 이름 입력
/// - InputField_Url     (TMP_InputField) : URL 입력
/// - Button_AddProcess  (Button)
/// - Button_AddUrl      (Button)
/// - Content_Process    (Transform)      : ScrollView Content
/// - Content_Url        (Transform)      : ScrollView Content
/// - Prefab_ListItem    (GameObject)     : Text + 삭제버튼 프리팹
/// </summary>
public class WhiteListEditorUI : MonoBehaviour
{
    [Header("프로세스 섹션")]
    [SerializeField] private TMP_InputField _processInput;
    [SerializeField] private Button _addProcessBtn;
    [SerializeField] private Transform _processContent;

    [Header("URL 섹션")]
    [SerializeField] private TMP_InputField _urlInput;
    [SerializeField] private Button _addUrlBtn;
    [SerializeField] private Transform _urlContent;

    [Header("리스트 아이템 프리팹")]
    [Tooltip("TMP_Text + Button(삭제) 가 있는 프리팹")]
    [SerializeField] private GameObject _listItemPrefab;

    // ───────── Unity 생명주기 ─────────
    private void Start()
    {
        _addProcessBtn.onClick.AddListener(OnAddProcess);
        _addUrlBtn.onClick.AddListener(OnAddUrl);
        RefreshAll();
    }

    // ───────── 버튼 이벤트 ─────────
    private void OnAddProcess()
    {
        string val = _processInput.text.Trim();
        if (string.IsNullOrEmpty(val)) return;

        WhitelistManager.Instance.AddProcess(val);
        _processInput.text = "";
        RefreshProcessList();
    }

    private void OnAddUrl()
    {
        string val = _urlInput.text.Trim();
        if (string.IsNullOrEmpty(val)) return;

        WhitelistManager.Instance.AddUrl(val);
        _urlInput.text = "";
        RefreshUrlList();
    }

    // ───────── 리스트 갱신 ─────────
    private void RefreshAll()
    {
        RefreshProcessList();
        RefreshUrlList();
    }

    private void RefreshProcessList()
    {
        ClearContent(_processContent);
        foreach (var item in WhitelistManager.Instance.GetProcessList())
        {
            string captured = item;
            SpawnItem(_processContent, item, () =>
            {
                WhitelistManager.Instance.RemoveProcess(captured);
                RefreshProcessList();
            });
        }
    }

    private void RefreshUrlList()
    {
        ClearContent(_urlContent);
        foreach (var item in WhitelistManager.Instance.GetUrlList())
        {
            string captured = item;
            SpawnItem(_urlContent, item, () =>
            {
                WhitelistManager.Instance.RemoveUrl(captured);
                RefreshUrlList();
            });
        }
    }

    // ───────── 유틸 ─────────
    private void ClearContent(Transform content)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
    }

    private void SpawnItem(Transform parent, string label, UnityEngine.Events.UnityAction onDelete)
    {
        var go = Instantiate(_listItemPrefab, parent);

        // 텍스트 설정
        var text = go.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = label;

        // 삭제 버튼 설정
        var btn = go.GetComponentInChildren<Button>();
        if (btn != null) btn.onClick.AddListener(onDelete);
    }
}
