using Bakiye.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bakiye.Controllers
{
    public class HomeController : Controller
    {
        private readonly string cs = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;

        public ActionResult Islemler() => View();

        public ActionResult Kasa() => View();

        public ActionResult Banka() => View();

        public ActionResult KasaBakiye() => View();

        public ActionResult BankaBakiye() => View();

        public JsonResult GetIslemler()
        {
            List<Islem> islemler = new List<Islem>();

            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (SqlCommand getislemler = new SqlCommand(@"select *, coalesce((select KasaAdi from Kasa where ID = Tahsilat_Odeme.KasaID),'') as KasaAdi,
                coalesce((select (BankaAdi + ' - ' + SubeAdi) as BankaAd from Banka where ID = Tahsilat_Odeme.BankaID),'') as BankaAdi from Tahsilat_Odeme", connection))
                {
                    using (SqlDataReader dr = getislemler.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Islem islem = new Islem();

                            islem.ID = Convert.ToInt32(dr["ID"]);
                            islem.IslemTipi = Convert.ToString(dr["IslemTipi"]);

                            islem.IslemNo = Convert.ToString(dr["IslemNo"]);
                            islem.IslemTarih = Convert.ToDateTime(dr["IslemTarih"]).ToLongDateString();
                            if (islem.IslemTipi == "T" || islem.IslemTipi == "O")
                            {
                                islem.KasaBankaID = Convert.ToInt32(dr["KasaID"]);
                                islem.KasaBankaAdi = Convert.ToString(dr["KasaAdi"]);
                            }
                            else
                            {
                                islem.KasaBankaID = Convert.ToInt32(dr["BankaID"]);
                                islem.KasaBankaAdi = Convert.ToString(dr["BankaAdi"]);
                            }
                            islem.Aciklama = Convert.ToString(dr["Aciklama"]);
                            islem.ParaBirimi = Convert.ToString(dr["ParaBirimi"]);
                            islem.Tutar = Convert.ToDecimal(dr["Tutar"]);
                            if (islem.IslemTipi == "T") islem.IslemTipi = "Nakit Tahsilat";
                            else if (islem.IslemTipi == "G") islem.IslemTipi = "Gelen Havale";
                            else if (islem.IslemTipi == "H") islem.IslemTipi = "Giden Havale";
                            else islem.IslemTipi = "Nakit Ödeme";
                            islemler.Add(islem);

                        }
                    }
                }
            }

            return Json(new { data = islemler.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKasa()
        {
            List<KasaBilgi> kasaBilgi = new List<KasaBilgi>();
            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (SqlCommand getkasa = new SqlCommand(@"select *,
                     coalesce((select Sum(Bakiye) from Bakiye_Kasa where ParaBirimi='TL' and KasaID = Kasa.ID),0) as Bakiye_TL,
                     coalesce((select Sum(Bakiye) from Bakiye_Kasa where ParaBirimi='USD' and KasaID = Kasa.ID),0) as Bakiye_USD,
                     coalesce((select Sum(Bakiye) from Bakiye_Kasa where ParaBirimi='EUR' and KasaID = Kasa.ID),0) as Bakiye_EUR,
                     coalesce((select Sum(Bakiye) from Bakiye_Kasa where ParaBirimi='GBP' and KasaID = Kasa.ID),0) as Bakiye_GBP
                     from Kasa", connection))
                {
                    using (SqlDataReader dr = getkasa.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            KasaBilgi kasa = new KasaBilgi();

                            kasa.KasaKodu = Convert.ToString(dr["KasaKodu"]);
                            kasa.KasaAdi = Convert.ToString(dr["KasaAdi"]);
                            kasa.Bakiye_TL = Convert.ToDecimal(dr["Bakiye_TL"]);
                            kasa.Bakiye_USD = Convert.ToDecimal(dr["Bakiye_USD"]);
                            kasa.Bakiye_EUR = Convert.ToDecimal(dr["Bakiye_EUR"]);
                            kasa.Bakiye_GBP = Convert.ToDecimal(dr["Bakiye_GBP"]);
                            kasaBilgi.Add(kasa);
                        }
                    }
                }
            }


            return Json(new { data = kasaBilgi.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBanka()
        {
            List<BankaBilgi> bankaBilgi = new List<BankaBilgi>();

            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (SqlCommand getBanka = new SqlCommand(@"select *,
                coalesce((select Sum(Bakiye) from Bakiye_Banka where ParaBirimi='TL' and BankaID = Banka.ID),0) as Bakiye_TL,
                coalesce((select Sum(Bakiye) from Bakiye_Banka where ParaBirimi='USD' and BankaID = Banka.ID),0) as Bakiye_USD,
                coalesce((select Sum(Bakiye) from Bakiye_Banka where ParaBirimi='EUR' and BankaID = Banka.ID),0) as Bakiye_EUR,
                coalesce((select Sum(Bakiye) from Bakiye_Banka where ParaBirimi='GBP' and BankaID = Banka.ID),0) as Bakiye_GBP
                from Banka", connection))
                {
                    using (SqlDataReader dr = getBanka.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            BankaBilgi banka = new BankaBilgi();
                            banka.BankaKodu = Convert.ToString(dr["BankaKodu"]);
                            banka.BankaAdi = Convert.ToString(dr["BankaAdi"]);
                            banka.SubeAdi = Convert.ToString(dr["SubeAdi"]);
                            banka.HesapNo = Convert.ToString(dr["HesapNo"]);
                            banka.Iban = Convert.ToString(dr["Iban"]);
                            banka.Bakiye_TL = Convert.ToDecimal(dr["Bakiye_TL"]);
                            banka.Bakiye_USD = Convert.ToDecimal(dr["Bakiye_USD"]);
                            banka.Bakiye_EUR = Convert.ToDecimal(dr["Bakiye_EUR"]);
                            banka.Bakiye_GBP = Convert.ToDecimal(dr["Bakiye_GBP"]);
                            bankaBilgi.Add(banka);
                        }
                    }
                }
            }

            return Json(new { data = bankaBilgi.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKasaBilgiIslem()
        {
            List<KasaBilgiIslem> kasaBilgiIslem = new List<KasaBilgiIslem>();
            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (SqlCommand getKasaBilgiIslem = new SqlCommand(@"select 
                (select KasaAdi from Kasa where ID = Bakiye_Kasa.KasaID) as KasaAdi,
                (Cast(Bakiye as nvarchar) + ' ' + ParaBirimi) as Islem
                from Bakiye_Kasa", connection))
                {
                    using (SqlDataReader dr = getKasaBilgiIslem.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            KasaBilgiIslem kbilgiIslem = new KasaBilgiIslem();
                            kbilgiIslem.KasaAdi = Convert.ToString(dr["KasaAdi"]);
                            kbilgiIslem.Islem = Convert.ToString(dr["Islem"]);
                            kasaBilgiIslem.Add(kbilgiIslem);
                        }
                    }
                }
            }

            return Json(new { data = kasaBilgiIslem.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBankaBilgiIslem()
        {
            List<BankaBilgiIslem> bankaBilgiIslem = new List<BankaBilgiIslem>();
            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (SqlCommand getBankaBilgiIslem = new SqlCommand(@"select 
                (select BankaAdi from Banka where ID = Bakiye_Banka.BankaID) as BankaAdi ,
                (Cast(Bakiye as nvarchar) + ' ' + ParaBirimi) as Islem
                from Bakiye_Banka", connection))
                {
                    using (SqlDataReader dr = getBankaBilgiIslem.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            BankaBilgiIslem bbilgiIslem = new BankaBilgiIslem();
                            bbilgiIslem.BankaAdi = Convert.ToString(dr["BankaAdi"]);
                            bbilgiIslem.Islem = Convert.ToString(dr["Islem"]);
                            bankaBilgiIslem.Add(bbilgiIslem);
                        }
                    }
                }
            }
            return Json(new { data = bankaBilgiIslem.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult KasaEkle(KasaBilgi model)
        {
            int sonuc = 0;
            string mesaj = "";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                using (SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("select top 1 * from Kasa", connection))
                {
                    using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "Kasa");
                        DataRow dr = ds.Tables["Kasa"].NewRow();


                        dr["KasaKodu"] = model.KasaKodu;
                        dr["KasaAdi"] = model.KasaAdi;

                        ds.Tables["Kasa"].Rows.Add(dr);
                        da.Update(ds, "Kasa");
                        sonuc = 1;
                        mesaj = "Kasa Eklendi";
                    }
                }
            }
            return Json(new { result = sonuc, message = mesaj }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BankaEkle(BankaBilgi model)
        {
            int sonuc = 0;
            string mesaj = "";
            using(SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                using(SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("select top 1 * from Banka",connection))
                {
                    using(SqlCommandBuilder cb = new SqlCommandBuilder(da))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "Banka");
                        DataRow dr = ds.Tables["Banka"].NewRow();

                        dr["BankaKodu"] = model.BankaKodu;
                        dr["BankaAdi"] = model.BankaAdi;
                        dr["SubeAdi"] = model.SubeAdi;
                        dr["HesapNo"] = model.HesapNo;
                        dr["Iban"] = model.Iban;

                        ds.Tables["Banka"].Rows.Add(dr);
                        da.Update(ds, "Banka");
                        sonuc = 1;
                        mesaj = "Banka Eklendi";
                    }
                }
            }
            return Json(new { result = sonuc, message = mesaj }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult IslemEkle(Islem model)
        {
            int sonuc = 0;
            string mesaj = "";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                using (SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("select top 1 * from Tahsilat_Odeme", connection))
                {
                    using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                    {
                        DataSet ds = new DataSet();
                        da.Fill(ds, "Tahsilat_Odeme");
                        DataRow dr = ds.Tables["Tahsilat_Odeme"].NewRow();

                        dr["IslemTipi"] = model.IslemTipi;
                        dr["IslemNo"] = model.IslemNo;
                        dr["IslemTarih"] = DateTime.Now;
                        if(model.IslemTipi == "T" || model.IslemTipi == "O")
                        {
                            dr["KasaID"] = model.KasaBankaID;
                        }
                        else 
                        {
                            dr["BankaID"] = model.KasaBankaID;
                        }            
                        dr["Aciklama"] = model.Aciklama;
                        dr["ParaBirimi"] = model.ParaBirimi;
                        dr["Tutar"] = model.Tutar;


                        ds.Tables["Tahsilat_Odeme"].Rows.Add(dr);
                        da.Update(ds, "Tahsilat_Odeme");
                        sonuc = 1;
                        mesaj = "İşlem Eklendi";
                    }
                }

                // Kasa İşlemi Kasa Bakiye Tablosuna da Kayıt edilecek.
                if(model.IslemTipi == "T" || model.IslemTipi == "O")
                {
                    using (SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("select top 1 * from Bakiye_Kasa", connection))
                    {
                        using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "Bakiye_Kasa");
                            DataRow dr = ds.Tables["Bakiye_Kasa"].NewRow();

                            dr["KasaID"] = model.KasaBankaID;
                            dr["ParaBirimi"] = model.ParaBirimi;
                            if(model.IslemTipi == "T")
                            {
                                dr["Bakiye"] = model.Tutar;
                            }else
                            {
                                dr["Bakiye"] = model.Tutar * -1;
                            }

                            ds.Tables["Bakiye_Kasa"].Rows.Add(dr);
                            da.Update(ds, "Bakiye_Kasa");
                        }
                    }
                }

                else
                {
                    // Banka İşlemi Banka Bakiye Tablosuna da Kayıt edilecek.

                    using (SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter("select top 1 * from Bakiye_Banka", connection))
                    {
                        using (SqlCommandBuilder cb = new SqlCommandBuilder(da))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds, "Bakiye_Banka");
                            DataRow dr = ds.Tables["Bakiye_Banka"].NewRow();


                            dr["BankaID"] = model.KasaBankaID;
                            dr["ParaBirimi"] = model.ParaBirimi;
                            if (model.IslemTipi == "G")
                            {
                                dr["Bakiye"] = model.Tutar;
                            }
                            else
                            {
                                dr["Bakiye"] = model.Tutar * -1;
                            }


                            ds.Tables["Bakiye_Banka"].Rows.Add(dr);
                            da.Update(ds, "Bakiye_Banka");
                        }
                    }
                }

            }
            return Json(new { result = sonuc, message = mesaj }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IslemSil(int IslemID=0)
        {
            int sonuc = 0;
            string mesaj = "";
            using (SqlConnection connection = new SqlConnection(cs))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                using (SqlCommand command = new SqlCommand("Delete Tahsilat_Odeme where ID = "+IslemID+" ", connection))
                {
                    command.ExecuteNonQuery();
                    sonuc = 1;
                    mesaj = "İşlem Silindi";
                }
            }
            return Json(new { result = sonuc, message = mesaj }, JsonRequestBehavior.AllowGet);
        }
    }
}