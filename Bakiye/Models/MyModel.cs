using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bakiye.Models
{
    public class Islem
    {
        public int ID { get; set; }
        public string IslemTipi { get; set; }
        public string IslemNo { get; set; }
        public string IslemTarih { get; set; }
        public int KasaBankaID { get; set; }
        public string Aciklama { get; set; }
        public string ParaBirimi { get; set; }
        public decimal Tutar { get; set; }
        public string KasaBankaAdi { get; set; }
    }

    public class KasaBilgi
    {
        public int ID { get; set; }
        public string KasaKodu { get; set; }
        public string KasaAdi { get; set; }
        public decimal Bakiye_TL { get; set; }
        public decimal Bakiye_USD { get; set; }
        public decimal Bakiye_EUR { get; set; }
        public decimal Bakiye_GBP { get; set; }
    }

    public class BankaBilgi
    {
        public int ID { get; set; }
        public string BankaKodu { get; set; }
        public string BankaAdi { get; set; }
        public string SubeAdi { get; set; }
        public string HesapNo { get; set; }
        public string Iban { get; set; }
        public decimal Bakiye_TL { get; set; }
        public decimal Bakiye_USD { get; set; }
        public decimal Bakiye_EUR { get; set; }
        public decimal Bakiye_GBP { get; set; }
    }

    public class KasaBilgiIslem
    {
        public string KasaAdi { get; set; }
        public string Islem { get; set; }
    }

    public class BankaBilgiIslem
    {
        public string BankaAdi { get; set; }
        public string Islem { get; set; }
    }

}