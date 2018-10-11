using System;
using Xamarin.Forms;

namespace XamarinFormsCalendar.View
{
    public partial class CalendarCell : ContentView
    {
        DateTime _date;
        Color _defaultBackgroundColor = Color.White;
        Color _defaultDisabledBackgroundColor = new Color(0.9, 0.9, 0.9);

        public event Action<CalendarCellSelectedArgs> Tapped;

        #region Bindable Properties

        public static readonly BindableProperty DayProperty =
            BindableProperty.Create("Day",
                                    typeof(string),
                                    typeof(CalendarCell),
                                    string.Empty,
                                    BindingMode.TwoWay,
                                    null,
                                    CellTextChanged);

        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create("Selected",
                                    typeof(bool),
                                    typeof(CalendarCell),
                                    false,
                                    BindingMode.TwoWay,
                                    null,
                                    OnSelected);


        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create("SelectedColor",
                                    typeof(Color),
                                    typeof(CalendarCell),
                                    Color.SkyBlue,
                                    BindingMode.TwoWay);

#endregion

        /// <summary>
        /// Gets or sets 
        /// </summary>
        /// <value>The day.</value>
        public string Day
        {
            get => (string)GetValue(DayProperty);
            set => SetValue(DayProperty, value);
        }

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        /// <summary>
        /// Gets or sets the color of the cell when selected
        /// </summary>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Gets or sets the date this cell is representing
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set 
            {
                _date = value;
                Day = value.Day + "";
            }
        }

        internal bool IsOutOfMonth
        {
            get; set;
        }

        /// <summary>
        /// The index in the Calendar that this grid represents
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; internal set; }

        public CalendarCell()
        {
            InitializeComponent();
            CellDateLabel.Text = Day;
            BackgroundColor = _defaultBackgroundColor;
        }

        static void CellTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var cell = (bindable as CalendarCell);
            cell.CellDateLabel.Text = (string)newValue;
            if (cell.BackgroundColor != cell._defaultBackgroundColor)
                cell.BackgroundColor = cell._defaultBackgroundColor;
        }

        static void OnSelected(BindableObject bindable, object oldValue, object newValue)
        {
            var cell = (bindable as CalendarCell);
            cell.Selected = (bool)newValue;
            if (cell.Selected)
                cell.BackgroundColor = cell.SelectedColor;
            else
                cell.BackgroundColor = cell._defaultBackgroundColor;
        }


        void OnTapped(object sender, System.EventArgs e)
        {
            Tapped?.Invoke(new CalendarCellSelectedArgs(this, _date));
        }

        public void SetOutOfMonthState(bool isEnabled)
        {
            //IsEnabled = isEnabled;
            IsOutOfMonth = isEnabled;
            BackgroundColor = isEnabled ? _defaultBackgroundColor : _defaultDisabledBackgroundColor;
        }

        //public void AddEvent(string title, DateTime start, DateTime end)
        //{
        //    ContentStack.Children.Add(new EventView
        //    {
        //        StartTime = start,
        //        EndTime = end,
        //        Title = title
        //    });
        //}
    }

    public class CalendarCellSelectedArgs : EventArgs
    {
        public CalendarCell Cell { get; set; }
        public DateTime Date { get; set; }

        public CalendarCellSelectedArgs(CalendarCell cell, DateTime date) : base()
        {
            Cell = cell;
            Date = date;
        }
    }
}
