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
    public class LabController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();

        [ResponseType(typeof(REKAM_MEDIS))]
        [HttpGet]
        [Authorize(Roles = "Super Admin,Lab")]
        public IHttpActionResult GetRekamMedisById(int idPemeriksaan)
        {
            var result = new REKAM_MEDIS();
            var tKasus = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
            var rekamMedis = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == tKasus.ID_REKAM_MEDIS).FirstOrDefault();

            result.ID_REKAM_MEDIS = rekamMedis.ID_REKAM_MEDIS;
            result.GEJALA = rekamMedis.GEJALA;
            result.DIAGNOSA = rekamMedis.DIAGNOSA;
            result.TINDAKAN = rekamMedis.TINDAKAN;
            result.HASIL_TEST_LAB = rekamMedis.HASIL_TEST_LAB;

            if (rekamMedis == null) return Ok();

            return Ok(result);
        }

        [ResponseType(typeof(REKAM_MEDIS))]
        [Authorize(Roles = "Super Admin,Dokter,Lab")]
        [HttpPut]
        public async Task<IHttpActionResult> PutOrderMedis(int idRekamMedis, REKAM_MEDIS rekamMedis)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var rekamMedisInDB = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == idRekamMedis).FirstOrDefault();
                rekamMedisInDB.HASIL_TEST_LAB = rekamMedis.HASIL_TEST_LAB;

                await db.SaveChangesAsync();

                return Ok(rekamMedis);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Authorize(Roles = "Super Admin,Lab")]
        [ResponseType(typeof(APIListResponse<VW_MONITOR_LAB>))]
        public IHttpActionResult GetMonitoringLab(string search, string sort, string type, int limit, int offset)
        {
            var pageSize = limit;
            var pageNumber = offset;
            var totalRecord = 0;
            var result = new List<VW_MONITOR_LAB>();

            //var appUser = db.USERS.Where(x => x.USERNAME == username).FirstOrDefault();
            result = db.VW_MONITOR_LAB.Where(x => x.STATUS_KASUS.ToLower() == type)
                        .OrderByDescending(x => x.TGL_PEMERIKSAAN).ThenBy(x => x.PASIEN)
                        .Skip(GetSkip(pageNumber, pageSize))
                        .Take(pageSize).ToList();

            totalRecord = db.VW_MONITOR_LAB.Count();

            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<VW_MONITOR_LAB>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        private static int GetSkip(int pageIndex, int take)
        {
            return (pageIndex - 1) * take;
        }
    }
}
