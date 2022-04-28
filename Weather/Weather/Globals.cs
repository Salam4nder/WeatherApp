using System;
using System.Collections.Generic;
using System.Text;

namespace Weather
{
    public class Globals
    {
        #region Lazy implementation
        Globals() { } // just to avoid any direct instantiations

        private static Lazy<Globals> _instance = new Lazy<Globals>(() => new Globals());
        public static Globals Data => _instance.Value;
        #endregion

        #region Data implementation 
        // accessible as Globals.Data.xx such as Globasl.Data.Item1, Globals.Data.Item2 in this example
        public string Item1 { get; set;}
        public double Item2 { get; set;}
        #endregion
    }
}
