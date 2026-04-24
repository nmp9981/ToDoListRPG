using System;
using UnityEngine;
using UnityEngine.UI;

public static class CalTimeUtility
{
    public const int minutesUnit = 60;
    public const int hourUnit = 3600;
    public const int dayUnit = 86400;
    public const int yearUnit = 31536000;

    /// <summary>
    /// 숫자(초)를 시간으로
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string NumToTime(float time)
    {
        string timeText = string.Empty;

        return timeText;
    }
    public static int DiffTime_Day(int inputHour, int inputMinute)
    {
        DateTime curTime = DateTime.Now;

        //초차
        int diffSecond = minutesUnit - curTime.Second;
        //분차
        int diffMinute = inputMinute - curTime.Minute - 1;
        //시차
        int diffHour = (diffMinute<0)?inputHour-1-curTime.Hour:inputHour - curTime.Hour;

        //받아내림 보정
        diffMinute = (diffMinute < 0) ? diffMinute + minutesUnit : diffMinute;
        diffHour = (diffHour < 0) ? diffHour + 24 : diffHour;
     
        //총 시간
        return diffSecond + minutesUnit * diffMinute + hourUnit * diffHour;
    }

    public static int DiffTime_Week(int inputHour, int inputMinute, Toggle weekTog)
    {
        DateTime curTime = DateTime.Now;

        //초차
        int diffSecond = minutesUnit - curTime.Second;
        //분차
        int diffMinute = inputMinute - curTime.Minute - 1;
        //시차
        int diffHour = (diffMinute < 0) ? inputHour - 1 - curTime.Hour : inputHour - curTime.Hour;
        //요일차
        int inputWeek = int.Parse(weekTog.gameObject.name.Substring(4,1));
        int curWeek = (int)curTime.DayOfWeek;
        int diffWeek = (diffHour < 0) ? inputWeek-1 - curWeek:inputWeek-curWeek;

        //받아내림 보정
        diffMinute = (diffMinute < 0) ? diffMinute + minutesUnit: diffMinute;
        diffHour = (diffHour < 0) ? diffHour + 24 : diffHour;
        diffWeek = (diffWeek < 0) ? diffWeek + 7 : diffWeek;

        //총 시간
        return diffSecond + minutesUnit * diffMinute + hourUnit * diffHour+ dayUnit*diffWeek;
    }
    /// <summary>
    /// 월간 미션 - N일
    /// </summary>
    /// <param name="inputHour"></param>
    /// <param name="inputMinute"></param>
    /// <param name="inputDay"></param>
    /// <returns></returns>
    public static int DiffTime_Month(int inputHour, int inputMinute, int inputDay)
    {
        DateTime curTime = DateTime.Now;

        //초차
        int diffSecond = minutesUnit - curTime.Second;
        //분차
        int diffMinute = inputMinute - curTime.Minute - 1;
        //시차
        int diffHour = (diffMinute < 0) ? inputHour - 1 - curTime.Hour : inputHour - curTime.Hour;
        //일차
        int diffDay = (diffHour < 0) ? inputDay - 1 - curTime.Day : inputDay - 1 - curTime.Day;

        //받아내림 보정
        diffMinute = (diffMinute < 0) ? diffMinute + minutesUnit : diffMinute;
        diffHour = (diffHour < 0) ? diffHour + 24 : diffHour;
        int addDay = AddMonthDay(curTime.Month, curTime.Year);
        diffDay = (diffDay < 0) ? diffDay + addDay : diffDay;

        //총 시간
        return diffSecond + minutesUnit * diffMinute + hourUnit * diffHour + dayUnit * diffDay;
    }
    /// <summary>
    /// 월간 미션 - N째주 k요일
    /// </summary>
    /// <param name="inputHour"></param>
    /// <param name="inputMinute"></param>
    /// <param name="weekTog"></param>
    /// <returns></returns>
    public static int DiffTime_WeekMonth(int inputHour, int inputMinute, Toggle weekTog,int weekN)
    {
        DateTime curTime = DateTime.Now;

        //다음 N째주 K요일의 날짜
        int curWeek = (int)curTime.DayOfWeek;//현재 요일
        int goalWeek = int.Parse(weekTog.gameObject.name.Substring(4, 1));//목표 요일
        int curMonthDay = 0;
        int nextMonthDay = 0;

        //1일 기준
        //이번달
        DateTime firstDay_CurMonth = new DateTime(curTime.Year, curTime.Month, 1);
        int firstDayOfWeek_CurMonth = (int)firstDay_CurMonth.DayOfWeek;//1일이 무슨 요일인가?
        if (firstDayOfWeek_CurMonth <= 4)//월~목 : 1일은 첫째주
        {
            int diffWeek = goalWeek - firstDayOfWeek_CurMonth;
            curMonthDay = (weekN - 1) * 7 + diffWeek+1;
            curMonthDay = Mathf.Max(1,Mathf.Min(AddMonthDay(curTime.Month, curTime.Year), curMonthDay));
        }
        else//8일이 첫째주
        {
            int diffWeek = goalWeek - firstDayOfWeek_CurMonth;
            curMonthDay = weekN * 7 + diffWeek+1;
            curMonthDay = Mathf.Max(1, Mathf.Min(AddMonthDay(curTime.Month, curTime.Year), curMonthDay));
        }
        //다음달
        DateTime firstDay_NextMonth = new DateTime(curTime.Year, curTime.Month+1, 1);
        int firstDayOfWeek_NextMonth = (int)firstDay_NextMonth.DayOfWeek;//1일이 무슨 요일인가?
        if (firstDayOfWeek_NextMonth <= 4)//월~목 : 1일은 첫째주
        {
            int diffWeek = goalWeek - firstDayOfWeek_NextMonth;
            nextMonthDay = (weekN - 1) * 7 + diffWeek + 1;
            nextMonthDay = Mathf.Max(1, Mathf.Min(AddMonthDay(curTime.Month, curTime.Year), nextMonthDay));
        }
        else//8일이 첫째주
        {
            int diffWeek = goalWeek - firstDayOfWeek_NextMonth;
            nextMonthDay = weekN * 7 + diffWeek + 1;
            nextMonthDay = Mathf.Max(1, Mathf.Min(AddMonthDay(curTime.Month, curTime.Year), nextMonthDay));
        }

        int day = (curTime.Day >= curMonthDay) ? nextMonthDay : curMonthDay;//이미 이번달 날짜 지났는지 여부
        return DiffTime_Month(inputHour, inputMinute, day);
    }
    /// <summary>
    /// 개인 미션 - 입력 날짜
    /// </summary>
    /// <param name="inputHour"></param>
    /// <param name="inputMinute"></param>
    /// <param name="inputDay"></param>
    /// <returns></returns>
    public static int DiffTime_Full(DateTime inputDate)
    {
        DateTime curTime = DateTime.Now;
        DateTime curTimeDay = curTime.Date;
        DateTime inputDateDay = new DateTime(inputDate.Year, inputDate.Month, inputDate.Day);
        int diffDay_TimeSpan = (inputDateDay - curTimeDay).Days;//일수 차
       
        //초차
        int diffSecond = minutesUnit - curTime.Second;
        //분차
        int diffMinute = inputDate.Minute - curTime.Minute - 1;
        //시차
        int diffHour = (diffMinute < 0) ? inputDate.Hour - 1 - curTime.Hour :inputDate.Hour - curTime.Hour;
        //일차
        int diffDay = (diffHour < 0) ? diffDay_TimeSpan - 1 : diffDay_TimeSpan;
   
        //받아내림 보정
        diffMinute = (diffMinute < 0) ? diffMinute + minutesUnit : diffMinute;
        diffHour = (diffHour < 0) ? diffHour + 24 : diffHour;

        //총 시간
        return diffSecond + minutesUnit * diffMinute + hourUnit * diffHour + dayUnit * diffDay;
    }
    /// <summary>
    /// 초를 일단위까지
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string SecondToDay(float second)
    {
        int year = (int)second / yearUnit;//연
        int yearRest = (int)second % yearUnit;
        int day = (int)yearRest / dayUnit;//일
        int dayRest = (int)yearRest % dayUnit;
        int hour = (int)dayRest / hourUnit;//시간
        int hourRest = (int)dayRest % hourUnit;
        int minute = (int)hourRest / minutesUnit;//분

        string dayString = $"{year}년 {day}일 {hour}시간 {minute}분";
        return dayString;
    }
    /// <summary>
    /// 초를 시간단위까지
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public static string SecondToHour(float second)
    {
        int hour = (int)second / hourUnit;//시간
        int hourRest = (int)second % hourUnit;
        int minute = (int)hourRest / minutesUnit;//분

        string hourString = (hour == 0) ? string.Empty : $"{hour}시간";
        string minuteString = (minute == 0) ? string.Empty : $" {minute}분";
        string dayString = hourString+" "+minuteString;

        if (hour == 0 && minute == 0) dayString = "0분";
        return dayString;
    }
    /// <summary>
    /// 각 요일별 개수
    /// </summary>
    /// <returns></returns>
    public static int WeekCount(DateTime date)
    {
        int countValue = 0;
        switch (date.DayOfWeek)
        {
            case DayOfWeek.Sunday:
                countValue = 6;
                break;
            case DayOfWeek.Monday:
                countValue = 0;
                break;
            case DayOfWeek.Tuesday:
                countValue = 1;
                break;
            case DayOfWeek.Wednesday:
                countValue = 2;
                break;
            case DayOfWeek.Thursday:
                countValue = 3;
                break;
            case DayOfWeek.Friday:
                countValue = 4;
                break;
            case DayOfWeek.Saturday:
                countValue = 5;
                break;
            default:
                break;
        }
        return countValue;
    }
    /// <summary>
    /// 요일 번호 -> 요일 문자
    /// </summary>
    /// <returns></returns>
    public static string NumToStringWeek(int num)
    {
        string weekText = string.Empty;

        switch (num)
        {
            case 0:
                weekText = "월";
                break;
            case 1:
                weekText = "화";
                break;
            case 2:
                weekText = "수";
                break;
            case 3:
                weekText = "목";
                break;
            case 4:
                weekText = "금";
                break;
            case 5:
                weekText = "토";
                break;
            case 6:
                weekText = "일";
                break;
            default:
                break;
        }
        return weekText;
    }

    /// <summary>
    /// 각 달별 일 수 
    /// </summary>
    /// <param name="month"></param>
    /// <returns></returns>
    public static int AddMonthDay(int month, int year)
    {
        switch (month)
        {
            case 1:
                return 31;
            case 2:
                //윤년 판정
                if (year % 4 == 0)//4의 배수
                {
                    if (year % 100 != 0)//100의 배수 아님
                    {
                        return 29;
                    }
                    else{
                        if (year % 400 == 0)//400의 배수
                        {
                            return 29;
                        }else return 28;//400의 배수 아님 -> 평년
                    }
                }
                return 28;
            case 3:
                return 31;
            case 4:
                return 30;
            case 5:
                return 31;
            case 6:
                return 30;
            case 7:
                return 31;
            case 8:
                return 31;
            case 9:
                return 30;
            case 10:
                return 31;
            case 11:
                return 30;
            case 12:
                return 31;
            default:
                break;
        }
        return 0;
    }
}
