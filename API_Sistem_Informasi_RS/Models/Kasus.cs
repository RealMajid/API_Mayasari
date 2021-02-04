using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API_Sistem_Informasi_RS.Models
{
    public static class Kasus
    {
        public enum StatusKasus
        {
            Registered = 1,
            Queuing,
            Checkup,
            Closed
        }
    }
}