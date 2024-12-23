using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.ViewModels;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;
    private readonly LogsController _log;
    public AuthController(AppDbContext context, EmailService emailService,LogsController log)
    {
        _log    =log;
        _context = context;
        _emailService = emailService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }



    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model); // Eğer ModelState geçerli değilse, formu tekrar göster
        }

        // E-posta adresinin zaten kullanılıp kullanılmadığını kontrol et
        if (await _context.Users.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
            TempData["ErrorMessage"] = "Bu e-posta adresi zaten kullanılıyor.";
            return View(model);
        }

        // Yeni kullanıcı oluştur
        var user = new User
        {
            Username = model.Username,
            Email = model.Email,
            PasswordHash = model.Password,
            IsActive = false // Hesap başlangıçta pasif olarak kaydediliyor
        };

        // Kullanıcıyı veritabanına ekle
        _context.Users.Add(user);
        await _context.SaveChangesAsync(); // UserId'nin oluşturulması için kaydet

        // Aktivasyon için token oluştur
        var token = new Token
        {
            Value = Guid.NewGuid().ToString(),
            ExpirationDate = DateTime.Now.AddMinutes(15), // Token 15 dakika geçerli
            IsActive = true,
            UserId = user.UserId // Token'i kullanıcıya bağla
        };

        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();

        // Doğrulama URL'si oluştur
        var verificationUrl = Url.Action("ActivateAccount", "Auth", new { token = token.Value }, Request.Scheme);

        // Kullanıcıya e-posta gönder
        await _emailService.SendEmailAsync(
            user.Email,
            "E-posta Doğrulama",
            $@"Merhaba {user.Username},

        Hesabınızı etkinleştirmek için aşağıdaki bağlantıyı kullanabilirsiniz. Bu bağlantı sizi hesap doğrulama sayfasına yönlendirecektir:

        {verificationUrl}

        Eğer bağlantıya tıklayamıyorsanız, lütfen yukarıdaki URL'yi tarayıcınıza kopyalayıp yapıştırın.

        Teşekkür ederiz,
        Destek Ekibiniz"
        );

        // Aktivasyon süreci için kullanıcıya bilgi mesajı göster
        TempData["SuccessMessage"] = "Kaydınız oluşturuldu. Lütfen hesabınızı aktif hale getirmek için e-postanızı kontrol edin.";
        await _log.AddLog(user.Email, "Kayıt", "Aktivasyon linki gönderildi");

        // Login sayfasına yönlendirme
        return RedirectToAction("Login", "Auth");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Veritabanında kullanıcıyı kontrol et
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || user.PasswordHash != model.Password)
            {
                // Kullanıcı bulunamadı veya şifre yanlış
                ViewData["ErrorMessage"] = "Geçersiz kullanıcı adı veya şifre.";
                return View(model);
            }

            // Kullanıcının aktif olup olmadığını kontrol et
            if (!user.IsActive)
            {
                ViewData["ErrorMessage"] = "Hesabınız henüz aktif değil. Lütfen e-postanızı kontrol ederek hesabınızı aktif hale getirin.";
                return View(model);
            }

            // Kullanıcı bulundu ve aktifse giriş işlemini gerçekleştir
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        ViewData["ErrorMessage"] = "Geçersiz giriş denemesi.";
        return View(model);
    }



    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ActivateAccount()
    {
        return View();
    }

   


    public async Task SignInActivation(User d)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }
        var token = new Token
        {
            Value = Guid.NewGuid().ToString(),
            ExpirationDate = DateTime.Now.AddMinutes(15), // 15 dakika
            IsActive = true, // Token geçerli
            User = d, // Token, bu kullanıcıya atanacak
        };

        if (await _context.Tokens.AnyAsync(x => x.User.UserId == d.UserId))
        {
            var oldToken = await _context.Tokens.FirstOrDefaultAsync(x => x.UserId == d.UserId);
            _context.Tokens.Remove(oldToken);
        }
        _context.Tokens.Add(token);
        await _context.SaveChangesAsync();

        // Doğrulama URL'si oluştur
        var verificationUrl = Url.Action("ActivateAccount", "Auth", new { token = token.Value }, Request.Scheme);

        // E-posta gönder
        await _emailService.SendEmailAsync(
            d.Email,
            "E-posta Doğrulama",
            $@"Merhaba {d.Username},

        Hesabınızı etkinleştirmek için aşağıdaki bağlantıyı kullanabilirsiniz. Bu bağlantı sizi hesap doğrulama sayfasına yönlendirecektir:

        {verificationUrl}

        Eğer bağlantıya tıklayamıyorsanız, lütfen yukarıdaki URL'yi tarayıcınıza kopyalayıp yapıştırın.

        Teşekkür ederiz,
        Destek Ekibiniz"
        );

        // Kullanıcıya bilgi mesajı ile Login sayfasına yönlendir
        await _log.AddLog(d.Email, "Kayıt", "Aktifleştirme linki yeniden gönderildi");
        TempData["SuccessMessage"] = "Kaydiniz olusturuldu. Lutfen hesabinizi aktif hale getirmek icin e-postanizi kontrol edin.";
    }

    [HttpPost]
    public async Task<IActionResult> ActivateAccount([FromBody] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Json(new { success = false, message = "Gecersiz istek: Token bos olamaz." });
        }

        // Token'ı ve ilişkili kullanıcıyı bul
        var userToken = await _context.Tokens.Include(t => t.User)
                                      .FirstOrDefaultAsync(t => t.Value == token && t.IsActive);

        // Token bulunamazsa veya geçersizse
        if (userToken == null)
        {
            return Json(new { success = false });
        }

        // Kullanıcıyı aktif hale getir ve token'ı geçersiz yap
        userToken.User.IsActive = true;
        userToken.IsActive = false;
        userToken.ExpirationDate = DateTime.Now; // Token süresini hemen dolmuş olarak ayarla gerek olmayabiilir

        await _context.SaveChangesAsync();

        // Başarılı bir şekilde hesap aktifleşti
        return Json(new { success = true, message = "Hesabınız basarıyla aktif hale getirildi!" });
    }
}
