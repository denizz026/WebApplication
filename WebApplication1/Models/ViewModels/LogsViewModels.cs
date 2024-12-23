using WebApplication1.Models;

namespace CinemaReview.Models
{
	public class LogViewModel
	{
		public List<Logs> Logs { get; set; } // Şu anki sayfanın logları
		public int CurrentPage { get; set; } // Şu anki sayfa numarası
		public int TotalPages { get; set; } // Toplam sayfa sayısı
	}

}