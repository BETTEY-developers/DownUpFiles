using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUpFilesWindows.Helper
{
    public class PromiseHelper
    {
        public static Task AsPromise(Func<Task> t)
        {
            return t();
        }
        public static Task<T> AsPromise<T>(Func<Task<T>> t)
        {
            return t();
        }
    }
}
