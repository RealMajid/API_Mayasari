using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Response
{
    [DataContract]
    public class APIResponse
    {
        public APIResponse(bool error, string code, string message)
        {
            this.Error = error;
            this.Code = code;
            this.Message = message;
        }

        [DataMember]
        public bool Error { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Version { get; set; } = "1.0.0";
    }
}