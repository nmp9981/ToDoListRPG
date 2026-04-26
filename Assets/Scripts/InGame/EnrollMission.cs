using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MonthType
{
    NDay,
    KWeek
}

public class EnrollMission : MonoBehaviour
{
    [Header("미션 내용")]
    [SerializeField] TMP_InputField _missionNameText;
    [SerializeField] TMP_InputField _missionDetailText;
    [SerializeField] TaskUnit _newMissionType;
    [SerializeField] MonthType _newMonthType;
    [SerializeField] Toggle _isRepeat;
    [SerializeField] TMP_InputField _missionGetExp;
    [SerializeField] TMP_InputField _missionGetMoney;
    [SerializeField] TMP_InputField _missionDueHour;
    [SerializeField] TMP_InputField _missionDueMinute;
    [SerializeField] TMP_InputField _missionDueDay;
    [SerializeField] TMP_InputField _missionDueWeekN;

    [Header("개인 미션 날짜 입력")]
    [SerializeField] TMP_InputField _personalmissionDueYearInput;
    [SerializeField] TMP_InputField _personalmissionDueMonthInput;
    [SerializeField] TMP_InputField _personalmissionDueDayInput;
    [SerializeField] TMP_InputField _personalmissionDueHourInput;
    [SerializeField] TMP_InputField _personalmissionDueMinuteInput;

    [Header("미션 기한")]
    [SerializeField] ToggleGroup _weekToggleGroup;
    [SerializeField] ToggleGroup _monthToggleGroup;
    [SerializeField] TextMeshProUGUI _deadlineText;

    [Header("미션 유형별 오브젝트")]
    [SerializeField] GameObject daySelectObj;
    [SerializeField] GameObject weekSelectObj;
    [SerializeField] GameObject monthSelectObj;
    [SerializeField] GameObject daySelect_MonthObj;
    [SerializeField] GameObject weekN_MonthObj;
    [SerializeField] GameObject missionRepeatTypeObj;
    [SerializeField] GameObject personalMissionDueAreaObj;

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
        daySelect_MonthObj.SetActive(false);
        personalMissionDueAreaObj.SetActive(false);
    }

    /// <summary>
    /// 미션 타입 선택
    /// </summary>
    public void SelectDayMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Day;
        daySelectObj.SetActive(tog.isOn);
        missionRepeatTypeObj.SetActive(true);
    }
    public void SelectWeekMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Week;
        weekSelectObj.SetActive(tog.isOn);
        daySelectObj.SetActive(tog.isOn);
        missionRepeatTypeObj.SetActive(true);
    }
    public void SelectMonthMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Month;
        monthSelectObj.SetActive(tog.isOn);
        missionRepeatTypeObj.SetActive(true);
    }
    public void SelectPersonalMissionType(Toggle tog)
    {
        _newMissionType = TaskUnit.Personal;
        missionRepeatTypeObj.SetActive(false);
        personalMissionDueAreaObj.SetActive(tog.isOn);
    }
    public void Select_Month_NDay(Toggle tog)
    {
        _newMonthType = MonthType.NDay;
        daySelect_MonthObj.SetActive(tog.isOn);
    }
    public void Select_Month_NWeek(Toggle tog)
    {
        _newMonthType = MonthType.KWeek;
        weekSelectObj.SetActive(tog.isOn);
        weekN_MonthObj.SetActive(tog.isOn);
    }

    /// <summary>
    /// 신규 미션 등록
    /// </summary>
    public void ComfirmNewMission()
    {
        //미션명 검사
        if (_missionNameText.text == string.Empty) return;

        //보상 입력 검사
        if (_missionGetExp.text == string.Empty)
        {
            UIManager.UIInstance.ShowMessage("보상을 입력해주세요", Color.black);
            return;
        }

        //보상 범위 검사
        if (_missionGetExp.text.Length > 7)
        {
            UIManager.UIInstance.ShowMessage("최대 7자리까지만 입력할 수 있습니다.", Color.black);
            return;
        }

        //미션 오브젝트
        GameObject _newMissionObj = Instantiate(GameManager.Instance._missionPrefab);
        MissionInfo _newMissionInfo = _newMissionObj.GetComponent<MissionInfo>();

        //미션 정보 등록
        _newMissionInfo.isComplete = false;
        _newMissionInfo.missionData.missionUnit = _newMissionType;
        _newMissionInfo.missionData.mission.Title = _missionNameText.text;
        _newMissionInfo.missionData.mission.getExp = int.Parse(_missionGetExp.text);
        _newMissionInfo.missionData.mission.isRepeat = (_newMissionType != TaskUnit.Personal)? _isRepeat.isOn:false;
        _newMissionInfo.missionData.missionDetail = _missionDetailText.text;
        _newMissionInfo.dueTime = SetDueTime(_newMissionType);
        _newMissionInfo.SetDeadline();
        
        //날짜 검사에서 미달시 미션생성 X
        if (Fail_InspectInputTime(_newMissionType))
        {
            UIManager.UIInstance.ShowMessage("올바른 날짜를 입력하시오.", Color.black);
            Destroy(_newMissionObj);
            return;
        }

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
        //감소 HP설정
        _newMissionInfo.SetDecreaseHP();

        //UI 공개
        _newMissionInfo.ShowMissionUI();

        //타이머 시작
        _newMissionInfo.StartTimer();

        //Json 파일 추가
        SaveManager.Instance.Data.activeMissions.Add(_newMissionInfo.missionData);

        //미션 등록 창 닫기
        Close_EnrollMisison();
    }

    /// <summary>
    /// 마감 시간 설정
    /// </summary>
    /// <returns></returns>
    float SetDueTime(TaskUnit unit)
    {
        int restTime = 0;
        int hour = 0;
        int minute = 0;

        switch (unit)
        {
            case TaskUnit.Day:
                hour = int.Parse(_missionDueHour.text);
                minute = int.Parse(_missionDueMinute.text);
                restTime = CalTimeUtility.DiffTime_Day(hour, minute);
                break;
            case TaskUnit.Week:
                hour = int.Parse(_missionDueHour.text);
                minute = int.Parse(_missionDueMinute.text);
                restTime = CalTimeUtility.DiffTime_Week(hour, minute, _weekToggleGroup.GetFirstActiveToggle());
                break;
            case TaskUnit.Month:
                if (_newMonthType == MonthType.NDay)
                {
                    int day = int.Parse(_missionDueDay.text);
                    restTime = CalTimeUtility.DiffTime_Month(hour, minute, day);
                }
                else if (_newMonthType == MonthType.KWeek)
                {
                    int weekNum = int.Parse(_missionDueWeekN.text);
                    restTime = CalTimeUtility.DiffTime_WeekMonth(hour, minute, _weekToggleGroup.GetFirstActiveToggle(),weekNum);
                }
                break;
            case TaskUnit.Personal:
                int yearInput = int.Parse(_personalmissionDueYearInput.text);
                int monthInput = int.Parse(_personalmissionDueMonthInput.text);
                int dayInput = int.Parse(_personalmissionDueDayInput.text);
                int hourInput = int.Parse(_personalmissionDueHourInput.text);
                int minuteInput = int.Parse(_personalmissionDueMonthInput.text);
                DateTime inputDate = new DateTime(yearInput, monthInput, dayInput, hourInput, minuteInput,0);
                restTime = CalTimeUtility.DiffTime_Full(inputDate);
                break;
            default:
                break;
        }
        return restTime;
    }

    //입력 검사
    bool Fail_InspectInputTime(TaskUnit unit)
    {
        int hour = 0;
        int minute = 0;
        int day = 0;
        int dayPermission = 0;
        int weekNum = 0;
        int month = 0;
        int year = DateTime.Now.Year;

        switch (unit)
        {
            case TaskUnit.Day:
                hour = int.Parse(_missionDueHour.text);
                minute = int.Parse(_missionDueMinute.text);
                break;
            case TaskUnit.Week:
                hour = int.Parse(_missionDueHour.text);
                minute = int.Parse(_missionDueMinute.text);
                break;
            case TaskUnit.Month:
                if (_newMonthType == MonthType.NDay)
                {
                    day = int.Parse(_missionDueDay.text);
                }
                else if (_newMonthType == MonthType.KWeek)
                {
                    weekNum = int.Parse(_missionDueWeekN.text);
                }
                break;
            case TaskUnit.Personal:
                year = int.Parse(_personalmissionDueYearInput.text);
                month = int.Parse(_personalmissionDueMonthInput.text);
                dayPermission = int.Parse(_personalmissionDueDayInput.text);
                hour = int.Parse(_personalmissionDueHourInput.text);
                minute = int.Parse(_personalmissionDueMonthInput.text);
                break;
            default:
                break;
        }

        //검사
        if (hour >= 24 || minute >= 60) return true;
        if (day > 31 || month > 12) return true;
        if (weekNum > 4) return true;
        int nowYear = DateTime.Now.Year;
        if (year < nowYear || year > nowYear + 200) return true;
        int maxDay = CalTimeUtility.AddMonthDay(month,year);
        if(dayPermission > maxDay) return true;

        return false;
    }
    /// <summary>
    /// 미션 등록 창 닫기
    /// </summary>
    public void Close_EnrollMisison()
    {
        gameObject.SetActive(false);
    }
}