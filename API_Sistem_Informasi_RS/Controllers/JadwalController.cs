using API_Sistem_Informasi_RS.Models.Request;
using API_Sistem_Informasi_RS.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Sistem_Informasi_RS.Controllers
{
    public class JadwalController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();
        private readonly Random _random = new Random();

        [ResponseType(typeof(JadwalRequest))]
        [HttpGet]
        [Authorize(Roles = "Super Admin,Pasien,Dokter")]
        public IHttpActionResult GetJadwalById(int idJadwal)
        {
            var result = new JadwalRequest();
            var jadwalInDB = db.JADWALs.Where(x => x.ID_JADWAL == idJadwal).FirstOrDefault();

            result.IdJadwal = jadwalInDB.ID_JADWAL;
            result.IdPemeriksaan = jadwalInDB.ID_PEMERIKSAAN;
            result.TglMulai = jadwalInDB.MULAI.GetValueOrDefault();
            result.TglSelesai = jadwalInDB.SELESAI.GetValueOrDefault();
            result.Keperluan = jadwalInDB.KEPERLUAN;
            result.HasilPemeriksaan = jadwalInDB.HASIL_PEMERIKSAAN;
            result.Status = jadwalInDB.STATUS.GetValueOrDefault();

            if (jadwalInDB == null) return Ok();

            return Ok(result);
        }

        [ResponseType(typeof(JadwalRequest))]
        [Authorize(Roles = "Super Admin,Pasien")]
        [HttpPost]
        public async Task<IHttpActionResult> PostPenjadwalan(JadwalRequest jadwalRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var jadwal = new JADWAL();
                jadwal.MULAI = jadwalRequest.TglMulai;
                jadwal.SELESAI = jadwalRequest.TglSelesai;
                jadwal.ID_PEMERIKSAAN = jadwalRequest.IdPemeriksaan;
                jadwal.KEPERLUAN = jadwalRequest.Keperluan;
                jadwal.STATUS = 1;

                while (true)
                {
                    jadwal.ID_JADWAL = _random.Next(1, 999999999);
                    var count = db.JADWALs.Where(k => k.ID_JADWAL == jadwal.ID_JADWAL).Count();

                    if (count == 0) break;
                }
                jadwalRequest.IdJadwal = jadwal.ID_JADWAL;

                db.JADWALs.Add(jadwal);

                await db.SaveChangesAsync();

                return Ok(jadwalRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ResponseType(typeof(JadwalRequest))]
        [Route("api/jadwal/recent")]
        [Authorize(Roles = "Super Admin,Pasien")]
        [HttpPost]
        public async Task<IHttpActionResult> PostPenjadwalanRecent(JadwalRequest jadwalRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var historyMedis = new VW_HISTORY_MEDIS_PASIEN();
                var rekamMedis = new REKAM_MEDIS();

                var identity = (ClaimsIdentity)User.Identity;
                var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
                if (roles.Value == "Pasien")
                {
                    var username = identity.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
                    var pasien = db.PASIENs.Where(x => x.USER.USERNAME == username).FirstOrDefault();

                    historyMedis = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PASIEN == pasien.ID_PASIEN).OrderByDescending(x => x.TGL_MASUK).FirstOrDefault();
                    var tKasus = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == historyMedis.ID_PEMERIKSAAN).FirstOrDefault();
                    if (tKasus.ID_REKAM_MEDIS.HasValue) rekamMedis = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == tKasus.ID_REKAM_MEDIS).FirstOrDefault();
                }
                else
                {
                    historyMedis = db.VW_HISTORY_MEDIS_PASIEN.OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN).FirstOrDefault();
                    var tKasus = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == historyMedis.ID_PEMERIKSAAN).FirstOrDefault();
                    if (tKasus.ID_REKAM_MEDIS.HasValue) rekamMedis = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == tKasus.ID_REKAM_MEDIS).FirstOrDefault();
                }

                var jadwal = new JADWAL();
                jadwal.MULAI = jadwalRequest.TglMulai;
                jadwal.SELESAI = jadwalRequest.TglSelesai;
                jadwal.ID_PEMERIKSAAN = historyMedis.ID_PEMERIKSAAN.GetValueOrDefault();
                jadwal.KEPERLUAN = jadwalRequest.Keperluan;
                jadwal.STATUS = 1;

                while (true)
                {
                    jadwal.ID_JADWAL = _random.Next(1, 999999999);
                    var count = db.JADWALs.Where(k => k.ID_JADWAL == jadwal.ID_JADWAL).Count();

                    if (count == 0) break;
                }
                jadwalRequest.IdJadwal = jadwal.ID_JADWAL;

                db.JADWALs.Add(jadwal);

                await db.SaveChangesAsync();

                return Ok(jadwalRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [ResponseType(typeof(JADWAL))]
        [Authorize(Roles = "Super Admin,Pasien,Dokter")]
        [HttpPut]
        public async Task<IHttpActionResult> PutPenjadwalan(int idJadwal, JadwalRequest jadwalRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var jadwalInDB = db.JADWALs.Where(x => x.ID_JADWAL == idJadwal).FirstOrDefault();

                jadwalInDB.MULAI = jadwalRequest.TglMulai;
                jadwalInDB.SELESAI = jadwalRequest.TglSelesai;
                jadwalInDB.KEPERLUAN = jadwalRequest.Keperluan;
                jadwalInDB.HASIL_PEMERIKSAAN = jadwalRequest.HasilPemeriksaan;

                await db.SaveChangesAsync();

                return Ok(jadwalRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(JADWAL))]
        [Route("api/jadwal/recent")]
        [Authorize(Roles = "Super Admin,Pasien,Dokter")]
        [HttpPut]
        public async Task<IHttpActionResult> PutPenjadwalanRecent(int idJadwal, JadwalRequest jadwalRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var jadwalInDB = db.JADWALs.Where(x => x.ID_JADWAL == idJadwal).FirstOrDefault();

                jadwalInDB.MULAI = jadwalRequest.TglMulai;
                jadwalInDB.SELESAI = jadwalRequest.TglSelesai;
                jadwalInDB.KEPERLUAN = jadwalRequest.Keperluan;
                jadwalInDB.HASIL_PEMERIKSAAN = jadwalRequest.HasilPemeriksaan;

                await db.SaveChangesAsync();

                return Ok(jadwalRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(JADWAL))]
        [Route("api/jadwal/done")]
        [Authorize(Roles = "Super Admin,Pasien,Dokter")]
        [HttpPut]
        public async Task<IHttpActionResult> PutSelesaiPenjadwalan(int idJadwal, JadwalRequest jadwalRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var jadwalInDB = db.JADWALs.Where(x => x.ID_JADWAL == idJadwal).FirstOrDefault();

                jadwalInDB.STATUS = 2;
                jadwalRequest.Status = 2;

                await db.SaveChangesAsync();

                return Ok(jadwalRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [Authorize(Roles = "Super Admin,Pasien,Dokter")]
        [ResponseType(typeof(APIListResponse<JadwalRequest>))]
        public IHttpActionResult GetListPenjadwalan(string search, string sort, int idPemeriksaan, int limit, int offset)
        {
            var pageSize = limit;
            var pageNumber = offset;
            var totalRecord = 0;
            var result = new List<JadwalRequest>();

            var jadwalRequests = db.JADWALs.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan)
                       .OrderByDescending(x => x.MULAI).ThenBy(x => x.SELESAI)
                       .Skip(GetSkip(pageNumber, pageSize))
                       .Take(pageSize).ToList();

            foreach (var x in jadwalRequests)
            {
                var jadwal = new JadwalRequest
                {
                    IdJadwal = x.ID_JADWAL,
                    IdPemeriksaan = x.ID_PEMERIKSAAN,
                    TglMulai = x.MULAI.GetValueOrDefault(),
                    TglSelesai = x.SELESAI.GetValueOrDefault(),
                    Keperluan = x.KEPERLUAN,
                    HasilPemeriksaan = x.HASIL_PEMERIKSAAN,
                    Status = x.STATUS.GetValueOrDefault()
                };

                result.Add(jadwal);
            }

            totalRecord = db.JADWALs.Count();

            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<JadwalRequest>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        [Authorize(Roles = "Super Admin,Pasien,Dokter")]
        [Route("api/jadwal/listrecent")]
        [ResponseType(typeof(APIListResponse<JadwalRequest>))]
        public IHttpActionResult GetListPenjadwalanRecent(string search, string sort, int limit, int offset)
        {
            var historyMedis = new VW_HISTORY_MEDIS_PASIEN();
            var rekamMedis = new REKAM_MEDIS();

            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            if (roles.Value == "Pasien")
            {
                var username = identity.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
                var pasien = db.PASIENs.Where(x => x.USER.USERNAME == username).FirstOrDefault();

                historyMedis = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PASIEN == pasien.ID_PASIEN).OrderByDescending(x => x.TGL_MASUK).FirstOrDefault();
                var tKasus = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == historyMedis.ID_PEMERIKSAAN).FirstOrDefault();
                if (tKasus.ID_REKAM_MEDIS.HasValue) rekamMedis = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == tKasus.ID_REKAM_MEDIS).FirstOrDefault();
            }
            else
            {
                historyMedis = db.VW_HISTORY_MEDIS_PASIEN.OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN).FirstOrDefault();
                var tKasus = db.PEMERIKSAANs.Where(x => x.ID_PEMERIKSAAN == historyMedis.ID_PEMERIKSAAN).FirstOrDefault();
                if (tKasus.ID_REKAM_MEDIS.HasValue) rekamMedis = db.REKAM_MEDIS.Where(x => x.ID_REKAM_MEDIS == tKasus.ID_REKAM_MEDIS).FirstOrDefault();
            }

            var pageSize = limit;
            var pageNumber = offset;
            var totalRecord = 0;
            var result = new List<JadwalRequest>();

            var idPemeriksaan = historyMedis.ID_PEMERIKSAAN.GetValueOrDefault();

            //var appUser = db.USERS.Where(x => x.USERNAME == username).FirstOrDefault();
            var jadwalRequests = db.JADWALs.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan)
                        .OrderByDescending(x => x.MULAI).ThenBy(x => x.SELESAI)
                        .Skip(GetSkip(pageNumber, pageSize))
                        .Take(pageSize).ToList();

            foreach (var x in jadwalRequests)
            {
                var jadwal = new JadwalRequest
                {
                    IdJadwal = x.ID_JADWAL,
                    IdPemeriksaan = x.ID_PEMERIKSAAN,
                    TglMulai = x.MULAI.GetValueOrDefault(),
                    TglSelesai = x.SELESAI.GetValueOrDefault(),
                    Keperluan = x.KEPERLUAN,
                    HasilPemeriksaan = x.HASIL_PEMERIKSAAN,
                    Status = x.STATUS.GetValueOrDefault()
                };

                result.Add(jadwal);
            }

            totalRecord = db.JADWALs.Count();

            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<JadwalRequest>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        private static int GetSkip(int pageIndex, int take)
        {
            return (pageIndex - 1) * take;
        }
    }
}
