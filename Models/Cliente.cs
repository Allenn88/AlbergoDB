using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlbergoDB.Models
{
    public class Cliente
    {
        public int IDCliente { get; set; }
        public string CodiceFiscale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Citta { get; set; }
        public string Provincia { get; set; }
        public string Email { get; set; }
        public long Telefono { get; set; }
        public long Cellulare { get; set; }
    }
}