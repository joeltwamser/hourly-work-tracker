using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace hourlyWorkTracker.Models
{
    public class ApplicationBehavior : INotifyPropertyChanged
    {
        public ApplicationBehavior(SolidColorBrush rectangle_fill, SolidColorBrush textblock_foreground,
            SolidColorBrush button_background, SolidColorBrush grid_background, FontFamily textblock_fontfamily,
            double opacity)
        {
            _rectangle_fill = rectangle_fill;
            _textblock_foreground = textblock_foreground;
            _button_background = button_background;
            _grid_background = grid_background;
            _textblock_fontfamily = textblock_fontfamily;
            _opacity = opacity;
        }

        private SolidColorBrush _rectangle_fill;
        private SolidColorBrush _textblock_foreground;
        private SolidColorBrush _button_background;
        private SolidColorBrush _grid_background;
        private FontFamily _textblock_fontfamily;
        private double _opacity;

        public SolidColorBrush RectangleFill
        {
            get { return _rectangle_fill; }
            set
            {
                _rectangle_fill = value;
                OnPropertyChanged("RectangleFill");
            }
        }

        public SolidColorBrush TextBlockForeground
        {
            get { return _textblock_foreground; }
            set
            {
                _textblock_foreground = value;
                OnPropertyChanged("TextBlockForeground");
            }
        }

        public SolidColorBrush ButtonBackground
        {
            get { return _button_background; }
            set
            {
                _button_background = value;
                OnPropertyChanged("ButtonBackground");
            }
        }

        public SolidColorBrush GridBackground
        {
            get { return _grid_background; }
            set
            {
                _grid_background = value;
                OnPropertyChanged("GridBackground");
            }
        }

        public FontFamily TextBlockFontFamily
        {
            get { return _textblock_fontfamily; }
            set
            {
                _textblock_fontfamily = value;
                OnPropertyChanged("TextBlockFontFamily");
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string p)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }
    }
}
