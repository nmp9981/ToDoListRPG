using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnrollMission : MonoBehaviour
{
    [Header("미션 내용")]
    [SerializeField] TMP_InputField _missionNameText;
    [SerializeField] TMP_InputField _missionDetailText;
    [SerializeField] TaskUnit _newMissionType;
    [SerializeField] Toggle _isRepeat;
    [SerializeField] TMP_InputField _missionGetExp;
    [SerializeField] TMP_InputField _missionGetMoney;

    [Header("미션 유형별 오브젝트")]
    [SerializeField] GameObject daySelectObj;
    [SerializeField] GameObject weekSelectObj;
    [SerializeField] GameObject monthSelectObj;

    [Header("미션 유형별 생성 위치")]
    [SerializeField] Transform _dayContent;
    [SerializeField] Transform _weekContent;
    [SerializeField] Transform _monthContent;
    [SerializeField] Transform _personalContent;

    private void OnEnable()
    {
        daySelectObj.SetActive(false);
        weekSelectObj.SetActive(false);
        monthSelectObj.SetActive(false);
    }

    /// <summary>
    /// 미션 타입 선택
    /// </summary>
    public void SelectDayMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Day;
        daySelectObj.SetActive(tog.isOn);
    }
    public void SelectWeekMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Week;
        weekSelectObj.SetActive(tog.isOn);
    }
    public void SelectMonthMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Month;
        monthSelectObj.SetActive(tog.isOn);
    }
    public void SelectPersonalMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Personal;
    }

    /// <summary>
    /// 신규 미션 등록
    /// </summary>
    public void ComfirmNewMission()
    {
        //미션명 검사
        if (_missionNameText.text == string.Empty) return;

        //보상 범위 검사
        if (_missionGetExp.text.Length > 7 || _missionGetMoney.text.Length > 7) return;

        //미션 오브젝트
        GameObject _newMissionObj = Instantiate(GameManager.Instance._missionPrefab);
        MissionInfo _newMissionInfo = _newMissionObj.GetComponent<MissionInfo>();

        //미션 정보 등록
        _newMissionInfo.missionUnit = _newMissionType;
        _newMissionInfo.mission.Title = _missionNameText.text;
        _newMissionInfo.mission.getExp = int.Parse(_missionGetExp.text);
        _newMissionInfo.mission.getMoney = int.Parse(_missionGetMoney.text);
        _newMissionInfo.mission.isRepeat = _isRepeat;
        _newMissionInfo.missionDetail = _missionDetailText.text;
        _newMissionInfo.dueTime = SetDueTime();
       
        //미션 생성
        switch (_newMissionType)
        {
            case TaskUnit.Day:
                _newMissionObj.transform.parent = _dayContent;
                break;
            case TaskUnit.Week:
                _newMissionObj.transform.parent = _weekContent;
                break;
            case TaskUnit.Month:
                _newMissionObj.transform.parent = _monthContent;
                break;
            case TaskUnit.Personal:
                _newMissionObj.transform.parent = _personalContent;
                break;
            default:
                break;
        }
        //기타 UI
        _newMissionInfo.repeatTextObj.SetActive(_isRepeat);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// 마감 시간 설정
    /// </summary>
    /// <returns></returns>
    float SetDueTime()
    {
        return 1000;
    }
}
