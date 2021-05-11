using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDStore
{
    class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int CopiesAvailable { get; set; }
        public double RentRate { get; set; }
    }
}
