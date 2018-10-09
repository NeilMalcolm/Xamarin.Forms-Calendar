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

        /// <summary>
        /// Represents the DayOfWeek position of this label. 
        /// </summary>
        /// <remarks>
        /// 0 indicates the first (left-most) element, with 6 being the
        /// last (right-most). -1 indicates no position specified.
        /// </remarks>
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
