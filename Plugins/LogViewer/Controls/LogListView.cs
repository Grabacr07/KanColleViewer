using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Grabacr07.KanColleViewer.Plugins.ViewModels;

namespace Grabacr07.KanColleViewer.Plugins.Controls
{
    public class LogListView
    {
        public static readonly DependencyProperty LogSourceProperty =
            DependencyProperty.RegisterAttached("LogSource",
                                                typeof(LogItemCollection), typeof(LogListView),
                                                new FrameworkPropertyMetadata(null, OnLogSourceChanged));

        public static LogItemCollection GetLogSource(DependencyObject d)
        {
            return (LogItemCollection)d.GetValue(LogSourceProperty);
        }

        public static void SetLogSource(DependencyObject d, LogItemCollection value)
        {
            d.SetValue(LogSourceProperty, value);
        }

        private static void OnLogSourceChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            ListView listView = d as ListView;
            LogItemCollection collection = e.NewValue as LogItemCollection;

            listView.ItemsSource = collection;
            GridView gridView = listView.View as GridView;
            int count = 0;
            gridView.Columns.Clear();
            foreach (var col in collection.Columns)
            {
                var cell = new FrameworkElementFactory(typeof(TextBlock));
                cell.SetBinding(TextBlock.TextProperty, new Binding(string.Format("[{0}]", count++)));
                cell.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 4, 0, 4));

                gridView.Columns.Add(
                    new GridViewColumn
                    {
                        Header = new TextBlock { Text = col, FontSize = 11, Margin = new Thickness(5, 4, 5, 4) },
                        CellTemplate = new DataTemplate
                        {
                            DataType = typeof(LogItemCollection),
                            VisualTree = cell,
                        }
                    });
            }
        }
    }
}
