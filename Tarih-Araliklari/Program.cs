using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tarih_Araliklari
{
    class Program
    {
        static void Main(string[] args)
        {
            List<TarihInfo> tarihler = new List<TarihInfo>();
            tarihler.Add(new TarihInfo { Id = 1, BasTarih = new DateTime(2020, 10, 1), BitTarih = new DateTime(2020, 10, 10) });
            tarihler.Add(new TarihInfo { Id = 2, BasTarih = new DateTime(2020, 10, 11), BitTarih = new DateTime(2020, 10, 20) });
            tarihler.Add(new TarihInfo { Id = 3, BasTarih = new DateTime(2020, 10, 21), BitTarih = new DateTime(2020, 11, 10) });
            tarihler.Add(new TarihInfo { Id = 4, BasTarih = new DateTime(2020, 11, 11), BitTarih = new DateTime(2020, 11, 20) });

            Console.WriteLine("Önceki tarih aralıkları:");
            foreach (var t in tarihler.OrderBy(x => x.BasTarih).ThenByDescending(x => x.BitTarih))
            {
                Console.WriteLine($"{t.Id} - {DTCevir(t.BasTarih)} - {DTCevir(t.BitTarih)}");
            }
            Console.WriteLine();

            var bas = new DateTime(2020, 10, 15);
            var bit = new DateTime(2020, 11, 15);

            Console.WriteLine("*** Yeni eklenecek tarih aralığı: ***");
            Console.WriteLine($"{DTCevir(bas)} - {DTCevir(bit)}");
            Console.WriteLine();


            // Tüm kayıtlar üzerinde çalışmaktansa sadece bizi ilgilendiren
            // kayıtlar üzerinde çalışıyoruz.
            var oncekiKayitlar = (from x in tarihler
                                    where x.BitTarih >= bas &&
                                          x.BasTarih <= bit
                                    select x).ToList();

            // db kullanmadığımızdan Id leri elle atayacağız.
            var maxId = tarihler.Max(x => x.Id);

            var silinenler = new List<int>();
            var eklenenler = new List<TarihInfo>();
            var guncellenenler = new List<TarihInfo>();

            foreach (var p in oncekiKayitlar)
            {
                bool baslangicDegistir = false;
                bool bitisDegistir = false;
                bool sil = false;

                // 1
                if (bit < p.BasTarih) 
                continue;

                // 2
                else if (bas < p.BasTarih && bit == p.BasTarih)
                baslangicDegistir = true;

                // 3
                else if (bas < p.BasTarih && bit > p.BasTarih && bit < p.BitTarih)
                baslangicDegistir = true;

                // 4
                else if (bas == p.BasTarih && bit > p.BasTarih && bit < p.BitTarih)
                baslangicDegistir = true;

                // 5
                else if (bas > p.BasTarih && bit < p.BitTarih) 
                {
                    sil = true;
                    eklenenler.Add(new TarihInfo
                    {
                        BasTarih = p.BasTarih,
                        BitTarih = bas.AddDays(-1),
                        Id = ++maxId
                    });

                    eklenenler.Add(new TarihInfo
                    {
                        BasTarih = bit.AddDays(1),
                        BitTarih = p.BitTarih,
                        Id = ++maxId
                    });
                }

                // 6
                else if (bas > p.BasTarih && bit == p.BitTarih) 
                bitisDegistir = true;

                // 7
                else if (bas > p.BasTarih && bit > p.BitTarih) 
                bitisDegistir = true;

                // 8
                else if (bas == p.BitTarih && bit > p.BitTarih) 
                bitisDegistir = true;

                // 9
                else if (bas > p.BitTarih) 
                continue;

                // 10
                else if (bas == p.BasTarih && bit == p.BitTarih)  
                sil = true;

                // 11 
                else if (bas < p.BasTarih && bit > p.BitTarih) 
                sil = true;

                // 12
                else if (bas == p.BasTarih && bit > p.BitTarih) 
                sil = true;

                // 13
                else if (bas < p.BasTarih && bit == p.BitTarih) 
                sil = true;

                // 14
                else if (bas == bit && bas == p.BasTarih) 
                baslangicDegistir = true;

                // 15
                else if (bas == bit && bas == p.BitTarih) 
                bitisDegistir = true; 


                if (baslangicDegistir)
                {
                    p.BasTarih = bit.AddDays(1);
                    guncellenenler.Add(p);
                }
                else if (bitisDegistir)
                {
                    p.BitTarih = bas.AddDays(-1);
                    guncellenenler.Add(p);
                }

                if (p.BasTarih > p.BitTarih) sil = true;

                if (sil) silinenler.Add(p.Id);

            }

            if (silinenler.Count > 0)
            {
                Console.WriteLine("Silinen tarih aralıkları:");
                var silinecekTarihler = (from x in oncekiKayitlar
                                         where silinenler.Contains(x.Id)
                                         select x).ToList();
                foreach (var t in silinecekTarihler)
                {
                    Console.WriteLine($"{t.Id} - {DTCevir(t.BasTarih)} - {DTCevir(t.BitTarih)}");
                    tarihler.Remove(t);
                }
                Console.WriteLine();
            }

            if (eklenenler.Count > 0)
            {
                Console.WriteLine("Eklenen tarih aralıkları:");
                foreach (var t in eklenenler)
                {
                    Console.WriteLine($"{t.Id} - {DTCevir(t.BasTarih)} - {DTCevir(t.BitTarih)}");
                    tarihler.Add(t);
                }
                Console.WriteLine();
            }

            if (guncellenenler.Count > 0)
            {
                Console.WriteLine("Güncellenen tarih aralıkları:");
                foreach (var t in guncellenenler)
                {
                    Console.WriteLine($"{t.Id} - {DTCevir(t.BasTarih)} - {DTCevir(t.BitTarih)}");
                }
                Console.WriteLine();
            }

            // Yeni tarih aralığı ekleniyor.
            tarihler.Add(new TarihInfo
            {
                BasTarih = bas,
                BitTarih = bit,
                Id = ++maxId
            });

            Console.WriteLine("Sonraki tarih aralıkları:");
            foreach (var t in tarihler.OrderBy(x => x.BasTarih).ThenByDescending(x => x.BitTarih))
            {
                Console.WriteLine($"{t.Id} - {DTCevir(t.BasTarih)} - {DTCevir(t.BitTarih)}");
            }

            Console.ReadKey();
        }

        public static string DTCevir(DateTime t)
        {
            return t.ToString("dd.MM.yyyy");
        }
    }
}
