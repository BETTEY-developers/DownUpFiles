using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace DownUpFilesWindows.Helper
{
    internal class ResourceHelper
    {
        internal static string GetResourceString(string Key) 
            => new ResourceLoader().GetString(Key);
    }

}
