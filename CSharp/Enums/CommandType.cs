using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CruiseControl.Enums
{
    public static class CommandType
    {
        public const string Fire = "fire";
        public const string Move = "move:{0}";
        public const string LoadCountermeasures = "load_countermeasures";
        public const string Repair = "repair";
        public const string Truce = "truce:{0}";
        public const string AcceptTruce = "accept_truce:{0}";
    }
}