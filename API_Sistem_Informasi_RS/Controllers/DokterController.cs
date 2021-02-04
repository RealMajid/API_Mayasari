using API_Sistem_Informasi_RS.Models;
using API_Sistem_Informasi_RS.Models.Request;
using API_Sistem_Informasi_RS.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Sistem_Informasi_RS.Controllers
{
    [Authorize]
    public class DokterController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();
        private readonly Random _random = new Random();

        [ResponseType(typeof(VW_HISTORY_MEDIS_PASIEN))]
        [Authorize(Roles = "Super Admin,Dokter")]
        public IHttpActionResult GetHistoryMedisByIdPemeriksaan(int idPemeriksaan)
        {
            var result = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
            result.STATUS_TUNGGU = GetStatusTunggu(idPemeriksaan);

            return Ok(result);
        }

        [ResponseType(typeof(RekamMedisRequest))]
        [Route("api/dokter/rekammedis")]
        [HttpGet]
        [Authorize(Roles = "Super Admin,Dokter,Pasien")]
        public IHttpActionResult GetRekamMedisById(int idPemeriksaan)
        {
            var result = new RekamMedisRequest();
            var tKasus = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
            var rekamMedis = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == tKasus.ID_REKAM_MEDIS).FirstOrDefault();

            if (rekamMedis == null) return Ok(); 

            result.IdPemeriksaan = idPemeriksaan;
            result.IdRekamMedis = rekamMedis.ID_REKAM_MEDIS;
            result.Gejala = rekamMedis.GEJALA;
            result.Tindakan = rekamMedis.TINDAKAN;
            result.Diagnosa = rekamMedis.DIAGNOSA;
            result.HasilTestLab = rekamMedis.HASIL_TEST_LAB;

            return Ok(result);
        }

        [Route("api/dokter/antrianpasien")]
        [Authorize(Roles = "Super Admin,Eksekutif,Dokter")]
        [ResponseType(typeof(APIListResponse<VW_HISTORY_MEDIS_PASIEN>))]
        public IHttpActionResult GetMonitoringAntrianPasien(string search, string sort, int limit, int offset)
        {
            var pageSize = limit;
            var pageNumber = offset;
            var totalRecord = 0;
            var result = new List<VW_HISTORY_MEDIS_PASIEN>();

            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            if (roles.Value == "Dokter")
            {
                var username = identity.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
                //var appUser = db.USERS.Where(x => x.USERNAME == username).FirstOrDefault();
                var dokter = db.DOKTERs.Where(x => x.USER.USERNAME == username).FirstOrDefault();

                result = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_DOKTER == dokter.ID_DOKTER)
                            .OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN)
                            .Skip(GetSkip(pageNumber, pageSize))
                            .Take(pageSize).ToList();

                totalRecord = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_DOKTER == dokter.ID_DOKTER).Count();
            }
            else
            {
                result = db.VW_HISTORY_MEDIS_PASIEN
                           .OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN)
                           .Skip(GetSkip(pageNumber, pageSize))
                           .Take(pageSize).ToList();

                totalRecord = db.VW_HISTORY_MEDIS_PASIEN.Count();
            }

            foreach (var item in result)
            {
                item.STATUS_TUNGGU = GetStatusTunggu(item.ID_PEMERIKSAAN.GetValueOrDefault());
            }

            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<VW_HISTORY_MEDIS_PASIEN>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        [ResponseType(typeof(RekamMedisRequest))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [Route("api/dokter/rekammedis")]
        [HttpPost]
        public async Task<IHttpActionResult> PostRekamMedis(RekamMedisRequest rekamMedisRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var kasusStat = new KASUS_STAT();
                var pemeriksaan = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == rekamMedisRequest.IdPemeriksaan).FirstOrDefault();
                var rekamMedis = new REKAM_MEDIS();
                rekamMedis.GEJALA = rekamMedisRequest.Gejala;
                rekamMedis.DIAGNOSA = rekamMedisRequest.Diagnosa;
                rekamMedis.VONIS = rekamMedis.DIAGNOSA;
                rekamMedis.TINDAKAN = rekamMedisRequest.Tindakan;

                while (true)
                {
                    rekamMedis.ID_REKAM_MEDIS = _random.Next(1, 999999999);
                    var count = db.REKAM_MEDIS.Where(k => k.ID_REKAM_MEDIS == rekamMedis.ID_REKAM_MEDIS).Count();

                    if (count == 0) break;
                }

                rekamMedisRequest.IdRekamMedis = rekamMedis.ID_REKAM_MEDIS;
                pemeriksaan.ID_REKAM_MEDIS = rekamMedis.ID_REKAM_MEDIS;

                db.REKAM_MEDIS.Add(rekamMedis);

                await db.SaveChangesAsync();

                return Ok(rekamMedisRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(RekamMedisRequest))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [Route("api/dokter/rekammedis")]
        [HttpPut]
        public async Task<IHttpActionResult> PutRekamMedis(int idRekamMedis, RekamMedisRequest rekamMedisRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var rekamMedisInDB = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == idRekamMedis).FirstOrDefault();
                rekamMedisInDB.GEJALA = rekamMedisRequest.Gejala;
                rekamMedisInDB.DIAGNOSA = rekamMedisRequest.Diagnosa;
                rekamMedisInDB.TINDAKAN = rekamMedisRequest.Tindakan;

                await db.SaveChangesAsync();

                return Ok(rekamMedisRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(RekamMedisRequest))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [Route("api/dokter/closedpemeriksaan")]
        [HttpPut]
        public async Task<IHttpActionResult> PutPemeriksaanToClosed(int idPemeriksaan, VW_HISTORY_MEDIS_PASIEN vwHistoryMedis)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var kasusStat = new KASUS_STAT();
                var pemeriksaanInDB = db.KASUS.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
                pemeriksaanInDB.STATUS_KASUS = Kasus.StatusKasus.Closed.ToString();

                kasusStat.ID_KASUS = pemeriksaanInDB.ID_KASUS;
                kasusStat.TGL = DateTime.Now;
                kasusStat.STATUS_KASUS = pemeriksaanInDB.STATUS_KASUS;
                db.KASUS_STAT.Add(kasusStat);

                await db.SaveChangesAsync();

                // update data checkup dari dokter bersangkutan
                var pemeriksaan = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();

                // get data pemeriksaan tgl masuk awal
                var pemeriksaanGiliran = db.Database.SqlQuery<PEMERIKSAAN>(@"SELECT * FROM PEMERIKSAAN WHERE
                        ID_PEMERIKSAAN IN(SELECT ID_PEMERIKSAAN FROM KASUS WHERE LOWER(STATUS_KASUS) IN ('queuing', 'checkup')) AND ID_DOKTER = @Id", new SqlParameter("@Id", pemeriksaan.ID_DOKTER))
                            .OrderBy(x => x.TGL)
                            .FirstOrDefault();

                if (pemeriksaanGiliran != null)
                {
                    var kasusGiliran = db.KASUS.Where(x => x.ID_PEMERIKSAAN == pemeriksaanGiliran.ID_PEMERIKSAAN).FirstOrDefault();
                    kasusGiliran.STATUS_KASUS = Kasus.StatusKasus.Checkup.ToString();

                    var kasusStatGiliran = new KASUS_STAT();
                    kasusStatGiliran.ID_KASUS = kasusGiliran.ID_KASUS;
                    kasusStatGiliran.TGL = kasusStat.TGL;
                    kasusStatGiliran.STATUS_KASUS = kasusGiliran.STATUS_KASUS;
                    db.KASUS_STAT.Add(kasusStatGiliran);

                    await db.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(REKAM_MEDIS))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [Route("api/dokter/rekammedis")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRekamMedis(int idRekamMedis)
        {
            var rekamMedisToRemove = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == idRekamMedis).FirstOrDefault();
            db.REKAM_MEDIS.Remove(rekamMedisToRemove);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                BadRequest(e.Message);
            }

            return Ok(rekamMedisToRemove);
        }

        private static int GetSkip(int pageIndex, int take)
        {
            return (pageIndex - 1) * take;
        }

        private string GetStatusTunggu(int idPemeriksaan)
        {
            var kasus = db.KASUS.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
            if (kasus == null) return "Menunggu di Admisi";
            if (kasus.STATUS_KASUS == Kasus.StatusKasus.Closed.ToString()) return "Selesai ditangani";
            if (kasus.STATUS_KASUS == Kasus.StatusKasus.Registered.ToString()) return "Menunggu di Admisi";
            if (kasus.STATUS_KASUS == Kasus.StatusKasus.Checkup.ToString()) return "Sedang ditangani (Checkup)";

            var pemeriksaan = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
            var pemeriksaanList = db.Database.SqlQuery<PEMERIKSAAN>(@"SELECT * FROM PEMERIKSAAN WHERE
                        ID_PEMERIKSAAN IN(SELECT ID_PEMERIKSAAN FROM KASUS WHERE LOWER(STATUS_KASUS) IN ('queuing', 'checkup')) AND ID_DOKTER = @Id", new SqlParameter("@Id", pemeriksaan.ID_DOKTER))
                        .OrderBy(x => x.TGL)
                        .ToList();

            var noAntrian = 0;
            for (int i = 0; i < pemeriksaanList.Count; i++)
            {
                if (pemeriksaanList[i].ID_PEMERIKSAAN == idPemeriksaan) noAntrian = i;
            }

            if (noAntrian == 0)
            {
                return "Sedang ditangani (Checkup)";
            }
            else
            {
                return $"{noAntrian} Antrian Lagi";
            }
        }
    }
}
