using API_Sistem_Informasi_RS.Models;
using API_Sistem_Informasi_RS.Models.Request;
using API_Sistem_Informasi_RS.Models.Response;
using API_Sistem_Informasi_RS.Security;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace API_Sistem_Informasi_RS.Controllers
{
    [Authorize(Roles = "Super Admin,Frontdesk")]
    public class RegisterKasusController : ApiController
    {
        private mayasariEntities db = new mayasariEntities();
        private readonly Random _random = new Random();
        
        [ResponseType(typeof(APIListResponse<VW_REGISTER_KASUS>))]
        public IHttpActionResult GetRegisterKasusList(string search, string sort, int limit, int offset)
        {
            var pageSize = limit;
            var pageNumber = offset;
            var result = db.VW_REGISTER_KASUS
                            .OrderByDescending(x => x.TGL_MASUK).ThenBy(x => x.PASIEN)
                            .Skip(GetSkip(pageNumber, pageSize))
                            .Take(pageSize).ToList();

            var totalRecord = db.VW_REGISTER_KASUS.Count();
            var totalPage = (totalRecord + pageSize - 1) / pageSize;

            return Ok(new APIListResponse<VW_REGISTER_KASUS>(false, HttpStatusCode.OK.ToString(), HttpStatusCode.OK.ToString(), result, totalRecord, totalPage));
        }

        [ResponseType(typeof(RegisterKasusRequest))]
        public IHttpActionResult GetRegisterKasusByRegNo(string regNo)
        {
            var result = new RegisterKasusRequest();
            var registerKasusInDB = db.REGISTER_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
            var transactKasusInDB = db.T_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
            var kasusInDB = db.KASUS.Where(x => x.ID_KASUS == transactKasusInDB.ID_KASUS).FirstOrDefault();
            var pasienInDB = db.PASIENs.Where(x => x.ID_PASIEN == transactKasusInDB.ID_PASIEN).FirstOrDefault();
            var userInDB = db.USERS.Where(x => x.ID_USER == pasienInDB.ID_USER).FirstOrDefault();

            result.Alamat = pasienInDB.ALAMAT;
            result.BeratBadan = registerKasusInDB.BERAT_BADAN.GetValueOrDefault();
            result.Email = userInDB.EMAIL;
            result.JenisKelamin = pasienInDB.JENIS_KELAMIN;
            result.Keluhan = registerKasusInDB.KELUHAN;
            result.Klinik = registerKasusInDB.ID_KLINIK.GetValueOrDefault();
            result.Nama = pasienInDB.NAMA;
            result.Telp = pasienInDB.TELP;
            result.TempatLahir = pasienInDB.TEMPAT_LAHIR;
            result.TensiDarah = registerKasusInDB.TENSI_DARAH.GetValueOrDefault();
            result.TglLahir = pasienInDB.TGL_LAHIR;
            result.TinggiBadan = registerKasusInDB.TINGGI_BADAN.GetValueOrDefault();

            return Ok(result);
        }

        // POST api/registerkasus
        [ResponseType(typeof(REGISTER_KASUS))]
        public async Task<IHttpActionResult> PutRegisterKasus(string regNo, RegisterKasusRequest registerKasusRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var registerKasusInDB = db.REGISTER_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
                var transactKasusInDB = db.T_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
                var kasusInDB = db.KASUS.Where(x => x.ID_KASUS == transactKasusInDB.ID_KASUS).FirstOrDefault();
                if (kasusInDB.STATUS_KASUS != Kasus.StatusKasus.Registered.ToString()) return BadRequest("Kasus sudah di admisi");

                registerKasusInDB.NAMA = registerKasusRequest.Nama;

                registerKasusInDB.KELUHAN = registerKasusRequest.Keluhan;
                registerKasusInDB.TINGGI_BADAN = registerKasusRequest.TinggiBadan;
                registerKasusInDB.BERAT_BADAN = registerKasusRequest.BeratBadan;
                registerKasusInDB.TENSI_DARAH = registerKasusRequest.TensiDarah;
                registerKasusInDB.ID_KLINIK = registerKasusRequest.Klinik;

                var pasienInDB = db.PASIENs.Where(x => x.ID_PASIEN == transactKasusInDB.ID_PASIEN).FirstOrDefault();
                var userInDB = db.USERS.Where(x => x.ID_USER == pasienInDB.ID_USER).FirstOrDefault();

                pasienInDB.ALAMAT = registerKasusRequest.Alamat;
                pasienInDB.NAMA = registerKasusRequest.Nama;
                pasienInDB.TGL_LAHIR = registerKasusRequest.TglLahir;
                pasienInDB.TEMPAT_LAHIR = registerKasusRequest.TempatLahir;
                pasienInDB.TELP = registerKasusRequest.Telp;
                pasienInDB.JENIS_KELAMIN = registerKasusRequest.JenisKelamin;

                userInDB.EMAIL = registerKasusRequest.Email;

                var indexAt = registerKasusRequest.Email.IndexOf("@");
                if (indexAt == -1) return BadRequest("Email Tidak Valid");

                userInDB.USERNAME = registerKasusRequest.Email.Substring(0, indexAt);
                userInDB.FULL_NAME = registerKasusRequest.Nama;

                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
           
        }

        [ResponseType(typeof(REGISTER_KASUS))]
        public async Task<IHttpActionResult> PostRegisterKasus(RegisterKasusRequest registerKasusRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var registerKasus = new REGISTER_KASUS();
                var transactKasus = new T_KASUS();
                var kasus = new KASU();
                var kasusStat = new KASUS_STAT();
                registerKasus.NAMA = registerKasusRequest.Nama;

                string registerNumber = string.Empty;
                while (true)
                {
                    registerNumber = GenerateRegisterNumber();
                    var count = db.REGISTER_KASUS.Where(rk => rk.REG_NO == registerNumber).Count();

                    if (count == 0) break;
                }

                while (true)
                {
                    kasus.ID_KASUS = _random.Next(1, 999999999);
                    var count = db.KASUS.Where(k => k.ID_KASUS == kasus.ID_KASUS).Count();

                    if (count == 0) break;
                }

                registerKasus.REG_NO = registerNumber;
                registerKasus.KELUHAN = registerKasusRequest.Keluhan;
                registerKasus.TINGGI_BADAN = registerKasusRequest.TinggiBadan;
                registerKasus.BERAT_BADAN = registerKasusRequest.BeratBadan;
                registerKasus.TENSI_DARAH = registerKasusRequest.TensiDarah;
                registerKasus.ID_KLINIK = registerKasusRequest.Klinik;

                transactKasus.REG_NO = registerKasus.REG_NO;
                transactKasus.ID_KASUS = kasus.ID_KASUS;
                transactKasus.TGL_MASUK = DateTime.Now;

                db.REGISTER_KASUS.Add(registerKasus);

                var pasien = new PASIEN();
                var user = new USER();

                if (registerKasusRequest.JenisPasien == "1")
                {
                    while (true)
                    {
                        pasien.ID_PASIEN = _random.Next(1, 999999999);
                        var count = db.PASIENs.Where(rk => rk.ID_PASIEN == pasien.ID_PASIEN).Count();

                        if (count == 0) break;
                    }

                    while (true)
                    {
                        user.ID_USER = _random.Next(1, 999999999);
                        var count = db.USERS.Where(rk => rk.ID_USER == user.ID_USER).Count();

                        if (count == 0) break;
                    }

                    pasien.ID_USER = user.ID_USER;
                    pasien.ALAMAT = registerKasusRequest.Alamat;
                    pasien.NAMA = registerKasusRequest.Nama;
                    pasien.TGL_LAHIR = registerKasusRequest.TglLahir;
                    pasien.TEMPAT_LAHIR = registerKasusRequest.TempatLahir;
                    pasien.TELP = registerKasusRequest.Telp;
                    pasien.JENIS_KELAMIN = registerKasusRequest.JenisKelamin;

                    db.PASIENs.Add(pasien);

                    user.ACTIVE_YN = "Y";
                    user.EMAIL = registerKasusRequest.Email;

                    var indexAt = registerKasusRequest.Email.IndexOf("@");
                    if (indexAt == -1) return BadRequest("Email Tidak Valid");

                    user.USERNAME = registerKasusRequest.Email.Substring(0, indexAt);
                    user.FULL_NAME = registerKasusRequest.Nama;

                    var tempPassword = "P@ssw0rd";
                    user.PASSWORD = Encryptor.Base64Encode(Encryptor.MD5Hash(tempPassword));

                    db.USERS.Add(user);

                    transactKasus.ID_PASIEN = pasien.ID_PASIEN;
                    kasus.STATUS_KASUS = Kasus.StatusKasus.Registered.ToString();

                    kasusStat.ID_KASUS = kasus.ID_KASUS;
                    kasusStat.STATUS_KASUS = kasus.STATUS_KASUS;
                    kasusStat.TGL = DateTime.Now;

                    db.T_KASUS.Add(transactKasus);
                    db.KASUS.Add(kasus);
                    db.KASUS_STAT.Add(kasusStat);

                    await db.SaveChangesAsync();

                    db.Database.ExecuteSqlCommand($"INSERT INTO ROLE_USERS (ID_USER, ID_ROLE) VALUES ({user.ID_USER}, 4)");

                    await db.SaveChangesAsync();

                    SendEmail(registerKasusRequest.Email, user.USERNAME, tempPassword);

                    return Ok();
                } 

                pasien = db.PASIENs.Where(x => x.ID_PASIEN == registerKasusRequest.IdPasien).FirstOrDefault();
                user = db.USERS.Where(x => x.ID_USER == registerKasusRequest.IdUser).FirstOrDefault();

                transactKasus.ID_PASIEN = pasien.ID_PASIEN;
                kasus.STATUS_KASUS = Kasus.StatusKasus.Registered.ToString();

                kasusStat.ID_KASUS = kasus.ID_KASUS;
                kasusStat.STATUS_KASUS = kasus.STATUS_KASUS;
                kasusStat.TGL = DateTime.Now;

                db.T_KASUS.Add(transactKasus);
                db.KASUS.Add(kasus);
                db.KASUS_STAT.Add(kasusStat);

                await db.SaveChangesAsync();

                return Ok();                   
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [Route("api/admisikasus")]
        [ResponseType(typeof(REGISTER_KASUS))]
        public async Task<IHttpActionResult> PutAdmisiKasus(string regNo, RegisterKasusRequest registerKasusRequest)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Data yang dikirim tidak lengkap");

                var pemeriksaan = new PEMERIKSAAN();
                var kasusStat = new KASUS_STAT();
                var registerKasus = db.REGISTER_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
                var transactKasus = db.T_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
                var kasusInDB = db.KASUS.Where(x => x.ID_KASUS == transactKasus.ID_KASUS).FirstOrDefault();

                string registerNumber = string.Empty;

                while (true)
                {
                    pemeriksaan.ID_PEMERIKSAAN = _random.Next(1, 999999999);
                    var count = db.PEMERIKSAANs.Where(k => k.ID_PEMERIKSAAN == pemeriksaan.ID_PEMERIKSAAN).Count();

                    if (count == 0) break;
                }

                transactKasus.VRF_DATA_PASIEN = transactKasus.PASIEN.NAMA;
                transactKasus.VRF_KELUHAN_PASIEN = registerKasus.KELUHAN;

                kasusInDB.ID_PEMERIKSAAN = pemeriksaan.ID_PEMERIKSAAN;

                kasusStat.ID_KASUS = kasusInDB.ID_KASUS;
                kasusStat.TGL = DateTime.Now;
                // get available dokter dengan spesialisasi sesuai dengan kasus
                var dokterList = db.DOKTERs.Where(x => x.ID_SPESIALISASI == registerKasus.KLINIK.ID_SPESIALISASI);
                //var minPemeriksaan = 0;
                var dokterDictionary = new Dictionary<int, int>();

                foreach (var item in dokterList)
                {
                    var countPemeriksaan = db.Database.SqlQuery<PEMERIKSAAN>(@"SELECT * FROM PEMERIKSAAN WHERE
                        ID_PEMERIKSAAN IN(SELECT ID_PEMERIKSAAN FROM KASUS WHERE LOWER(STATUS_KASUS) IN ('queuing', 'checkup')) AND ID_DOKTER = @Id", new SqlParameter("@Id", item.ID_DOKTER)).Count();

                    //if (countPemeriksaan == 0) pemeriksaan.ID_DOKTER = item.ID_DOKTER;
                    dokterDictionary.Add(item.ID_DOKTER, countPemeriksaan);
                    //if (minPemeriksaan > countPemeriksaan)
                    //{
                    //    minPemeriksaan = countPemeriksaan;
                    //    pemeriksaan.ID_DOKTER = item.ID_DOKTER;
                    //}
                }

                pemeriksaan.ID_DOKTER = dokterDictionary.OrderBy(x => x.Value).FirstOrDefault().Key;
                if (dokterDictionary.Where(x => x.Key == pemeriksaan.ID_DOKTER).FirstOrDefault().Value == 0)
                {
                    kasusInDB.STATUS_KASUS = Kasus.StatusKasus.Checkup.ToString();
                } else 
                {
                    kasusInDB.STATUS_KASUS = Kasus.StatusKasus.Queuing.ToString();
                }

                kasusStat.STATUS_KASUS = kasusInDB.STATUS_KASUS;
                // cek masing-masing beban dokter
                //var idDokter = db.Database.SqlQuery<Nullable<int>>($@"SELECT a.ID_DOKTER FROM (
                //                            SELECT a.ID_DOKTER, MIN(a.COUNT_PEMERIKSAAN) MIN_COUNT_PEMERIKSAAN FROM (
                //                            SELECT a.ID_DOKTER, COUNT(a.ID_PEMERIKSAAN) COUNT_PEMERIKSAAN FROM (
                //                            SELECT ID_DOKTER, ID_PEMERIKSAAN FROM PEMERIKSAAN WHERE
                //                            ID_PEMERIKSAAN IN (SELECT ID_PEMERIKSAAN FROM KASUS WHERE LOWER(STATUS_KASUS) = 'checkup')
                //                            AND ID_SPESIALISASI = @Id
                //                            ) a GROUP BY a.ID_DOKTER
                //                            ) a GROUP BY a.ID_DOKTER) a", new SqlParameter("@Id", registerKasus.KLINIK.ID_SPESIALISASI)).FirstOrDefault().GetValueOrDefault();

                pemeriksaan.TGL = DateTime.Now;
                pemeriksaan.RUANGAN = GenerateRuangan();
                db.PEMERIKSAANs.Add(pemeriksaan);
                db.KASUS_STAT.Add(kasusStat);

                await db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [ResponseType(typeof(RegisterKasusRequest))]
        public async Task<IHttpActionResult> DeleteRegisterKasusByRegNo(string regNo)
        {
            var result = new RegisterKasusRequest();
            var registerKasusInDB = db.REGISTER_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
            var transactKasusInDB = db.T_KASUS.Where(x => x.REG_NO == regNo).FirstOrDefault();
            var kasusInDB = db.KASUS.Where(x => x.ID_KASUS == transactKasusInDB.ID_KASUS).FirstOrDefault();
            if (kasusInDB.STATUS_KASUS != Kasus.StatusKasus.Registered.ToString()) return BadRequest("Kasus yang sudah berjalan tidak dapat diahapus");
           
            var pasienInDB = db.PASIENs.Where(x => x.ID_PASIEN == transactKasusInDB.ID_PASIEN).FirstOrDefault();
            var userInDB = db.USERS.Where(x => x.ID_USER == pasienInDB.ID_USER).FirstOrDefault();

            result.Alamat = pasienInDB.ALAMAT;
            result.BeratBadan = registerKasusInDB.BERAT_BADAN.GetValueOrDefault();
            result.Email = userInDB.EMAIL;
            result.JenisKelamin = pasienInDB.JENIS_KELAMIN;
            result.Keluhan = registerKasusInDB.KELUHAN;
            result.Klinik = registerKasusInDB.ID_KLINIK.GetValueOrDefault();
            result.Nama = pasienInDB.NAMA;
            result.Telp = pasienInDB.TELP;
            result.TempatLahir = pasienInDB.TEMPAT_LAHIR;
            result.TensiDarah = registerKasusInDB.TENSI_DARAH.GetValueOrDefault();
            result.TglLahir = pasienInDB.TGL_LAHIR;
            result.TinggiBadan = registerKasusInDB.TINGGI_BADAN.GetValueOrDefault();

            db.REGISTER_KASUS.Remove(registerKasusInDB);
            db.T_KASUS.Remove(transactKasusInDB);
            db.KASUS.Remove(kasusInDB);
            db.PASIENs.Remove(pasienInDB);
            db.USERS.Remove(userInDB);

            try
            {
                await db.SaveChangesAsync();

                db.Database.ExecuteSqlCommand($"DELETE FROM ROLE_USERS WHERE ID_USER = {userInDB.ID_USER}");
            }
            catch (Exception e)
            {
                BadRequest(e.Message);
            }

            return Ok(result);
        }

        private string GenerateRegisterNumber()
        {
            var registerNumber = new StringBuilder();

            registerNumber.Append("REG-");
            registerNumber.Append(_random.Next(100000, 999999));

            return registerNumber.ToString();
        }

        private string GenerateRuangan()
        {
            var ruanganNumber = new StringBuilder();

            const string chars = "ABCDEFGHIJKL";
            ruanganNumber.Append(Enumerable.Repeat(chars, 1)
              .Select(s => s[_random.Next(s.Length)]).FirstOrDefault());
            ruanganNumber.Append("-");
            ruanganNumber.Append(_random.Next(1, 9));
            ruanganNumber.Append("0");
            ruanganNumber.Append(_random.Next(1, 9));

            return ruanganNumber.ToString();
        }

        private void SendEmail(string email, string namaUser, string password)
        {
            string recipient = email;
            string subject = "Penyampaian Password Aplikasi Mayasari";
            string emailFrom = "abdulmajid23@perbanas.id";

            MailMessage message = new MailMessage();
            message.To.Add(recipient);
            message.From = new MailAddress(emailFrom);
            message.Subject = subject;
            message.Body = $@"Yth. Pengguna Aplikasi Mayasari a.n {namaUser}<br/>
                            <p>Kami sampaikan username password aplikasi mayasari.</p>
                            <p>Demikian kami sampaikan, atas perhatian dan kerja sama yang baik kami ucapkan terima kasih.</p>
                            <table>
                            <tr>
                            <td>Username: </td>
                            <td>{namaUser}</td>
                            </tr>
                            <tr>
                            <td>Password: </td>
                            <td>{password}</td>
                            </tr>
                            </table>
                            <br/>
                            <br/>Salam hormat,
                            <br/>
                            Tim Aplikasi Mayasari";
            message.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(emailFrom, "Reg2020#");
            smtp.EnableSsl = true;
            try
            {
                smtp.Send(message);
            }
            catch (Exception e)
            {
                return;
            }
            return;
        }

        private string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(_random.Next(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        private static int GetSkip(int pageIndex, int take)
        {
            return (pageIndex - 1) * take;
        }
    }
}
