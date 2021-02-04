using API_Sistem_Informasi_RS.Models;
using API_Sistem_Informasi_RS.Models.Response;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Sistem_Informasi_RS.Controllers
{
    [Authorize]
    public class PasienController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();

        [ResponseType(typeof(VW_HISTORY_MEDIS_PASIEN))]
        [Authorize(Roles = "Super Admin,Pasien")]
        public IHttpActionResult GetHistoryMedisByIdPemeriksaan(int idPemeriksaan)
        {
            var result = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).FirstOrDefault();
            result.STATUS_TUNGGU = GetStatusTunggu(idPemeriksaan);

            return Ok(result);
        }

        [Route("api/pasien/recent")]
        [ResponseType(typeof(VW_HISTORY_MEDIS_PASIEN))]
        [Authorize(Roles = "Super Admin,Pasien")]
        public IHttpActionResult GetHistoryMedisRecent()
        {
            var result = new VW_HISTORY_MEDIS_PASIEN();

            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            if (roles.Value == "Pasien")
            {
                var username = identity.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
                var pasien = db.PASIENs.Where(x => x.USER.USERNAME == username).FirstOrDefault();

                result = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PASIEN == pasien.ID_PASIEN).OrderByDescending(x => x.TGL_MASUK).FirstOrDefault();
            }
            else
            {
                result = db.VW_HISTORY_MEDIS_PASIEN.OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN).FirstOrDefault();
            }

            result.STATUS_TUNGGU = GetStatusTunggu(result.ID_PEMERIKSAAN.GetValueOrDefault());

            return Ok(result);
        }

        [Route("api/pasien/historymedis")]
        [Authorize(Roles = "Super Admin,Eksekutif,Pasien")]
        [ResponseType(typeof(APIListResponse<VW_HISTORY_MEDIS_PASIEN>))]
        public IHttpActionResult GetMonitoringHistoryMedisPasien(string search, string sort, int limit, int offset)
        {
            var pageSize = limit;
            var pageNumber = offset;
            var totalRecord = 0;
            var result = new List<VW_HISTORY_MEDIS_PASIEN>();

            var identity = (ClaimsIdentity)User.Identity;
            var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).FirstOrDefault();
            if (roles.Value == "Pasien")
            {
                var username = identity.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value;
                //var appUser = db.USERS.Where(x => x.USERNAME == username).FirstOrDefault();
                var pasien = db.PASIENs.Where(x => x.USER.USERNAME == username).FirstOrDefault();

                result = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PASIEN == pasien.ID_PASIEN)
                            .OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN)
                            .Skip(GetSkip(pageNumber, pageSize))
                            .Take(pageSize).ToList();

                totalRecord = db.VW_HISTORY_MEDIS_PASIEN.Where(x => x.ID_PASIEN == pasien.ID_PASIEN).Count();
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
