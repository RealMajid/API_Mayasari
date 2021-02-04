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
    [Authorize]
    public class OrderMedisController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();
        private readonly Random _random = new Random();

        [ResponseType(typeof(RekamMedisRequest))]
        [HttpGet]
        [Authorize(Roles = "Super Admin,Dokter")]
        public IHttpActionResult GetOrderMedisById(int idOrder)
        {
            var result = new OrderMedisRequest();
            var orderMedis = db.ORDER_MEDIS.Where(x => x.ID_ORDER == idOrder).FirstOrDefault();

            if (orderMedis == null) return Ok();

            result.IdPemeriksaan = orderMedis.ID_PEMERIKSAAN.GetValueOrDefault();
            result.IdOrder = orderMedis.ID_ORDER;
            result.IdPasien = orderMedis.ID_PASIEN.GetValueOrDefault();
            result.IdObat = orderMedis.ID_OBAT.GetValueOrDefault();
            result.IdLaborat = orderMedis.ID_LABORAT.GetValueOrDefault();
            result.JenisOrderMedis = orderMedis.JENIS_ORDER_MEDIS.GetValueOrDefault();
            result.Jumlah = orderMedis.JUMLAH.GetValueOrDefault();

            return Ok(result);
        }

        [Authorize(Roles = "Super Admin,Eksekutif,Dokter,Pasien")]
        [ResponseType(typeof(APIListResponse<VW_ORDER_MEDIS>))]
        public IHttpActionResult GetOrderMedisList(string search, string sort, int idPemeriksaan, int limit, int offset)
        {
            var pageSize = limit;
            var pageNumber = offset;
            var totalRecord = 0;
            var result = new List<VW_ORDER_MEDIS>();

            result = db.VW_ORDER_MEDIS.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan)
                        .OrderByDescending(x => x.ID_ORDER).ThenBy(x => x.ID_PEMERIKSAAN)
                        .Skip(GetSkip(pageNumber, pageSize))
                        .Take(pageSize).ToList();

            totalRecord = db.VW_ORDER_MEDIS.Where(x => x.ID_PEMERIKSAAN == idPemeriksaan).Count();
            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<VW_ORDER_MEDIS>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        [ResponseType(typeof(OrderMedisRequest))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [HttpPost]
        public async Task<IHttpActionResult> PostOrderMedis(OrderMedisRequest orderMedisRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var orderMedis = new ORDER_MEDIS();
                var kasus = db.KASUS.Where(x => x.ID_PEMERIKSAAN == orderMedisRequest.IdPemeriksaan).FirstOrDefault();
                var tKasus = db.T_KASUS.Where(x => x.ID_KASUS == kasus.ID_KASUS).FirstOrDefault();

                while (true)
                {
                    orderMedis.ID_ORDER = _random.Next(1, 999999999);
                    var count = db.ORDER_MEDIS.Where(k => k.ID_ORDER == orderMedis.ID_ORDER).Count();

                    if (count == 0) break;
                }

                orderMedis.ID_PEMERIKSAAN = orderMedisRequest.IdPemeriksaan;
                orderMedis.ID_OBAT = orderMedisRequest.IdObat;
                orderMedis.ID_LABORAT = orderMedisRequest.IdLaborat;
                orderMedis.ID_PASIEN = tKasus.ID_PASIEN;
                orderMedis.JUMLAH = orderMedisRequest.Jumlah;
                orderMedis.JENIS_ORDER_MEDIS = orderMedisRequest.JenisOrderMedis;
                orderMedisRequest.IdOrder = orderMedis.ID_ORDER;
                orderMedisRequest.IdPasien = orderMedis.ID_PASIEN.GetValueOrDefault();

                db.ORDER_MEDIS.Add(orderMedis);

                await db.SaveChangesAsync();

                return Ok(orderMedisRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(OrderMedisRequest))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [HttpPut]
        public async Task<IHttpActionResult> PutOrderMedis(int idOrder, OrderMedisRequest orderMedisRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var orderMedisInDB = db.ORDER_MEDIS.Where(x => x.ID_ORDER == idOrder).FirstOrDefault();
                orderMedisInDB.ID_OBAT = orderMedisRequest.IdObat;
                orderMedisInDB.ID_LABORAT = orderMedisRequest.IdLaborat;
                orderMedisInDB.JUMLAH = orderMedisRequest.Jumlah;
                orderMedisInDB.JENIS_ORDER_MEDIS = orderMedisRequest.JenisOrderMedis;

                await db.SaveChangesAsync();

                return Ok(orderMedisRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(ORDER_MEDIS))]
        [Authorize(Roles = "Super Admin,Dokter")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRekamMedis(int idOrderMedis)
        {
            var orderMedisToRemove = db.ORDER_MEDIS.Where(x => x.ID_ORDER == idOrderMedis).FirstOrDefault();
            db.ORDER_MEDIS.Remove(orderMedisToRemove);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                BadRequest(e.Message);
            }

            return Ok(orderMedisToRemove);
        }

        private static int GetSkip(int pageIndex, int take)
        {
            return (pageIndex - 1) * take;
        }
    }
}
