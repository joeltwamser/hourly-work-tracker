using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace hourlyWorkTracker.Models
{
    public class ApplicationBehavior : INotifyPropertyChanged, IDataErrorInfo
    {
        public ApplicationBehavior(Color rectangle_fill, Color ticker_foreground, Color button_background,
            Color button_text_foreground, Color grid_background, double opacity, double hourly_wage, bool hourly_wage_changed,
            double total_money_made, bool total_money_made_changed)
        {
            _rectangle_fill = rectangle_fill;
            _ticker_foreground = ticker_foreground;
            _button_background = button_background;
            _button_text_foreground = button_text_foreground;
            _grid_background = grid_background;
            _opacity = opacity;
            _hourly_wage = hourly_wage;
            _hourly_wage_changed = hourly_wage_changed;
            _total_money_made = total_money_made;
            _total_money_made_changed = total_money_made_changed;
        }

        private Color _rectangle_fill;
        private Color _ticker_foreground;
        private Color _button_background;
        private Color _button_text_foreground;
        private Color _grid_background;
        private double _opacity;
        private double _hourly_wage;
        private bool _hourly_wage_changed;
        private double _total_money_made;
        private bool _total_money_made_changed;
        private double _money_made_this_session;

        public Color RectangleFill
        {
            get { return _rectangle_fill; }
            set
            {
                _rectangle_fill = value;
                OnPropertyChanged("RectangleFill");
            }
        }

        public Color TickerForeground
        {
            get { return _ticker_foreground; }
            set
            {
                _ticker_foreground = value;
                OnPropertyChanged("TickerForeground");
            }
        }

        public Color ButtonBackground
        {
            get { return _button_background; }
            set
            {
                _button_background = value;
                OnPropertyChanged("ButtonBackground");
            }
        }

        public Color ButtonTextForeground
        {
            get { return _button_text_foreground; }
            set
            {
                _button_text_foreground = value;
                OnPropertyChanged("ButtonTextForeground");
            }
        }

        public Color GridBackground
        {
            get { return _grid_background; }
            set
            {
                _grid_background = value;
                OnPropertyChanged("GridBackground");
            }
        }

        public double Opacity
        {
            get { return _opacity; }
            set
            {
                _opacity = value;
                OnPropertyChanged("Opacity");
            }
        }

        public double HourlyWage
        {
            get { return _hourly_wage; }
            set
            {
                //Need to put in a condition to make sure that hourly wage has been entered in the correct format.  Not sure where that is done, probably not here.
                _hourly_wage = value;
                OnPropertyChanged("HourlyWage");
            }
        }

        public bool HourlyWageChanged
        {
            get { return _hourly_wage_changed; }
            set
            {
                _hourly_wage_changed = value;
                OnPropertyChanged("HourlyWageChanged");
            }
        }

        public double TotalMoneyMade
        {
            get { return _total_money_made; }
            set
            {
                _total_money_made = value;
                OnPropertyChanged("TotalMoneyMade");
            }
        }

        public bool TotalMoneyMadeChanged
        {
            get { return _total_money_made_changed; }
            set
            {
                _total_money_made_changed = value;
                OnPropertyChanged("TotalMoneyMadeChanged");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public string Error
        {
            get
            {
                return String.Empty;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                string result = String.Empty;

                switch(propertyName)
                {
                    case "HourlyWage":

                        break;
                }
                return result;
            }
        }
    }
}
