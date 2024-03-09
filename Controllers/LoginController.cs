using AlbergoDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AlbergoDB.Controllers
{

    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated) return RedirectToAction("Prova");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Dipendenti dipendente)
        {
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();
            var command = new SqlCommand(@"
            SELECT *
            FROM Dipendenti
            WHERE Nome = @username AND Password = @password
        ", conn);
            command.Parameters.AddWithValue("@username", dipendente.Nome);
            command.Parameters.AddWithValue("@password", dipendente.Password);
            var reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                reader.Read();
                FormsAuthentication.SetAuthCookie(reader["IDDipendente"].ToString(), true);
                return RedirectToAction("BackOffice", "BackOffice"); // TODO: alla pagina di pannello
            }

            TempData["ErrorLogin"] = "Nome utente o password errati. Riprova.";
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult Prova()
        {
            var dipendenteId = HttpContext.User.Identity.Name;
            ViewBag.DipendenteId = dipendenteId;
            return View();
        }

        [Authorize, HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }

}