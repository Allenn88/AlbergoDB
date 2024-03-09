using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlbergoDB.Models
{
    public class Prenotazione
    {
        public int IDPrenotazione { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public int NumeroProgressivoAnno { get; set; }
        public int Anno { get; set; }
        public DateTime PeriodoDal { get; set; }
        public DateTime PeriodoAl { get; set; }
        public decimal CaparraConfirmatoria { get; set; }
        public decimal TariffaApplicata { get; set; }
        public string Dettagli { get; set; }
        public int IDCliente { get; set; }
        public int IDCamere { get; set; }
    }
}