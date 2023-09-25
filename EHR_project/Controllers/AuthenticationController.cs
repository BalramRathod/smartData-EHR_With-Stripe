using Dapper;
using EHR_project.Data;
using EHR_project.Dto;
using EHR_project.encryption;
using EHR_project.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EHR_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DBContext _context;

        private DapperContext _Connectionstring;
        private IWebHostEnvironment _webHostEnvironment;

        public AuthenticationController(DBContext context, DapperContext connectionstring, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _Connectionstring = connectionstring;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/Authentication
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> Getusers()
        {
            if (_context.users == null)
            {
                return NotFound();
            }
            return await _context.users.ToListAsync();
        }


        [HttpPost("register")]
        public async Task<ActionResult<Users>> PostUsers(RegisterDto userdto)
        {
            if (_context.users == null)
            {
                return Problem("Entity set 'DBContext.users'  is null.");
            }
            // _context.users.Add(users);
            var random = new Random();
            string str = "abcdefghijklmnopqrstuvwxyz";
            var rand_string = "";
            for (int i = 0; i < 7; i++)
            {
                rand_string = rand_string + str[random.Next(0, 26)];
            }
            var username= $"EHR-{userdto.FirstName.ToUpper()}{userdto.@LastName.Substring(0, 1).ToUpper()}{userdto.DOB.ToString("MMddyy")}";
            UsersDto users = new UsersDto();
            users.Username = username; 
            users.User_type = userdto.User_type;
            users.Password = rand_string;
            users.Phone = userdto.Phone;
            users.profile_Path = userdto.profile_Path;

            Users DBuser = users.Adapt<Users>();
            await _context.users.AddAsync(DBuser);

            var emailAuth = await _context.patient.Where(x => x.Email == userdto.Email).ToArrayAsync();
            var count = 0;
            count = emailAuth.Count();
            var emailAuth2 = await _context.provider.Where(x => x.Email == userdto.Email).ToArrayAsync();
            count = count + emailAuth2.Count();
            if (count == 0)
            {
                var result = _context.SaveChanges();

                if (result > 0)
                {
                    if (userdto.User_type == 2)
                    {

                        PatientDto patientDto = new PatientDto();
                        patientDto.UserId = DBuser.UserId;
                        patientDto.FirstName = userdto.FirstName;
                        patientDto.LastName = userdto.LastName;
                        patientDto.Address = userdto.Address;
                        patientDto.Email = userdto.Email;
                        patientDto.Phone = userdto.Phone;
                        patientDto.DOB = userdto.DOB;

                        patientDto.InsuranceNo = userdto.InsuranceNo;
                        Patient DBpatient = patientDto.Adapt<Patient>();
                        await _context.patient.AddAsync(DBpatient);
                        _context.SaveChanges();

                    }
                    if (userdto.User_type == 1)
                    {
                        ProviderDto providerDto = new ProviderDto();
                        providerDto.UserId = DBuser.UserId;
                        providerDto.First_name = userdto.FirstName;
                        providerDto.Last_name = userdto.LastName;
                        providerDto.Phone = userdto.Phone;
                        providerDto.Email = userdto.Email;
                        providerDto.DOB = (DateTime)userdto.DOB;
                        providerDto.Address = userdto.Address;

                        Provider DBprovider = providerDto.Adapt<Provider>();
                        await _context.provider.AddAsync(DBprovider);
                        _context.SaveChanges();


                    }
                    var otp = random.Next(1000, 9999);
                    OTP DBotp = new OTP();
                    DBotp.UserId = DBuser.UserId;
                    DBotp.Otp = otp;
                    await _context.otp.AddAsync(DBotp);
                    _context.SaveChanges();

                    sendEmail(rand_string, userdto.Email, DBuser.UserId, username, otp, userdto.FirstName);


                }
                // await _context.SaveChangesAsync();

                // return CreatedAtAction("GetUsers", new { id = users.UserId }, users); 
            }
            else
            {
                return Ok(new { Message = "emailfound" });
            }
            return Ok(new { Message = "register succesfull", DBuser.UserId });
        }



        [HttpPost("OTP")]
        public async Task<ActionResult> OtpValidation(OtpAuth otpAuth)
        {
            var id = EncryptDecrypt.Decrypt(otpAuth.UserId);

            var user = await _context.otp.FirstOrDefaultAsync(o => o.UserId == int.Parse(id) && o.Otp == otpAuth.Otp);
            if (user != null)
            {
                Users users = await _context.users.FindAsync(int.Parse(id));
                users.isValidate = true;
                _context.SaveChanges();
                return Ok(true);

            }
            return Ok(false);

        }

        [HttpPost("login")]
        public async Task<ActionResult> loginuser(LoginDto login)
        {
            //EncryptDecryptService obj = new EncryptDecryptService();

            var connection = _Connectionstring.CreateConnection();

            var query = "";
            query = "Select * from users where Username= @username and Password = @password";

            // var query2 = "Delete from otpVerifications where User_Id=@user_id";


            var dbUser = await connection.QueryFirstOrDefaultAsync<Users>(query, new { login.username, login.password });

            //   await connection.ExecuteAsync(query2, new { user_id = dbUser.User_Id });

            if (dbUser == null)
            {
                return Ok(new
                {
                    message = "Invalid Credentials! Please Enter Valid Details"
                });
            }

            Random ran = new Random();
            var otp = ran.Next(11111, 99999);



            var Token = CreateJwt(dbUser);
            var role = dbUser.User_type == 1 ? "Provider" : "Patient";
            var id = 0;


            if (dbUser.User_type == 1)
            {
                var cout = await _context.provider.FirstOrDefaultAsync(x => x.UserId == dbUser.UserId);
                query = "Insert into OTPVerification values(@User_Id, @otp)";
                var otpParams = new DynamicParameters();
                otpParams.Add("User_Id", dbUser.UserId, DbType.Int32);
                otpParams.Add("OTP", otp, DbType.String);
                await connection.ExecuteAsync(query, otpParams);
                id = cout.ProviderId;

                 SendsmsService sms = new SendsmsService(cout.Phone, otp);
                 sms.sendSms();

                return Ok(new
                {
                    Token = Token,
                    ProviderId = id,
                    user_Id = dbUser.UserId,
                    message = "success",
                    User_type = role
                });
            }

            if (dbUser.User_type == 2)
            {
                var cout = await _context.patient.FirstOrDefaultAsync(x => x.UserId == dbUser.UserId);
                query = "Insert into OTPVerification values(@User_Id, @otp)";
                var otpParams = new DynamicParameters();
                otpParams.Add("User_Id", dbUser.UserId, DbType.Int32);
                otpParams.Add("OTP", otp, DbType.String);
                await connection.ExecuteAsync(query, otpParams);
                id = cout.PatientId;


                   SendsmsService sms = new SendsmsService(cout.Phone, otp);
                  sms.sendSms();

                return Ok(new
                {
                    Token = Token,
                    PatientId = id,
                    user_Id = dbUser.UserId,
                    message = "success",
                    user_type = role
                });
            }

            return Ok();
        }

        [HttpPost("ValidateUsdsfdsafer")]
        public async Task<ActionResult> ValidateUser(OtpValidateDto data)
        {
            // EncryptDecryptService obj = new EncryptDecryptService();
            var connection = _Connectionstring.CreateConnection();
            var query = "";


            query = "select * from OTPVerification where UserId=@UserId and Otp=@Otp";

            var dbData = await connection.QuerySingleOrDefaultAsync(query, new { data.UserId, data.otp, data.Is_Validate });

            if (dbData == null)
            {
                connection.Close();
                return Ok(new
                {
                    message = false,

                });
            }

            query = "Delete from OTPVerification Where UserId=@UserId";
            await connection.ExecuteAsync(query, new { data.UserId });


            return Ok(new
            {
                message = true
            });
        }

        [NonAction]
        private string CreateJwt(Users user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("This Is SampleSecured Key.....");
            var role = user.User_type == 1 ? "Provider" : "Patient";
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Name, user.Username )
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = credentials
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            return jwtTokenHandler.WriteToken(token);
        }


        //Email API
        [HttpPost("sendemail")]
        public IActionResult sendEmail(string Password, string UserEmail, int id, string username, int otp, string FirstName)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("rupenjaychoudharysd@gmail.com"));
            email.To.Add(MailboxAddress.Parse(UserEmail));
            email.Subject = "One Time Password";
            string string_id = id.ToString();
            var Encrypt_id = EncryptDecrypt.Encrypt(string_id);

            var url = "http://localhost:4200/activeAccount?id=" + Encrypt_id;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = "<h1>Dear " + FirstName + ",</h1><br><h3>We are delighted to welcome you as a valued member of our Electronic Health Record . To facilitate your access and ensure a seamless login experience, we are providing you with your login credentials. <br><br>Below you will find your login information: <h2><br>Username: " + username + "<br> Password: " + Password + "<br> Otp: " +otp+"</h3></h3><br><br> <h3> Visit Application for login ..... <a style='color:blue' href=" + url + "> Electronic Health Record </a>. </h3><br><br><h4> Thank you for choosing Electronic Health Record. We look forward to providing you with a rewarding and enjoyable experience on our platform!</h4>"
            };
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("rupenjaychoudharysd@gmail.com", "ziccgncrunyqtxyo");
            smtp.Send(email);
            smtp.Disconnect(true);
            return Ok();
        }









    }

}

