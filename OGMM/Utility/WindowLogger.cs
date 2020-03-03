﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace OGMM.Utility
{
    public static class WindowLogger
    {
        public static VirtualizingStackPanel LogPanel { get; private set; }

        public static void SetLogPanel(VirtualizingStackPanel logPanel)
        {
            LogPanel = logPanel;
        }

        public static void Log(string message)
        {
            //Done this way because an issue with threading.
            //See here for an explanation: https://stackoverflow.com/questions/9732709/the-calling-thread-cannot-access-this-object-because-a-different-thread-owns-it
            LogPanel?.Dispatcher?.Invoke(() =>
            {
                LogPanel.Children.Add(new TextBlock
                {
                    Text = message,
                    Margin = new Thickness(5, 0, 0, 0)
                });
            });
        }
    }
}
