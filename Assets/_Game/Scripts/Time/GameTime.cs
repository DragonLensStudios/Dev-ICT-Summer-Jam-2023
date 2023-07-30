using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DLS.Core
{

    /// <summary>
    /// This is the Time class, This class holds time variables and methods used for custom timers and custom clocks.
    /// To use this class simply add it as a variable or a property to utilize each of the methods and types.
    /// </summary>
    [CreateAssetMenu(fileName ="GameTime", menuName ="DLS/Game/Time/GameTime")]
    public class GameTime : ScriptableObject
    {
        #region Protected Variables

        [SerializeField] protected float m_Total_Time_Sec;
        [SerializeField] protected Days m_CurrentDay = Days.Monday;
        [SerializeField] protected Months m_CurrentMonth = Months.January;
        [SerializeField] protected float m_Year = 0;
        [SerializeField] protected float m_Month = 1;
        [SerializeField] protected float m_Week = 1;
        [SerializeField] protected float m_Day = 1;
        [SerializeField] protected float m_Hour = 0;
        [SerializeField] protected float m_Minute = 0;
        [SerializeField] protected float m_Second = 0;
        [SerializeField] protected float m_TimeScale = 1;
        [SerializeField] protected int m_SecondsInMinute = 60;
        [SerializeField] protected int m_MinutesInHour = 60;
        [SerializeField] protected int m_HoursInDay = 24;
        [SerializeField] protected int m_DaysInWeek = 7;
        [SerializeField] protected int m_DaysInMonth = 30;
        [SerializeField] protected int m_MonthsInYear = 4;

        #endregion

        #region Public Properties

        public float TotalTimeSeconds
        {
            get
            {
                var output = m_Year;
                output = output * m_MonthsInYear + m_Month;
                output = output * m_DaysInMonth + m_Day;
                output = output * m_HoursInDay + m_Hour;
                output = output * m_MinutesInHour + m_Minute;
                output = output * m_SecondsInMinute + m_Second;
                return output;
            }
        }
        public Days CurrentDay
        {
            get
            {
                return m_CurrentDay;
            }

            set
            {
                m_CurrentDay = value;
            }
        }

        public Months CurrentMonth
        {
            get
            {
                return m_CurrentMonth;
            }

            set
            {
                m_CurrentMonth = value;
            }
        }

        public float Year
        {
            get
            {
                return m_Year;
            }

            set
            {
                m_Year = value;
            }
        }

        public float Month
        {
            get
            {
                return m_Month;
            }

            set
            {
                m_Month = value;
            }
        }

        public float Week
        {
            get
            {
                return m_Week;
            }

            set
            {
                m_Week = value;
            }
        }

        public float Day
        {
            get
            {
                return m_Day;
            }

            set
            {
                m_Day = value;
            }
        }

        public float Hour
        {
            get
            {
                return m_Hour;
            }

            set
            {
                m_Hour = value;
            }
        }

        public float Minute
        {
            get
            {
                return m_Minute;
            }

            set
            {
                m_Minute = value;
            }
        }

        public float Second
        {
            get
            {
                return m_Second;
            }

            set
            {
                m_Second = value;
            }
        }

        public float TimeScale
        {
            get
            {
                return m_TimeScale;
            }

            set
            {
                m_TimeScale = value;
            }
        }

        public int SecondsInMinute
        {
            get
            {
                return m_SecondsInMinute;
            }

            set
            {
                m_SecondsInMinute = value;
            }
        }

        public int MinutesInHour
        {
            get
            {
                return m_MinutesInHour;
            }

            set
            {
                m_MinutesInHour = value;
            }
        }

        public int HoursInDay
        {
            get
            {
                return m_HoursInDay;
            }

            set
            {
                m_HoursInDay = value;
            }
        }

        public int DaysInWeek
        {
            get
            {
                return m_DaysInWeek;
            }

            set
            {
                m_DaysInWeek = value;
            }
        }

        public int DaysInMonth
        {
            get
            {
                return m_DaysInMonth;
            }

            set
            {
                m_DaysInMonth = value;
            }
        }

        public int MonthsInYear
        {
            get
            {
                return m_MonthsInYear;
            }

            set
            {
                m_MonthsInYear = value;
            }
        }

        #endregion

        #region Public Virtual Methods

        /// <summary>
        /// This sets the Seconds to Time.deltaTime * TimeScale. This effectively will set the proper selected value when called in update.
        /// this method also calls ValidateTime. <see cref="ValidateTime"/>
        /// (Must be called in Update for the Time to be properly set.)
        /// </summary>
        /// <returns></returns>
        public virtual void StartTime(TimeType _timetype = TimeType.Second)
        {
            switch (_timetype)
            {
                case TimeType.Second:
                    m_Second += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Minute:
                    m_Minute += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Hour:
                    m_Hour += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Day:
                    m_Day += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Week:
                    m_Week += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Month:
                    m_Month += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Year:
                    m_Year += UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
            }
            ValidateTime();
        }

        /// <summary>
        /// This sets the Seconds to Time.deltaTime * TimeScale. This effectively will set the proper selected value in reverse when called in update.
        /// this method also calls ValidateTime. <see cref="ValidateTime"/>
        /// (Must be called in Update for the Time to be properly set.)
        /// For example if you had a had a timer that had 5 minutes it would start going down by the value provided.
        /// </summary>
        /// <param name="_timetype"></param>
        public virtual void ReverseTime(TimeType _timetype = TimeType.Second)
        {
            switch (_timetype)
            {
                case TimeType.Second:
                    m_Second -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Minute:
                    m_Minute -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Hour:
                    m_Hour -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Day:
                    m_Day -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Week:
                    m_Week -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Month:
                    m_Month -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
                case TimeType.Year:
                    m_Year -= UnityEngine.Time.deltaTime * m_TimeScale;
                    break;
            }
            ValidateTime();
        }

        /// <summary>
        /// This method checks the time counting to make sure that after 60 seconds 1 minute is added (by default) and 60 Minutes adds 1 hour, etc...
        /// </summary>
        public virtual void ValidateTime()
        {
            if ((int)m_Second < 0 && m_Minute >= 0)
            {
                m_Minute--;
                m_Second += m_SecondsInMinute;
            }

            if ((int)m_Minute < 0 && m_Hour >= 0)
            {
                m_Hour--;
                m_Minute += m_MinutesInHour;
            }

            if ((int)m_Hour < 0 && m_Day >= 1)
            {
                m_Day--;
                if (m_CurrentDay == Days.Monday)
                {
                    m_CurrentDay = Days.Sunday;
                }
                else
                {
                    m_CurrentDay--;
                }
                m_CurrentDay--;
                m_Hour += m_HoursInDay;
            }

            if ((int)m_Day < 1 && m_Month >= 1)
            {
                m_Month--;
                m_Day = m_DaysInMonth;
            }

            if ((int)m_Month < 1 && (int)m_Year > 0)
            {
                m_Year--;
                m_Month = m_MonthsInYear;
            }

            if ((int)m_Second + 1 > m_SecondsInMinute)
            {
                m_Minute++;
                m_Second = 0;
            }
            if ((int)m_Minute + 1 > m_MinutesInHour)
            {
                m_Hour++;
                m_Minute = 0;
            }
            if ((int)m_Hour >= m_HoursInDay)
            {
                m_Day++;
                if (m_CurrentDay == Days.Sunday)
                {
                    m_CurrentDay = Days.Monday;
                }
                else
                {
                    m_CurrentDay++;
                }
                m_Hour = 0;
            }
            if ((int)m_Day > m_DaysInMonth)
            {
                m_Month++;
                m_Day = 1;
            }
            if ((int)m_Month > m_MonthsInYear)
            {
                m_Year++;
                m_Month = 1;
            }

            m_Week = (int)(m_Day / m_DaysInWeek) + 1;
            m_CurrentMonth = (Months)m_Month;

        }

        /// <summary>
        /// This will check the time provided and return true or false based on the check. This has optional parameters that can be called like this
        /// EXAMPLE: CheckFullTIme(day:5,minute:30); - This will check to make sure Day 5 and 30 Minutes is true and execute.
        /// Passing -1 on a parameter will always evaluate the selected check to true for the check call.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public virtual bool CheckTime(int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            bool yearchecked = (int)m_Year == _year;
            bool monthchecked = (int)m_Month == _month;
            bool weekchecked = (int)m_Week == _week;
            bool daychecked = (int)m_Day == _day;
            bool hourchecked = (int)m_Hour == _hour;
            bool minutechecked = (int)m_Minute == _minute;
            bool secondchecked = (int)m_Second == _second;

            if (_year == -1)
            {
                yearchecked = true;
            }
            if (_month == -1)
            {
                monthchecked = true;
            }
            if (_week == -1)
            {
                weekchecked = true;
            }
            if (_day == -1)
            {
                daychecked = true;
            }
            if (_hour == -1)
            {
                hourchecked = true;
            }
            if (_minute == -1)
            {
                minutechecked = true;
            }
            if (_second == -1)
            {
                secondchecked = true;
            }
            if (yearchecked && monthchecked && weekchecked && daychecked && hourchecked && minutechecked && secondchecked)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the time to the optional times provided. just provide a value that is not -1 in order to set the time for that slot.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void SetTime(int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if (_year != -1)
            {
                m_Year = _year;
            }
            if (_month != -1)
            {
                m_Month = _month;
            }
            if (_week != -1)
            {
                m_Week = _week;
            }
            if (_day != -1)
            {
                m_Day = _day;
            }
            if (_hour != -1)
            {
                m_Hour = _hour;
            }
            if (_minute != -1)
            {
                m_Minute = _minute;
            }
            if (_second != -1)
            {
                m_Second = _second;
            }
            ValidateTime();
        }

        /// <summary>
        /// Adds the time to the optional times provided. just provide a value that is not -1 in order to add the time for that slot.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void AddTime(int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if (_year != -1)
            {
                m_Year += _year;
            }
            if (_month != -1)
            {
                m_Month += _month;
            }
            if (_week != -1)
            {
                m_Week += _week;
            }
            if (_day != -1)
            {
                m_Day += _day;
            }
            if (_hour != -1)
            {
                m_Hour += _hour;
            }
            if (_minute != -1)
            {
                m_Minute += _minute;
            }
            if (_second != -1)
            {
                m_Second += _second;
            }
            ValidateTime();
        }

        /// <summary>
        /// Removes the time to the optional times provided. just provide a value that is not -1 in order to remove the time for that slot.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void RemoveTime(int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if (_year != -1)
            {
                m_Year -= _year;
            }
            if (_month != -1)
            {
                m_Month -= _month;
            }
            if (_week != -1)
            {
                m_Week -= _week;
            }
            if (_day != -1)
            {
                m_Day -= _day;
            }
            if (_hour != -1)
            {
                m_Hour -= _hour;
            }
            if (_minute != -1)
            {
                m_Minute -= _minute;
            }
            if (_second != -1)
            {
                m_Second -= _second;
            }
            ValidateTime();
        }

        /// <summary>
        /// Multiplies the time to the optional times provided. just provide a value that is not -1 in order to multiply the time for that slot.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void MultiplyTime(int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if (_year != -1)
            {
                m_Year *= _year;
            }
            if (_month != -1)
            {
                m_Month *= _month;
            }
            if (_week != -1)
            {
                m_Week *= _week;
            }
            if (_day != -1)
            {
                m_Day *= _day;
            }
            if (_hour != -1)
            {
                m_Hour *= _hour;
            }
            if (_minute != -1)
            {
                m_Minute *= _minute;
            }
            if (_second != -1)
            {
                m_Second *= _second;
            }
            ValidateTime();
        }

        /// <summary>
        /// Divides the time to the optional times provided. just provide a value that is not -1 in order to divide the time for that slot.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void DivideTime(int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if (_year != -1 && _year != 0)
            {
                m_Year /= _year;
            }
            if (_month != -1 && _month != 0)
            {
                m_Month /= _month;
            }
            if (_week != -1 && _week != 0)
            {
                m_Week /= _week;
            }
            if (_day != -1 && _day != 0)
            {
                m_Day /= _day;
            }
            if (_hour != -1 && _hour != 0)
            {
                m_Hour /= _hour;
            }
            if (_minute != -1 && _minute != 0)
            {
                m_Minute /= _minute;
            }
            if (_second != -1 && _second != 0)
            {
                m_Second /= _second;
            }
            ValidateTime();
        }
        /// <summary>
        /// Sets the time to the provided GameTime to the optional times provided. just provide a value that is not -1 in order to set the time for that slot.
        /// </summary>
        /// <param name="_gametime"></param>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void SetTimeToGameTime(GameTime _gametime, int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if(_gametime != null)
            {
                if (_year != -1)
                {
                    m_Year = _gametime.m_Year;
                }
                if (_month != -1)
                {
                    m_Month = _gametime.m_Month;
                }
                if (_week != -1)
                {
                    m_Week = _gametime.m_Week;
                }
                if (_day != -1)
                {
                    m_Day = _gametime.m_Day;
                }
                if (_hour != -1)
                {
                    m_Hour = _gametime.m_Hour;
                }
                if (_minute != -1)
                {
                    m_Minute = _gametime.m_Minute;
                }
                if (_second != -1)
                {
                    m_Second = _gametime.m_Second;
                }
                ValidateTime();
            }
        }

        /// <summary>
        /// Adds the time by the provided GameTime to the optional times provided. just provide a value that is not -1 in order to add the time for that slot.
        /// </summary>
        /// <param name="_gametime"></param>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public void AddTimeByGameTime(GameTime _gametime, int _year = -1, int _month = -1, int _week = -1, int _day = -1, int _hour = -1, int _minute = -1, int _second = -1)
        {
            if (_gametime != null)
            {
                if (_year != -1)
                {
                    m_Year += _gametime.m_Year;
                }
                if (_month != -1)
                {
                    m_Month += _gametime.m_Month;
                }
                if (_week != -1)
                {
                    m_Week += _gametime.m_Week;
                }
                if (_day != -1)
                {
                    m_Day += _gametime.m_Day;
                }
                if (_hour != -1)
                {
                    m_Hour += _gametime.m_Hour;
                }
                if (_minute != -1)
                {
                    m_Minute += _gametime.m_Minute;
                }
                if (_second != -1)
                {
                    m_Second += _gametime.m_Second;
                }
                ValidateTime();
            }
        }

        /// <summary>
        /// <para>Checks the Exact Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_second"></param>
        /// <returns></returns>
        public bool CheckExactTime(int _second)
        {
            if ((int)m_Second == _second)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the Exact Minute, Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public bool CheckExactTime(int _minute, int _second)
        {
            if (m_Minute == _minute && (int)m_Second == _second) //Refactor: Find a way to work with float's better for this.
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the Exact Hour, Minute, Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns>test</returns>
        public bool CheckExactTime(int _hour, int _minute, int _second)
        {
            if (m_Hour == _hour && m_Minute == _minute && (int)m_Second == _second) //Refactor: Find a way to work with float's better for this.
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the Exact Day, Hour, Minute, Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public bool CheckExactTime(int _day, int _hour, int _minute, int _second)
        {
            if (m_Day == _day && m_Hour == _hour && m_Minute == _minute && (int)m_Second == _second) //Refactor: Find a way to work with float's better for this.
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the Exact Week, Day, Hour, Minute, Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public bool CheckExactTime(int _week, int _day, int _hour, int _minute, int _second)
        {
            if (m_Week == _week && m_Day == _day && m_Hour == _hour && m_Minute == _minute && (int)m_Second == _second) //Refactor: Find a way to work with float's better for this.
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the Exact Month, Week, Day, Hour, Minute, Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public bool CheckExactTime(int _month, int _week, int _day, int _hour, int _minute, int _second)
        {
            if (m_Month == _month && m_Week == _week && m_Day == _day && m_Hour == _hour && m_Minute == _minute && (int)m_Second == _second) //Refactor: Find a way to work with float's better for this.
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the Exact Year, Month, Week, Day, Hour, Minute, Second.</para>
        /// Returns True if the time has been reached otherwise returns False.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        /// <returns></returns>
        public bool CheckExactTime(int _year, int _month, int _week, int _day, int _hour, int _minute, int _second)
        {
            if (m_Year == _year && m_Month == _month && m_Week == _week && m_Day == _day && m_Hour == _hour && m_Minute == _minute && (int)m_Second == _second) //Refactor: Find a way to work with float's better for this.
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// <para>Checks the time between two provided <see cref="GameTime"/> </para>
        /// Returns True if the time current time is within the spcified two times and selected time checks. Otherwise returns False.
        /// </summary>
        /// <param name="_time1"></param>
        /// <param name="_time2"></param>
        /// <param name="_checkYear"></param>
        /// <param name="_checkMonth"></param>
        /// <param name="_checkWeek"></param>
        /// <param name="_checkDay"></param>
        /// <param name="_checkHour"></param>
        /// <param name="_checkMinute"></param>
        /// <param name="_checkSecond"></param>
        /// <returns></returns>
        public bool CheckTimeBetweenGameTimes(GameTime _time1, GameTime _time2, bool _checkYear = false, bool _checkMonth = false, bool _checkWeek = false, bool _checkDay = false, bool _checkHour = false, bool _checkMinute = false, bool _checkSecond = false)
        {
            if (_time1 == null || _time2 == null) { return false; }
            bool isInbetween = false;
            if (_checkYear) { isInbetween = (m_Year >= _time1.m_Year && m_Year <= _time2.m_Year); }
            if (_checkMonth) { isInbetween = (m_Month >= _time1.m_Month && m_Month <= _time2.m_Month); }
            if (_checkWeek) { isInbetween = (m_Week >= _time1.m_Week && m_Week <= _time2.m_Week); }
            if (_checkDay) { isInbetween = (m_Day >= _time1.m_Day && m_Day <= _time2.m_Day); }
            if (_checkHour) { isInbetween = (m_Hour >= _time1.m_Hour && m_Hour <= _time2.m_Hour); }
            if (_checkMinute) { isInbetween = (m_Minute >= _time1.m_Minute && m_Minute <= _time2.m_Minute); }
            if (_checkSecond) { isInbetween = (m_Second >= _time1.m_Second && m_Second <= _time2.m_Second); }

            return isInbetween;
        }

        /// <summary>
        /// Resets the time to the optional values use true for each value you want to reset.
        /// </summary>
        /// <param name="_year"></param>
        /// <param name="_month"></param>
        /// <param name="_week"></param>
        /// <param name="_day"></param>
        /// <param name="_hour"></param>
        /// <param name="_minute"></param>
        /// <param name="_second"></param>
        public virtual void ResetTime(bool _year = false, bool _month = false, bool _week = false, bool _day = false, bool _hour = false, bool _minute = false, bool _second = false)
        {
            if (_year)
            {
                m_Year = 0;
            }
            if (_month)
            {
                m_Month = 1;
                m_CurrentMonth = Months.January;
            }
            if (_week)
            {
                m_Week = 1;
            }
            if (_day)
            {
                m_Day = 1;
            }
            if (_hour)
            {
                m_Hour = 0;
            }
            if (_minute)
            {
                m_Minute = 0;
            }
            if (_second)
            {
                m_Second = 0;
            }

        }

        /// <summary>
        /// Resets the Days, Hours, Minutes, and Seconds within a month and the week.
        /// </summary>
        public virtual void ResetMonth()
        {
            m_Day = 1;
            m_Week = 1;
            m_CurrentDay = Days.Monday;
            m_Hour = 0;
            m_Minute = 0;
            m_Second = 0;
        }

        /// <summary>
        /// Resets the day to the day of the week first.
        /// </summary>
        public virtual void ResetWeek()
        {
            m_Day -= (int)m_Day % m_DaysInWeek;  // Not sure if this works yet.? Untested
        }

        /// <summary>
        /// Resets the hours, minutes and seconds within a day.
        /// </summary>
        public virtual void ResetDay()
        {
            m_Hour = 0;
            m_Minute = 0;
            m_Second = 0;
        }

        /// <summary>
        /// Resets the Minutes and seconds within an hour.
        /// </summary>
        public virtual void ResetHour()
        {
            m_Minute = 0;
            m_Second = 0;
        }

        /// <summary>
        /// Resets the seconds within a minute.
        /// </summary>
        public virtual void ResetMinute()
        {
            m_Second = 0;
        }

        /// <summary>
        /// Resets all of the values to the defaults 1/1/1 00:00:00
        /// </summary>
        public virtual void ResetFullDate()
        {
            m_CurrentDay = Days.Monday;
            m_CurrentMonth = Months.January;
            m_Year = 1;
            m_Month = 1;
            m_Week = 1;
            m_Day = 1;
            m_Hour = 0;
            m_Minute = 0;
            m_Second = 0;
        }

        #endregion

    }
}