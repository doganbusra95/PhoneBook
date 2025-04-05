using System.Data;

namespace PhoneBookSharedTools.ExcelFileWorks
{
    public interface IExcelFileWorks
    {
        /// <summary>
        /// Gönderilen DataSet içindeki tüm DataTable sınıflarını ayrı birer excel sayfası
        /// olarak doldurarak geri gönderir.
        /// DataTable da STYLE isminde bir kolon açılırsa bu kolonu dosyaya basmaz. Bu satıra uygulanacak renk düzenini belirler.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        byte[] ExcelDosyasiGetir(DataSet dataSet);




    }
}
