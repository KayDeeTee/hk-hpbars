using HPBar.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HPBar
{
    class ResourceLoader
    {
        public static byte[] GetBackgroundImage()
        {
            return Properties.Resources.bg;
        }

        public static byte[] GetForegroundImage()
        {
            return Properties.Resources.fg;
        }

        public static byte[] GetOutlineImage()
        {
            return Properties.Resources.outline;
        }
    }
}
