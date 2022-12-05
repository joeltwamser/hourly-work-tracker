using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace hourlyWorkTracker.Services
{
    public interface IWindowService
    {
        void ShowWindow(object? view_model);
        void CloseWindow(object? w);
    }

    public class WindowService : IWindowService
    {
        public void ShowWindow(object? view_model)
        {
            Window w = new();
            w.Content = view_model;
            w.Show();
        }

        public void CloseWindow(object? w)
        {
            if (w is Window window && window != null)
            {
                window.Close();
            }
        }
    }
}
