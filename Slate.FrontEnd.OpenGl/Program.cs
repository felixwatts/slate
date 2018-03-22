﻿using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Slate.Core.Controls.DataGrid;

namespace Slate.FrontEnd.OpenGl
{
    public class Row : INotifyPropertyChanged
    {
        private int _property4;

        public string Property1 { get; }
        public int Property2 { get; private set; }
        public bool Property3 => Property2 % 2 == 0;

        public int Property4 
        { 
            get => _property4; 
            private set 
            {
                _property4 = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property4)));
            }  
        }

        public Row(long n)
        {
            Property1 = $"Row #{n}";
            Property2 = (int)n;

            Observable.Interval(TimeSpan.FromMilliseconds(50)).Subscribe(l =>
            {
                Property4 = (int)l;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Method1()
        {
            Property2 = 0;
        }
    }

    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var testSource = new Subject<Row>();
            Task.Factory.StartNew(() =>
            {
                for(var n = 0;; n++)
                {
                    testSource.OnNext(new Row(n));
                    Thread.Sleep(1000);
                }
            });

            //var testSource = Observable.Interval(TimeSpan.FromMilliseconds(1)).Select(t => new Row(t));

            var testSlate = new EventGrid<Row>(testSource, ColumnFactory.FromProperties<Row>());

            using (var game = new FrontEnd(testSlate))
                game.Run();
        }
    }    
}
