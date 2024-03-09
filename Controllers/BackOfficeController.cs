using AlbergoDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlbergoDB.Controllers
{
   
    public class BackOfficeController : Controller
    {

        public ActionResult BackOffice()
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();

            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();

            var command = new SqlCommand("SELECT * FROM Prenotazione", conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    prenotazioni.Add(new Prenotazione
                    {
                        IDPrenotazione = reader.GetInt32(0),
                        DataPrenotazione = reader.GetDateTime(1),
                        NumeroProgressivoAnno = reader.GetInt32(2),
                        Anno = reader.GetInt32(3),
                        PeriodoDal = reader.GetDateTime(4),
                        PeriodoAl = reader.GetDateTime(5),
                        CaparraConfirmatoria = reader.GetDecimal(6),
                        TariffaApplicata = reader.GetDecimal(7),
                        Dettagli = reader.GetString(8),
                        IDCliente = reader.GetInt32(9),
                        IDCamere = reader.GetInt32(10)
                    });
                }
            }

            return View(prenotazioni);
        }
        public ActionResult AddCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCliente([Bind(Include = "IDCliente,CodiceFiscale,Cognome,Nome,Citta,Provincia,Email,Telefono,Cellulare")] Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();
                var command = new SqlCommand(@"
            INSERT INTO Cliente
            (CodiceFiscale, Cognome, Nome, Citta, Provincia, Email, Telefono, Cellulare)
            OUTPUT INSERTED.IDCliente
            VALUES (@codiceFiscale, @cognome, @nome, @citta, @provincia, @email, @telefono, @cellulare)
        ", conn);

                command.Parameters.AddWithValue("@codiceFiscale", cliente.CodiceFiscale);
                command.Parameters.AddWithValue("@cognome", cliente.Cognome);
                command.Parameters.AddWithValue("@nome", cliente.Nome);
                command.Parameters.AddWithValue("@citta", cliente.Citta);
                command.Parameters.AddWithValue("@provincia", cliente.Provincia);
                command.Parameters.AddWithValue("@email", cliente.Email);
                command.Parameters.AddWithValue("@telefono", cliente.Telefono);
                command.Parameters.AddWithValue("@cellulare", cliente.Cellulare);
                var clienteId = command.ExecuteScalar();

                return RedirectToAction("BackOffice");
            }

            return View(cliente);
        }

        public ActionResult AddPrenotazione()
        {
            List<Cliente> clienti = new List<Cliente>();
            List<Camera> camere = new List<Camera>();

            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();

            var command = new SqlCommand("SELECT * FROM Cliente", conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    clienti.Add(new Cliente { IDCliente = reader.GetInt32(0), Nome = reader.GetString(2) + " " + reader.GetString(3) + " " + reader.GetString(1)});
                }
            }

    
            command = new SqlCommand("SELECT * FROM Camera", conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    camere.Add(new Camera { IDCamere = reader.GetInt32(0), Descrizione = reader.GetString(1) });
                }
            }

            ViewBag.IDCliente = new SelectList(clienti, "IDCliente", "Nome");
            ViewBag.IDCamere = new SelectList(camere, "IDCamere", "Descrizione");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddPrenotazione([Bind(Include = "IDPrenotazione,DataPrenotazione,NumeroProgressivoAnno,Anno,PeriodoDal,PeriodoAl,CaparraConfirmatoria,TariffaApplicata,Dettagli,IDCliente,IDCamere")] Prenotazione prenotazione)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();

                int anno = prenotazione.PeriodoAl.Year;
                prenotazione.Anno = anno;

                var command1 = new SqlCommand("SELECT MAX(NumeroProgressivoAnno) FROM Prenotazione WHERE Anno = @anno", conn);
                command1.Parameters.AddWithValue("@anno", anno);
                var result = command1.ExecuteScalar();
                int ultimoNumeroProgressivo = result != DBNull.Value ? (int)result : 0;
                prenotazione.NumeroProgressivoAnno = ultimoNumeroProgressivo + 1;

                var commandCliente = new SqlCommand("SELECT Nome, Cognome FROM Cliente WHERE IDCliente = @idCliente", conn);
                commandCliente.Parameters.AddWithValue("@idCliente", prenotazione.IDCliente);
                using (var reader = commandCliente.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        prenotazione.Dettagli = reader.GetString(0) + " " + reader.GetString(1);
                    }
                }
                var command2 = new SqlCommand(@"
            INSERT INTO Prenotazione
            (DataPrenotazione,NumeroProgressivoAnno,Anno,PeriodoDal,PeriodoAl,CaparraConfirmatoria,TariffaApplicata,Dettagli,IDCliente,IDCamere)
            OUTPUT INSERTED.IDPrenotazione
            VALUES (@dataPrenotazione,@numeroProgressivoAnno,@anno,@periodoDal,@periodoAl,@caparraConfirmatoria,@tariffaApplicata,@dettagli,@idCliente,@idCamere)
        ", conn);

                command2.Parameters.AddWithValue("@dataPrenotazione", prenotazione.DataPrenotazione);
                command2.Parameters.AddWithValue("@numeroProgressivoAnno", prenotazione.NumeroProgressivoAnno);
                command2.Parameters.AddWithValue("@anno", prenotazione.Anno);
                command2.Parameters.AddWithValue("@periodoDal", prenotazione.PeriodoDal);
                command2.Parameters.AddWithValue("@periodoAl", prenotazione.PeriodoAl);
                command2.Parameters.AddWithValue("@caparraConfirmatoria", prenotazione.CaparraConfirmatoria);
                command2.Parameters.AddWithValue("@tariffaApplicata", prenotazione.TariffaApplicata);
                command2.Parameters.AddWithValue("@dettagli", prenotazione.Dettagli);
                command2.Parameters.AddWithValue("@idCliente", prenotazione.IDCliente);
                command2.Parameters.AddWithValue("@idCamere", prenotazione.IDCamere);
                var prenotazioneId = command2.ExecuteScalar();

                return RedirectToAction("BackOffice");
            }
            return View(prenotazione);
        }
        public ActionResult AddServizi()
        {
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();

            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            var command = new SqlCommand("SELECT * FROM Prenotazione", conn);
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    prenotazioni.Add(new Prenotazione { IDPrenotazione = reader.GetInt32(0), Dettagli = reader.GetString(8) });
                }
            }

            ViewBag.IDPrenotazione = new SelectList(prenotazioni, "IDPrenotazione", "Dettagli");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddServizi([Bind(Include = "IDServizi,Data,Quantita,Prezzo,IDPrenotazione")] Servizi servizio)
        {
            if (ModelState.IsValid)
            {
                string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
                var conn = new SqlConnection(connString);
                conn.Open();

                var command = new SqlCommand(@"
            INSERT INTO Servizi
            (Data,Quantita,Prezzo,IDPrenotazione)
            OUTPUT INSERTED.IDServizi
            VALUES (@data,@quantita,@prezzo,@idPrenotazione)
        ", conn);

                command.Parameters.AddWithValue("@data", servizio.Data);
                command.Parameters.AddWithValue("@quantita", servizio.Quantita);
                command.Parameters.AddWithValue("@prezzo", servizio.Prezzo);
                command.Parameters.AddWithValue("@idPrenotazione", servizio.IDPrenotazione);
                var servizioId = command.ExecuteScalar();

                return RedirectToAction("BackOffice");
            }
            return View(servizio);
        }
        [HttpGet]
        public ActionResult CheckOut(int id)
        {
            string connString = ConfigurationManager.ConnectionStrings["DbBlogConnection"].ToString();
            var conn = new SqlConnection(connString);
            conn.Open();

            // Recupera la prenotazione specifica
            var command = new SqlCommand("SELECT * FROM Prenotazione WHERE IDPrenotazione = @id", conn);
            command.Parameters.AddWithValue("@id", id);
            Prenotazione prenotazione = null;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    prenotazione = new Prenotazione
                    {
                        IDPrenotazione = reader.GetInt32(0),
                        DataPrenotazione = reader.GetDateTime(1),
                        NumeroProgressivoAnno = reader.GetInt32(2),
                        Anno = reader.GetInt32(3),
                        PeriodoDal = reader.GetDateTime(4),
                        PeriodoAl = reader.GetDateTime(5),
                        CaparraConfirmatoria = reader.GetDecimal(6),
                        TariffaApplicata = reader.GetDecimal(7),
                        Dettagli = reader.GetString(8),
                        IDCliente = reader.GetInt32(9),
                        IDCamere = reader.GetInt32(10)
                    };
                }
            }

            // Recupera il cliente associato
            command = new SqlCommand("SELECT * FROM Cliente WHERE IDCliente = @idCliente", conn);
            command.Parameters.AddWithValue("@idCliente", prenotazione.IDCliente);
            Cliente cliente = null;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    cliente = new Cliente
                    {
                        IDCliente = reader.GetInt32(0),
                        CodiceFiscale = reader.GetString(1),
                        Cognome = reader.GetString(2),
                        Nome = reader.GetString(3),
                        Citta = reader.GetString(4),
                        Provincia = reader.GetString(5),
                        Email = reader.GetString(6),
                        Telefono = reader.GetInt64(7),
                        Cellulare = reader.GetInt64(8)
                    };
                }
            }

            // Recupera la camera associata
            command = new SqlCommand("SELECT * FROM Camera WHERE IDCamere = @idCamere", conn);
            command.Parameters.AddWithValue("@idCamere", prenotazione.IDCamere);
            Camera camera = null;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    camera = new Camera
                    {
                        IDCamere = reader.GetInt32(0),
                        Descrizione = reader.GetString(1),
                        Tipologia = reader.GetString(2)
                    };
                }
            }

            // Recupera i servizi associati
            command = new SqlCommand("SELECT * FROM Servizi WHERE IDPrenotazione = @idPrenotazione", conn);
            command.Parameters.AddWithValue("@idPrenotazione", prenotazione.IDPrenotazione);
            List<Servizi> servizi = new List<Servizi>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    servizi.Add(new Servizi
                    {
                        IDServizi = reader.GetInt32(0),
                        Data = reader.GetDateTime(1),
                        Quantita = reader.GetInt32(2),
                        Prezzo = reader.GetDecimal(3),
                        IDPrenotazione = reader.GetInt32(4)
                    });
                }
            }

            // Passa i dati alla vista utilizzando ViewBag
            ViewBag.Prenotazione = prenotazione;
            ViewBag.Cliente = cliente;
            ViewBag.Camera = camera;
            ViewBag.Servizi = servizi;

            return View();
        }
    }
}