using API_Sistem_Informasi_RS.Models.Report;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace API_Sistem_Informasi_RS.Controllers
{
    public class ReportController : Controller
    {
        private mayasariEntities db = new mayasariEntities();

        // GET: Report
        [HttpGet]
        public async Task<ActionResult> RptRegisterKasus(string format)
        {
            //Get Data
            var dataRegisterKasus = await Task.Run(() => db.VW_REGISTER_KASUS.AsEnumerable());

            return CreateReport(dataRegisterKasus, format, "DataSet1", "Laporan Register Kasus", "Reports/RptRegisterKasus.rdlc");
        }

        [HttpGet]
        public async Task<ActionResult> RptAlokasiDokter(string format)
        {
            //Get Data
            var dataAlokasiDokter = await Task.Run(() => db.Database.SqlQuery<RptAlokasiDokter>(@"SELECT a.DOKTER Dokter, a.JENIS_SPESIALISASI Spesialisasi, (a.Queuing + a.Checkup + a.Closed) AS JumlahPemeriksaan, a.Queuing, a.Checkup, a.Closed FROM (
	                            SELECT DOKTER, JENIS_SPESIALISASI, [Queuing], [Checkup], [Closed] 
	                            FROM (
	                            SELECT f.NAMA AS DOKTER, g.JENIS_SPESIALISASI, c.STATUS_KASUS
	                            FROM dbo.REGISTER_KASUS AS a INNER JOIN
	                            dbo.T_KASUS AS b ON a.REG_NO = b.REG_NO INNER JOIN
	                            dbo.KASUS AS c ON b.ID_KASUS = c.ID_KASUS INNER JOIN
	                            dbo.KLINIK AS d ON a.ID_KLINIK = d.ID_KLINIK LEFT OUTER JOIN
	                            dbo.PEMERIKSAAN AS e ON c.ID_PEMERIKSAAN = e.ID_PEMERIKSAAN LEFT OUTER JOIN
	                            dbo.DOKTER AS f ON e.ID_DOKTER = f.ID_DOKTER LEFT OUTER JOIN
	                            dbo.SPESIALISASI AS g ON f.ID_SPESIALISASI = g.ID_SPESIALISASI 
	                            ) AS SourceTable
	                            PIVOT (
	                             COUNT(STATUS_KASUS) 
	                             FOR STATUS_KASUS IN ([Queuing], [Checkup], [Closed])  
	                            ) As PivotTable
                            ) As a").AsEnumerable());

            return CreateReport(dataAlokasiDokter, format, "DataSet1", "Laporan Alokasi Dokter", "Reports/RptAlokasiDokter.rdlc");
        }

        [HttpGet]
        public async Task<ActionResult> RptPemeriksaanPasien(string format)
        {
            //Get Data
            var detailPemeriksaanPasiens = await Task.Run(() => db.Database.SqlQuery<RptDetailPemeriksaanPasien>(@"
                   SELECT a.REG_NO, a.NAMA AS PASIEN, f.NAMA AS DOKTER, d.NAMA_KLINIK, b.TGL_MASUK, e.RUANGAN, a.KELUHAN, g.GEJALA, g.DIAGNOSA, g.TINDAKAN, g.HASIL_TEST_LAB
                        FROM dbo.REGISTER_KASUS AS a INNER JOIN
                         dbo.T_KASUS AS b ON a.REG_NO = b.REG_NO INNER JOIN
                         dbo.KASUS AS c ON b.ID_KASUS = c.ID_KASUS INNER JOIN
                         dbo.KLINIK AS d ON a.ID_KLINIK = d.ID_KLINIK LEFT OUTER JOIN
                         dbo.PEMERIKSAAN AS e ON c.ID_PEMERIKSAAN = e.ID_PEMERIKSAAN LEFT OUTER JOIN
                         dbo.DOKTER AS f ON e.ID_DOKTER = f.ID_DOKTER LEFT OUTER JOIN
                         dbo.REKAM_MEDIS AS g ON e.ID_REKAM_MEDIS = g.ID_REKAM_MEDIS INNER JOIN
                         dbo.SPESIALISASI AS h ON f.ID_SPESIALISASI = h.ID_SPESIALISASI
						 WHERE c.STATUS_KASUS = 'Closed'
                    ").AsEnumerable());

            return CreateReport(detailPemeriksaanPasiens, format, "DataSet1", "Laporan Pemeriksaan Pasien", "Reports/RptDetailPemeriksaanPasien.rdlc");
        }

        [HttpGet]
        public async Task<ActionResult> RptKinerjaDokter(string format)
        {
            //Get Data
            var rptKinerjaDokters = await Task.Run(() => db.Database.SqlQuery<RptKinerjaDokter>(@"
                  SELECT f.NAMA AS DOKTER, h.JENIS_SPESIALISASI, e.ID_PEMERIKSAAN, b.TGL_MASUK, i.TGL AS TGL_KELUAR
                        FROM dbo.REGISTER_KASUS AS a INNER JOIN
                         dbo.T_KASUS AS b ON a.REG_NO = b.REG_NO INNER JOIN
                         dbo.KASUS AS c ON b.ID_KASUS = c.ID_KASUS INNER JOIN
                         dbo.KLINIK AS d ON a.ID_KLINIK = d.ID_KLINIK LEFT OUTER JOIN
                         dbo.PEMERIKSAAN AS e ON c.ID_PEMERIKSAAN = e.ID_PEMERIKSAAN LEFT OUTER JOIN
                         dbo.DOKTER AS f ON e.ID_DOKTER = f.ID_DOKTER LEFT OUTER JOIN
                         dbo.REKAM_MEDIS AS g ON e.ID_REKAM_MEDIS = g.ID_REKAM_MEDIS INNER JOIN
                         dbo.SPESIALISASI AS h ON f.ID_SPESIALISASI = h.ID_SPESIALISASI INNER JOIN
						 dbo.KASUS_STAT AS i ON c.ID_KASUS = i.ID_KASUS 
						 WHERE i.STATUS_KASUS = 'Closed'
                    ").ToList());

            foreach (var item in rptKinerjaDokters)
            {
                var lamaPemeriksaan = item.TGL_KELUAR - item.TGL_MASUK;
                var lamaHariPemeriksaan = lamaPemeriksaan.Days;
                var lamaJamPemeriksaan = lamaPemeriksaan.Hours;
                var lamaMenitPemeriksaan = lamaPemeriksaan.Minutes;

                if (lamaHariPemeriksaan > 0)
                {
                    item.LAMA_PEMERIKSAAN += $"{lamaHariPemeriksaan} Hari, ";
                }
                item.LAMA_PEMERIKSAAN += $"{lamaJamPemeriksaan} Jam, {lamaMenitPemeriksaan} Menit";
                item.KINERJA = "Baik";
            }

            return CreateReport(rptKinerjaDokters, format, "DataSet1", "Laporan Kinerja Dokter", "Reports/RptKinerjaDokter.rdlc");
        }

        private ActionResult CreateReport<T>(IEnumerable<T> data, string format, string dataSet, string reportName, string reportPath)
        {
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;

            DataTable dt = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in props)
            {
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ??
                    prop.PropertyType);
            }

            foreach (T item in data)
            {
                var values = new object[props.Length];
                for (int x = 0; x < props.Length; x++)
                {
                    values[x] = props[x].GetValue(item, null);
                }
                dt.Rows.Add(values);
            }

            ReportDataSource rds = new ReportDataSource(dataSet, dt);
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = reportPath;
            viewer.LocalReport.EnableExternalImages = true;
            viewer.LocalReport.DataSources.Add(rds);
            byte[] bytes = viewer.LocalReport.Render(format, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            if (format == "PDF")
            {
                mimeType = "Application/" + format;
                format = "";
            }
            else
            {
                Response.AddHeader("content-disposition", $"attachment; filename={reportName}." + extension);
            }
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;

            Response.BinaryWrite(bytes); // create the file
            Response.Flush();
            Response.End();
            return View();
        }
    }
}