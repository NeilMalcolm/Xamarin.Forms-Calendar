using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinFormsCalendar
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            var labelsInOrder = Calendar.GetLabelsByPosition();
            foreach (var label in labelsInOrder)
            {
                Console.WriteLine(label);
            }

            Calendar.CurrentDate = DateTime.Now.AddMonths(1);
            //Calendar.StartOfWeek = DayOfWeek.Sunday;
            //labelsInOrder = Calendar.GetLabelsByPosition();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            var button = (Button)sender;
            if (button.Equals(Sun))
            {
                Calendar.StartOfWeek = DayOfWeek.Sunday;
            }
            else if (button.Equals(Mon))
            {
                Calendar.StartOfWeek = DayOfWeek.Monday;
            }
            else if(button.Equals(Tue))
            {
                Calendar.StartOfWeek = DayOfWeek.Tuesday;
            }
            else if (button.Equals(Wed))
            {
                Calendar.StartOfWeek = DayOfWeek.Wednesday;
            }
            else if (button.Equals(Thu))
            {
                Calendar.StartOfWeek = DayOfWeek.Thursday;
            }
            else if (button.Equals(Fri))
            {
                Calendar.StartOfWeek = DayOfWeek.Friday;
            }
            else if (button.Equals(Sat))
            {
                Calendar.StartOfWeek = DayOfWeek.Saturday;
            }

            var labelsInOrder = Calendar.GetLabelsByPosition();
            foreach(var label in labelsInOrder)
            {
                Console.WriteLine(label);
            }
        }
    }
}
