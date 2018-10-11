using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using System.Linq;
using XamarinFormsCalendar.Controls;
using Xamarin.Forms.Internals;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using XamarinFormsCalendar.Enums;
using System.Windows.Input;

namespace XamarinFormsCalendar.View
{
    public class CalendarRangeSelectedArgs : EventArgs
    {
        public CalendarCell StartCell { get; set; }
        public CalendarCell EndCell { get; set; }

        public CalendarRangeSelectedArgs(CalendarCell start, CalendarCell end)
        {
            StartCell = start;
            EndCell = end;
        }
    }

    public partial class CalendarView : ContentView, INotifyPropertyChanged
    {
        CalendarCell _highlightedCell;
        CalendarCell[] _cells = new CalendarCell[42];

        CalendarCell _startCell;    // Represents the earliest cell selected in range
        CalendarCell _endCell;      // Represents the latest cell selected in range

        DayOfWeekLabel[] _dayOfWeekLabels;
        int _startPos = 0;
        DayOfWeekLabel[] _dayOfWeekWithSelectedDayInFront = new DayOfWeekLabel[7];
        List<DateTime> _selectedDates;
        DayOfWeek _startOfWeek = DayOfWeek.Sunday;
        DateTime _currentDate = DateTime.Now;
        DateTime? _selectedDate;

        public event Action<CalendarCellSelectedArgs> CellSelected;
        public event Action<CalendarRangeSelectedArgs> RangeEndCellSelected;

        /// <summary>
        /// Gets or sets the day at the start of week. Default Sunday.
        /// </summary>
        public DayOfWeek StartOfWeek 
        {
            get => _startOfWeek;
            set 
            {
                _startOfWeek = value;
                StartOfWeekChanged();
            } 
        }

        /// <summary>
        /// Gets or sets the current date being displayed by the calendar.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        /// <value>The selected date.</value>
        public DateTime? SelectedDate
        {
            get => _selectedDate ?? _currentDate;
            set
            {
                if (_selectedDate == null)
                    _selectedDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected dates.
        /// </summary>
        /// <value>The selected dates.</value>
        public List<DateTime> SelectedDates
        {
            get { return _selectedDates; }
            set { _selectedDates = value; }
        }

        /// <summary>
        /// Gets the friendly current month string
        /// </summary>
        /// <value>The current month.</value>
        public string CurrentMonth 
        {
            get
            {
                return DateHelper.GetDateStringForMonth(_currentDate.Month);
            }
        }

#region Bindable Properties

        public static readonly BindableProperty SelectionModeProperty = 
            BindableProperty.Create("SelectionMode", 
                                    typeof(CalendarSelectionMode), 
                                    typeof(CalendarView), 
                                    CalendarSelectionMode.Single);

        /// <summary>
        /// Gets or sets the selection mode for the calendar view.
        /// </summary>
        public CalendarSelectionMode SelectionMode
        {
            get => (CalendarSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        public static readonly BindableProperty CellSelectedCommandProperty =
            BindableProperty.Create("CellSelectedCommand",
                                    typeof(ICommand),
                                    typeof(CalendarView),
                                    null);


        public ICommand CellSelectedCommand
        {
            get => (ICommand)GetValue(CellSelectedCommandProperty);
            set => SetValue(CellSelectedCommandProperty, value);
        }


#endregion

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
            //Container.FadeTo(1, 1000, Easing.CubicOut);
        }

        /// <summary>
        /// Initialises the cells used for the Calendar View
        /// </summary>
        void InitCells()
        {
            var count = 0;
            for (int i = 0; i < _cells.Length / 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    _cells[count] = new CalendarCell
                    {
                        Index = count
                    };
                    Grid.SetRow(_cells[count], i);
                    Grid.SetColumn(_cells[count], j);
                    MonthGrid.Children.Add(_cells[count]);
                    _cells[count].Tapped += OnCellTapped;
                    count++;
                }
            }
        }

        /// <summary>
        /// Sets up the calendar cells for the given date. Creates all cells from the 1st of month
        /// until the end. And creates cells for visible days preceeding and following the given month.
        /// </summary>
        /// <param name="date">Date.</param>
        void SetupCellsForDate(DateTime date)
        {
            // Ensure our iteration begins on the 1st day of month.
            if (date.Day > 1)
                date = new DateTime(date.Year, date.Month, 1);

            var day = 1;
            var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            var firstDayIndex = GetFirstDayCalendarIndex(date.DayOfWeek);
            DateTime prevMonth = date.AddMonths(-1);
            DateTime nextMonth = date.AddMonths(1);
            for (int i = 0; i < _cells.Length; i++)
            {
                _cells[i].Selected = false;
                if (i < firstDayIndex)
                {
                    // If the current cell preceeds the 1st of the month.
                    var newDate = new DateTime(prevMonth.Year,
                                               prevMonth.Month,
                                               DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month) - (firstDayIndex - 1) + i);
                    _cells[i].Date = newDate;
                    _cells[i].SetOutOfMonthState(true);
                }
                else if (i > (daysInMonth - 1 + firstDayIndex))
                {
                    // If the cell is after the end of the month.
                    var newDate = new DateTime(nextMonth.Year,
                                               nextMonth.Month,
                                               day - daysInMonth);
                    _cells[i].Date = newDate;
                    _cells[i].SetOutOfMonthState(true);
                    day++;
                }
                else
                {
                    // If the current cell is within the current month.
                    var newDate = new DateTime(date.Year, date.Month, day);
                    _cells[i].Date = newDate;
                    _cells[i].SetOutOfMonthState(false);
                    //_cells[i].Tapped += OnCellTapped;
                    if (newDate == _selectedDate)
                    {
                        _cells[i].Selected = true;
                        _highlightedCell = _cells[i];
                    }
                    day++;
                }
            }
        }

        void OnCellTapped(CalendarCellSelectedArgs args)
        {
            if(!args.Cell.IsOutOfMonth)
            {
                //AddEvent(args.Cell, "New Event", DateTime.Now, DateTime.Now.AddHours(1));
                if (SelectionMode == CalendarSelectionMode.Single)
                    HandleCellTappedInSingleSelectionMode(args);
                else
                    HandleCellTappedInRangeSelectionMode(args);
            }
            else
            {
                HandleOutOfMonthCellTappedInSingleSelectionMode(args);
            }
        }

        /// <summary>
        /// Handles the out of month cell tapped in single selection mode.
        /// </summary>
        async void HandleOutOfMonthCellTappedInSingleSelectionMode(CalendarCellSelectedArgs args)
        {
            // TODO: change current month to new month
            // change selected date to selected date

            if (_highlightedCell != null)
            {
                _highlightedCell.Selected = false;
                _highlightedCell = null;
            }
            _selectedDate = args.Date;
            _currentDate = _selectedDate.Value;
            OnPropertyChanged("CalendarMonth");
            await Task.Run(() =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetupCellsForDate(args.Date);
                });
            });
            //DoCellSelection(_currentDate);
        }

        /// <summary>
        /// Handles the cell tapped event when in single selection mode.
        /// </summary>
        /// <param name="args">Arguments of the cell tapped event</param>
        void HandleCellTappedInSingleSelectionMode(CalendarCellSelectedArgs args)
        {
            var cell = args.Cell;
            if (cell == _highlightedCell)
            {
                // de-select cell
                _highlightedCell.Selected = false;
                _highlightedCell = null;
                _selectedDate = null;
            }
            else
            {
                // select cell
                if (_highlightedCell != null)
                    _highlightedCell.Selected = false;

                _highlightedCell = cell;
                _highlightedCell.Selected = true;
                _selectedDate = cell.Date;
                CellSelected?.Invoke(new CalendarCellSelectedArgs(cell, cell.Date ));
                CellSelectedCommand?.Execute(cell);
            }
        }

        /// <summary>
        /// Handles the cell tapped event when in range selection mode.
        /// </summary>
        /// <param name="args">Arguments of the cell tapped event</param>
        void HandleCellTappedInRangeSelectionMode(CalendarCellSelectedArgs args)
        {
            //var selectedCell = args.Cell;
            //if(_startCell is null)
            //{
            //    // If we haven't selected any cells yet.
            //    _startCell = selectedCell;
            //    _startCell.Selected = true;
            //}
            //else if(_endCell is null)
            //{
            //    // If we have selected the first cell, but not a second
            //    if(_startCell.Date < selectedCell.Date)
            //    {
            //        _endCell = selectedCell;
            //        _endCell.Selected = true;
            //    }
            //    else if(_startCell.Date > selectedCell.Date)
            //    {
            //        _endCell = _startCell;
            //        _startCell = selectedCell;
            //        _startCell.Selected = true;
            //        SelectAllCellsBetweenRange(_startCell, _endCell);
            //    }
            //    else if(_startCell.Date == selectedCell.Date)
            //    {
            //        _startCell.Selected = false;
            //        _startCell = null;
            //    }
            //}
            //else if(_startCell != null && _endCell != null)
            //{
            //    // If we have selected two cells and are selecting more

            //}
        }

        void SelectAllCellsBetweenRange(CalendarCell start, CalendarCell end)
        {
            if (start.Date > end.Date || start.Date == end.Date)
                return;

            for (int i = start.Index; i <= end.Index; i++)
            {
                if (!_cells[i].Selected)
                    _cells[i].Selected = true;
            }
        }

        void DeselectAllCellsBetweenRange(CalendarCell start, CalendarCell end)
        {
            if (start.Date > end.Date || start.Date == end.Date)
                return;

            for (int i = start.Index; i <= end.Index; i++)
            {
                if(_cells[i].Selected)
                    _cells[i].Selected = false;
            }
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

        void CalendarDateSelected(object sender, Xamarin.Forms.DateChangedEventArgs e)
        {
            if (e.OldDate == e.NewDate)
                return;

            CurrentDate = e.NewDate;

            if(_highlightedCell != null)
            {
                _highlightedCell.Selected = false;
            }

            var cellIndex = GetCellIndexFromDate(e.NewDate);
            _highlightedCell = _cells[cellIndex];
            _highlightedCell.Selected = true;
        }


        /// <summary>
        /// Gets the position of the cell that represents the given date
        /// </summary>
        /// <returns>The cell index representing the date</returns>
        /// <param name="date">Date</param>
        int GetCellIndexFromDate(DateTime date)
        {
            var startIndex = GetFirstDayCalendarIndex(new DateTime(date.Year, date.Month, 1).DayOfWeek);
            return _currentDate.Day - 1 + startIndex;
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
