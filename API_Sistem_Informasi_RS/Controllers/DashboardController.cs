using API_Sistem_Informasi_RS.Models.Response;
using API_Sistem_Informasi_RS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Sistem_Informasi_RS.Controllers
{
    [Authorize(Roles = "Super Admin,Eksekutif")]
    public class DashboardController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();

        [Route("api/dashboard/klinik")]
        [ResponseType(typeof(APIListResponse<VwAggregateChart>))]
        public async Task<IHttpActionResult> GetAggregateKlinik()
        {
            var result = await db.Database.SqlQuery<VwAggregateChart>(
                $@"select ID_KLINIK Id, SUBSTRING(NAMA_KLINIK,8, 50) Name, COUNT(ID_KLINIK) Value FROM VW_HISTORY_MEDIS_PASIEN
                    GROUP BY ID_KLINIK, NAMA_KLINIK"
                ).ToListAsync();

            var totalRecord = result.Count();

            return Ok(new APIListResponse<VwAggregateChart>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, 0));
        }

        [Route("api/dashboard/pasienperminggu")]
        [ResponseType(typeof(APIListResponse<VwPasienWeekly>))]
        public async Task<IHttpActionResult> GetPasienPerWeek()
        {
            var result = await db.Database.SqlQuery<VwPasienWeekly>(
                $@"  SELECT IdHari, Hari AS Hari,   
                [Registered], [Queuing], [Checkup], [Closed]  
                FROM  
                (SELECT DATEPART(dw,TGL_MASUK) IdHari, FORMAT(TGL_MASUK, 'dddd', 'id') Hari, STATUS_KASUS, 1 AS COUNT_STATUS_KASUS
                    FROM VW_REGISTER_KASUS) AS SourceTable  
                PIVOT  
                (  
                COUNT(COUNT_STATUS_KASUS)  
                FOR STATUS_KASUS IN ([Registered], [Queuing], [Checkup], [Closed])  
                ) AS PivotTable"
                ).ToListAsync();

            foreach (var item in result)
            {
                item.Registered = item.Queuing + item.Checkup + item.Closed;
            }

            var totalRecord = result.Count();

            return Ok(new APIListResponse<VwPasienWeekly>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result.OrderBy(x => x.IdHari), totalRecord, 0));
        }

        [Route("api/dashboard/aggregatestatus")]
        [ResponseType(typeof(VwAggregateStatusPasien))]
        public async Task<IHttpActionResult> GetAggregateStatusPasien()
        {
            var result = new VwAggregateStatusPasien();
            result = await db.Database.SqlQuery<VwAggregateStatusPasien>($@"
                SELECT 'STATUS_KASUS' AS Jenis,   
                [Registered], [Queuing], [Checkup], [Closed]  
                FROM  
                (SELECT STATUS_KASUS, 1 AS COUNT_STATUS_KASUS
                    FROM VW_REGISTER_KASUS) AS SourceTable  
                PIVOT  
                (  
                COUNT(COUNT_STATUS_KASUS)  
                FOR STATUS_KASUS IN ([Registered], [Queuing], [Checkup], [Closed])  
                ) AS PivotTable
            ").FirstOrDefaultAsync();

            return Ok(result);
        }
    }
}
