using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using System.Linq;
using XamarinFormsCalendar.Controls;
using Xamarin.Forms.Internals;
using System.Runtime.CompilerServices;

namespace XamarinFormsCalendar.View
{
    public partial class CalendarView : ContentView, INotifyPropertyChanged
    {
        CalendarCell[] _cells = new CalendarCell[42];
        DayOfWeekLabel[] _dayOfWeekLabels;
        int _startPos = 0;
        DayOfWeekLabel[] _dayOfWeekWithSelectedDayInFront = new DayOfWeekLabel[7];

        DayOfWeek _startOfWeek = DayOfWeek.Sunday;
        DateTime _currentDate = DateTime.Now;
        bool _selectBetweenTwoDatesMode = false;

        public DayOfWeek StartOfWeek 
        {
            get => _startOfWeek;
            set 
            {
                _startOfWeek = value;

                StartOfWeekChanged();
            } 
        }

        public DateTime CurrentDate
        {
            get { return _currentDate; }
            set 
            {
                _currentDate = value;
                SetupCellsForDate(_currentDate);
                OnPropertyChanged();
                OnPropertyChanged("CurrentMonth");
            }
        }

        public string CurrentMonth 
        {
            get
            {
                return DateHelper.GetDateStringForMonth(_currentDate.Month);
            }
        }

        public bool SelectBetweenTWoDatesMode
        {
            get { return _selectBetweenTwoDatesMode; }
            set { _selectBetweenTwoDatesMode = value; }
        }

        public CalendarView()
        {
            InitializeComponent();
            _dayOfWeekLabels = new DayOfWeekLabel[]
            {
                SundayLabel,
                MondayLabel,
                TuesdayLabel,
                WednesdayLabel,
                ThursdayLabel,
                FridayLabel,
                SaturdayLabel
            };
            _dayOfWeekWithSelectedDayInFront = _dayOfWeekLabels;

            InitCells();
        }

        void InitCells()
        {
            var count = 0;
            for (int i = 0; i < _cells.Length / 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    _cells[count] = new CalendarCell();
                    _cells[count].Index = count;
                    Grid.SetRow(_cells[count], i);
                    Grid.SetColumn(_cells[count], j);
                    MonthGrid.Children.Add(_cells[count]);
                    count++;
                }
            }
        }

        void SetupCellsForDate(DateTime date)
        {
            if (date.Day > 1)
                date = new DateTime(date.Year, date.Month, 1);

            var day = 1;
            var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            var firstDayIndex = GetFirstDayCalendarIndex(date.DayOfWeek);
            DateTime prevMonth = date.AddMonths(-1);
            DateTime nextMonth = date.AddMonths(1);
            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i].Tapped -= Handle_Tapped;

                if (i < firstDayIndex)
                {
                    var newDate = new DateTime(prevMonth.Year, 
                                               prevMonth.Month, 
                                               DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month) - (firstDayIndex-1)+i);
                    _cells[i].Date = newDate;
                    _cells[i].BackgroundColor = Color.Silver;
                    _cells[i].IsEnabled = false;
                }
                else if(i > (daysInMonth - 1 + firstDayIndex))
                {

                    var newDate = new DateTime(nextMonth.Year,
                                               nextMonth.Month,
                                               day-daysInMonth);
                    _cells[i].Date = newDate;
                    _cells[i].BackgroundColor = Color.Silver;
                    _cells[i].IsEnabled = false;
                    day++;
                }
                else
                {
                    _cells[i].Date = new DateTime(date.Year, date.Month, day);
                    _cells[i].Tapped += Handle_Tapped;
                    day++;
                }

            }
        }

        DateTime _lowerSelected;
        DateTime _higherSelected;

        void Handle_Tapped(CalendarCellSelectedArgs args)
        {
        }

        void StartOfWeekChanged()
        {
            if(StartOfWeek == DayOfWeek.Sunday)
            {
                ForEach(_dayOfWeekLabels, x => x.Position = _dayOfWeekLabels.IndexOf(x));
            }
            else
            {
                var start = (int)StartOfWeek;
                _dayOfWeekWithSelectedDayInFront = new DayOfWeekLabel[7];
                Array.Copy(_dayOfWeekLabels,
                           start,
                           _dayOfWeekWithSelectedDayInFront,
                           0,
                           _dayOfWeekLabels.Length - start);

                Array.Copy(_dayOfWeekLabels,
                            0,
                            _dayOfWeekWithSelectedDayInFront,
                            _dayOfWeekLabels.Length - start,
                           start);
                _startPos = start;
                ForEach(_dayOfWeekLabels, x => x.Position = _dayOfWeekWithSelectedDayInFront.IndexOf(x));
            }

            SetupCellsForDate(_currentDate);
        }

        /// <summary>
        /// Gets the index in the calendar where the first day of the month should
        /// appear. If Sunday is the user-defined first day of the week, and this
        /// month begins on a Monday, this method will return 1. If this month begins
        /// on a sunday, this method returns 0.
        /// </summary>
        /// <returns>The first day calendar index.</returns>
        /// <param name="startDayOfWeek">Start day of week.</param>
        int GetFirstDayCalendarIndex(DayOfWeek startDayOfWeek)
        {
            var startDay = (int)startDayOfWeek;
            return _dayOfWeekWithSelectedDayInFront.IndexOf(_dayOfWeekLabels[startDay]);
        }

        void SetDayOfWeekGridPosition(Label label, int pos)
        {
            Grid.SetColumn(label, pos);
        }

        public string[] GetLabelsByPosition()
        {
            return _dayOfWeekLabels
                .OrderBy(d => Grid.GetColumn(d))
                .Select(d => d.Text)
                .ToArray();
        }

        public static void ForEach<T>(T[] array, Action<T> action)
        {
            foreach(T item in array)
            {
                action(item);
            }
        }

        void SetupGrid(DateTime date)
        {
            //MonthGrid.Children.
        }

        void GoBackOneMonth(object sender, ClickedEventArgs args)
        {
            CurrentDate = _currentDate.AddMonths(-1);
        }

        void GoForwardOneMonth(object sender, ClickedEventArgs args)
        {
            CurrentDate = _currentDate.AddMonths(1);
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
        }
    }

    public sealed class DateHelper
    {
        public static string GetDateStringForMonth(int month)
        {
            string monthString;
            switch (month)
            {
                case 1:
                    monthString = "January";
                    break;
                case 2:
                    monthString = "February";
                    break;
                case 3:
                    monthString = "March";
                    break;
                case 4:
                    monthString = "April";
                    break;
                case 5:
                    monthString = "May";
                    break;
                case 6:
                    monthString = "June";
                    break;
                case 7:
                    monthString = "July";
                    break;
                case 8:
                    monthString = "August";
                    break;
                case 9:
                    monthString = "September";
                    break;
                case 10:
                    monthString = "October";
                    break;
                case 11:
                    monthString = "November";
                    break;
                case 12:
                    monthString = "December";
                    break;
                default:
                    monthString = string.Empty;
                    break;
            }
            return monthString;
        }
    }
}
