using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlbergoDB.Models
{
    public class Servizi
    {
        public int IDServizi { get; set; }
        public DateTime Data { get; set; }
        public int Quantita { get; set; }
        public decimal Prezzo { get; set; }
        public int IDPrenotazione { get; set; }
    }
}