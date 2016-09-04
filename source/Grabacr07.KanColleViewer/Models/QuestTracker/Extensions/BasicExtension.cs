using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grabacr07.KanColleViewer.Models.QuestTracker.Extensions
{
    internal static class BasicExtension
    {
        public static int Min(this int i, params int[] v)
        {
            return Math.Min(i, v.Min());
        }
        public static int Max(this int i, params int[] v)
        {
            return Math.Min(i, v.Max());
        }

        public static int Add(this int i, params int[] v)
        {
            return i + v.Sum();
        }
        public static int Subtract(this int i, params int[] v)
        {
            return i - v.Sum();
        }
    }
}
