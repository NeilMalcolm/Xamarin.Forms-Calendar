using System;
using Xamarin.Forms;

namespace XamarinFormsCalendar.Controls
{
    public class DayOfWeekLabel : Label
    {
        public static readonly BindableProperty PositionProperty = 
            BindableProperty.Create("Position", 
                                    typeof(int), 
                                    typeof(DayOfWeekLabel), 
                                    -1);

        public int Position
        {
            get { return (int)GetValue(PositionProperty); }
            set {  SetValue(PositionProperty, value); }
        }

        public DayOfWeekLabel() : base()
        {

        }
    }
}
