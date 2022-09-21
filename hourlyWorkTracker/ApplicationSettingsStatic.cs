using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hourlyWorkTracker
{
    //Notes: This static class is used to store all of the main values that persist across the application between individual uses (closing and reopening the application for later use.)
    //This may be bad practice as it feels like I'm using global memory.  I have also now set up events to be raised when certain values are changed from anywhere in my application.
    //Again I feel like this may be bad practice since it feels like I'm using this class as one giant global variable but I'm not sure.
    internal static class ApplicationSettingsStatic
    {
        //private fields
        private static double _hourly_wage;
        private static double _total_money;
        public static double OpacitySliderValue
        { get; set; }

        public static double MainWindowOpacity
        { get; set; }

        public static double HourlyWage
        {
            get
            {
                return _hourly_wage;
            }
            set
            {
                _hourly_wage = value;
                HourlyWageChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static double TotalMoney
        {
            get
            {
                return _total_money;
            }
            set
            {
                _total_money = value;
                TotalMoneyChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static event EventHandler? HourlyWageChanged;
        public static event EventHandler? TotalMoneyChanged;
    }
}
