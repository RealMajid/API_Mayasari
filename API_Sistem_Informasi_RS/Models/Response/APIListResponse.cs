using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace API_Sistem_Informasi_RS.Models.Response
{
    public class APIListResponse<T> : APIResponse
    {
        public APIListResponse(bool error, string code, string message, IEnumerable<T> data, int totalRecord = 0, int totalPage = 1) : base(error, code, message)
        {
            this.Error = error;
            this.Code = code;
            this.Message = message;
            this.Data = data;
            this.TotalRecord = totalRecord;
            this.MaxOffset = totalPage - 1;
        }
        [DataMember]
        public IEnumerable<T> Data { get; set; } = new List<T>();

        [DataMember]
        public int TotalRecord { get; set; }

        [DataMember]
        public int MaxOffset { get; set; }
    }
}