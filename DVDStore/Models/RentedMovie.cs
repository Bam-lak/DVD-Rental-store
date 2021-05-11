using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDStore
{
    class RentedMovie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string MovieName { get; set; }
        public double RentRate { get; set; }
        public DateTime StartDate { get; set; }
        public bool Status { get; set; }
    }
}
