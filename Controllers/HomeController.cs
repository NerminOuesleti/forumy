using Forumy.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Drawing.Printing;

namespace Forumy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            // Handle user login and authentication using MySQL connection
            if (Auth(user.Username, user.Password))
            {
                TempData["SuccessMessage"] = "Login successful!";

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Authentication failed, return to the login page with an error message
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                return View(user);
            }
        }

        private bool Auth(string username, string password)
        {
            using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                connection.Open();

                string query = "SELECT * FROM users WHERE Username = @Username AND Password = @Password";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            // Handle user login and authentication using MySQL connection
            if (RegisterUser(user))
            {
                TempData["SuccessMessage"] = "Register successful!";

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Authentication failed, return to the login page with an error message
                TempData["FailedMessage"] = "Register Failed , Check Your Information!";
                return View(user);
            }
        }
        private bool RegisterUser(User user)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    connection.Open();

                    string query = "INSERT INTO Users (Username,Email , Password) VALUES (@Username,@Email, @Password)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Username", user.Username);
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@Password", user.Password);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return false;
            }
        
    }
    
    public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}