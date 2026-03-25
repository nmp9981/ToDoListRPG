using System;
using System.Diagnostics;
using UnityEngine.UI;

public static class CalTimeUtility
{
    public static int DiffTime_Day(int inputHour, int inputMinute)
    {
        DateTime curTime = DateTime.Now;

        //초차
        int diffSecond = 60 - curTime.Second;
        //분차
        int diffMinute = inputMinute - curTime.Minute - 1;
        //시차
        int diffHour = (diffMinute<0)?inputHour-1-curTime.Hour:inputHour - curTime.Hour;

        //받아내림 보정
        diffMinute = (diffMinute < 0) ? diffMinute + 60 : diffMinute;
        diffHour = (diffHour < 0) ? diffHour + 24 : diffHour;
     
        //총 시간
        return diffSecond + 60 * diffMinute + 3600 * diffHour;
    }

    public static int DiffTime_Week(int inputHour, int inputMinute, Toggle weekTog)
    {
        DateTime curTime = DateTime.Now;

        //초차
        int diffSecond = 60 - curTime.Second;
        //분차
        int diffMinute = inputMinute - curTime.Minute - 1;
        //시차
        int diffHour = (diffMinute < 0) ? inputHour - 1 - curTime.Hour : inputHour - curTime.Hour;
        //요일차
        int inputWeek = int.Parse(weekTog.gameObject.name.Substring(4,1));
        int curWeek = (int)curTime.DayOfWeek;
        int diffWeek = (diffHour < 0) ? inputWeek-1 - curWeek:inputWeek-curWeek;

        //받아내림 보정
        diffMinute = (diffMinute < 0) ? diffMinute + 60 : diffMinute;
        diffHour = (diffHour < 0) ? diffHour + 24 : diffHour;
        diffWeek = (diffWeek < 0) ? diffWeek + 7 : diffWeek;

        //총 시간
        return diffSecond + 60 * diffMinute + 3600 * diffHour+ 86400*diffWeek;
    }
}
