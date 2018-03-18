using System;
using System.Reactive.Linq;
using Slate.Core.Controls.DataGrid;

namespace Slate.FrontEnd.OpenGl
{
    public class Row
    {
        public string Property1 { get; }
        public int Property2 { get; private set; }
        public bool Property3 => Property2 % 2 == 0;

        public Row(long n)
        {
            Property1 = $"Row #{n}";
            Property2 = (int)n;
        }

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
            var testSource = Observable.Interval(TimeSpan.FromMilliseconds(1)).Select(t => new Row(t));

            var testSlate = new EventGrid<Row>(testSource, ColumnFactory.FromProperties<Row>());

            using (var game = new FrontEnd(testSlate))
                game.Run();
        }
    }    
}
