
using Chemistry_laboratory_management.Dtos;
using laboratory.BLL.Services;
using laboratory.DAL.Data.context;
using laboratory.DAL.Models.Identity;
using LinkDev.Facial_Recognition.BLL.Helper.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using System.Threading.Tasks;

namespace Chemistry_laboratory_management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _user;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthServices _auth;
        private readonly LaboratoryDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> user, SignInManager<AppUser> signInManager, IAuthServices auth , LaboratoryDbContext  context , IConfiguration configuration)
        {
            _user = user;
            _signInManager = signInManager;
            _auth = auth;
            _context = context;
            _configuration = configuration;

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var CheckEmaile = await _user.FindByEmailAsync(model.Email);
            if (CheckEmaile == null)
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(CheckEmaile, model.Password, false);
            if (result.Succeeded is false)
                return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto()
            {
                DisplayName = CheckEmaile.DisplayName,
                Email = CheckEmaile.Email,
                Token = await _auth.CreateTokenAsync(CheckEmaile, _user)

            });
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            // التحقق إذا كان الإيميل موجود مسبقًا في نظام المستخدمين
            var existingUser = await _user.FindByEmailAsync(model.Email);
            if (existingUser != null)
                return BadRequest(new ApiResponse(400, "Email is already registered."));

            // قراءة إيميلات الإدمن من `appsettings.json`
            var adminEmails = _configuration.GetSection("AdminEmails").Get<List<string>>();

            // تحديد الـ Role بناءً على البيانات
            string role = "Student"; // الافتراضي طالب
            if (await _context.Doctors.AnyAsync(d => d.Email == model.Email))
                role = "DoctorName"; // لو الإيميل موجود في جدول الدكاترة
            else if (adminEmails.Contains(model.Email))
                role = "Admin"; // لو الإيميل موجود في قائمة الإدمنات

            // إنشاء الحساب الجديد
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _user.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400));

            // تعيين الـ Role المناسب
            await _user.AddToRoleAsync(user, role);

            return Ok(new UserDto()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                Token = await _auth.CreateTokenAsync(user, _user)
            });
        }


    }
}
