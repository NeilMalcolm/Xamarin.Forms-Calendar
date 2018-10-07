using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace XamarinFormsCalendar.View
{
    public partial class CalendarCell : ContentView
    {
        DateTime _date;
        Color _defaultBackgroundColor = Color.White;

        public int Index { get; set; }

        public event Action<CalendarCellSelectedArgs> Tapped;

        public static readonly BindableProperty DayProperty =
            BindableProperty.Create("Day",
                                    typeof(string),
                                    typeof(CalendarCell),
                                    string.Empty,
                                    BindingMode.TwoWay,
                                    null,
                                    CellTextChanged);

        public string Day
        {
            get => (string)GetValue(DayProperty);
            set => SetValue(DayProperty, value);
        }

        public static readonly BindableProperty SelectedProperty =
            BindableProperty.Create("Selected",
                                    typeof(bool),
                                    typeof(CalendarCell),
                                    false,
                                    BindingMode.TwoWay,
                                    null,
                                    OnSelected);

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        public static readonly BindableProperty SelectedColorProperty =
            BindableProperty.Create("SelectedColor",
                                    typeof(Color),
                                    typeof(CalendarCell),
                                    Color.SkyBlue,
                                    BindingMode.TwoWay);

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public DateTime Date
        {
            get { return _date; }
            set 
            {
                _date = value;
                Day = value.Day + "";
            }
        }

        public CalendarCell()
        {
            InitializeComponent();
            CellLabel.Text = Day;
            BackgroundColor = _defaultBackgroundColor;
        }

        static void CellTextChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var cell = (bindable as CalendarCell);
            cell.CellLabel.Text = (string)newValue;
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
