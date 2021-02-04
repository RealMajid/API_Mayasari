using API_Sistem_Informasi_RS.Models.Response;
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
    [Authorize]
    [RoutePrefix("api/ref")]
    public class RefController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();

        [Route("klinik")]
        [ResponseType(typeof(AutoCompleteResponse))]
        public IHttpActionResult GetKLINIKs()
        {
            var result = db.Database.SqlQuery<AutoCompleteResponse>("SELECT ID_KLINIK Id, NAMA_KLINIK Value FROM KLINIK ORDER BY ID_KLINIK").ToList();
            if (result == null) return BadRequest("Data Klinik tidak ditemukan");

            return Ok(result);
        }

        [Route("obat")]
        [ResponseType(typeof(AutoCompleteResponse))]
        public IHttpActionResult GetOBATs(int idKlinik)
        {
            var result = db.OBATs.Where(x => x.ID_KLINIK == idKlinik)
                                 .Select(x => new AutoCompleteResponse { Id = x.ID_OBAT, Value = x.NAMA_OBAT, Name = x.GUNA_OBAT })
                                 .ToList();
            if (result == null) return BadRequest("Data Klinik tidak ditemukan");

            return Ok(result);
        }

        [Route("lab")]
        [ResponseType(typeof(AutoCompleteResponse))]
        public IHttpActionResult GetLABs(int idKlinik)
        {
            var result = db.LABs.Where(x => x.ID_KLINIK == idKlinik)
                                .Select(x => new AutoCompleteResponse { Id = x.ID_LABORAT, Value = x.NAMA_TES, Name = x.GUNA_TES })
                                .ToList();
            if (result == null) return BadRequest("Data Klinik tidak ditemukan");

            return Ok(result);
        }

        [Route("monitoringdokter")]
        [Authorize(Roles = "Super Admin,Eksekutif")]
        [ResponseType(typeof(APIListResponse<VW_ALOKASI_DOKTER>))]
        public IHttpActionResult GetMonitoringDokter(string search, string sort, int limit, int offset)
        {

            var pageSize = limit;
            var pageNumber = offset;
            var result = db.VW_ALOKASI_DOKTER
                            .OrderByDescending(x => x.JENIS_SPESIALISASI).ThenByDescending(x => x.JUMLAH_PASIEN)
                            .Skip(GetSkip(pageNumber, pageSize))
                            .Take(pageSize).ToList();

            var totalRecord = db.VW_ALOKASI_DOKTER.Count();
            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<VW_ALOKASI_DOKTER>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        private static int GetSkip(int pageIndex, int take)
        {
            return (pageIndex - 1) * take;
        }
    }
}
