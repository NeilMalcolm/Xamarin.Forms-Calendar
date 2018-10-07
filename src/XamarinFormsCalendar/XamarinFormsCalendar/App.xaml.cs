using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinFormsCalendar.View;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XamarinFormsCalendar
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            //var mon = DayOfWeek.Monday;
            //var tues = DayOfWeek.Tuesday;
            //var wed = DayOfWeek.Wednesday;
            //var thur = DayOfWeek.Thursday;
            //var fri = DayOfWeek.Friday;
            //var sat = DayOfWeek.Saturday;
            //var sun = DayOfWeek.Sunday;

            //Console.WriteLine(mon);
            //Console.WriteLine(tues);
            //Console.WriteLine(wed);
            //Console.WriteLine(thur);
            //Console.WriteLine(fri);
            //Console.WriteLine(sat);
            //Console.WriteLine(sun);

            //Console.WriteLine((int)mon);
            //Console.WriteLine((int)tues);
            //Console.WriteLine((int)wed);
            //Console.WriteLine((int)thur);
            //Console.WriteLine((int)fri);
            //Console.WriteLine((int)sat);
            //Console.WriteLine((int)sun);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
