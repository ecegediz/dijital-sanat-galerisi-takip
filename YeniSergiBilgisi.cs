using System;

namespace dijitalsanatgalerisi
{
    public class YeniSergiBilgisi
    {
        public int GalericiId { get; set; }

        public string SergiAdi { get; set; }
        public string SergiTuru { get; set; }
        public string SergiTemasi { get; set; }
        public string HedefKitle { get; set; }
        public int Kapasite { get; set; }

        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }

        public int EserSayisi { get; set; }
        public decimal GaleriKirasi { get; set; }
        public decimal EserBasiUcret { get; set; }

        // ✅ Read-only kalsın: set etmiyoruz, gerek yok
        public decimal ToplamFiyat => GaleriKirasi + (EserSayisi * EserBasiUcret);

        public int? SanatciID { get; set; }
        public string SergiciAdSoyad { get; set; }
        public string SergiciEposta { get; set; }
        public string SergiciSifre { get; set; }
    }
}
