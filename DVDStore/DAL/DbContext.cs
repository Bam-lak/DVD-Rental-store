using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDStore.DAL
{
    class DbContext
    {
        string strCon= @"data source=(LocalDB)\MSSQLLocalDB;attachdbfilename=|DataDirectory|\Database.mdf;integrated security=True;MultipleActiveResultSets=True;";
        SqlConnection con;
        SqlCommand cmd;
        SqlDataReader sdr;
        public DbContext()
        {
            con = new SqlConnection(strCon);
        }
        public SqlConnection OpenConnection()
        {
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            return con;
        }
        public void AddClient(Client client)
        {
            cmd = new SqlCommand("INSERT INTO Clients Values('" + client.Name + "','" + client.Address + "')",OpenConnection());
            cmd.ExecuteNonQuery();
        }
        public void AddMovieCopy(Movie movie)
        {
            cmd = new SqlCommand("INSERT INTO Movies Values('" + movie.Title + "','" + movie.CopiesAvailable + "','"+movie.RentRate+"')", OpenConnection());
            cmd.ExecuteNonQuery();
        }
        public List<Movie> GetMoviesCopy()
        {
            List<Movie> movies = new List<Movie>();
            cmd = new SqlCommand("SELECT * FROM Movies", OpenConnection());
            sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                movies.Add(new Movie { MovieId=int.Parse(sdr[0].ToString()),
                                      Title = sdr["Title"].ToString(),
                                      RentRate=double.Parse(sdr["Rate"].ToString()),
                                      CopiesAvailable = int.Parse(sdr[2].ToString()) });
            }
            return movies;
        }
        public List<Client> GetClientList()
        {
            List<Client> clients = new List<Client>();
            cmd = new SqlCommand("SELECT * FROM Clients", OpenConnection());
            sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                clients.Add(new Client
                {
                    ClientId = int.Parse(sdr[0].ToString()),
                    Name = sdr["Name"].ToString(),
                    Address = sdr[2].ToString()
                });
            }
            return clients;
        }
        public List<RentedMovie> GetRentedList()
        {
            List<RentedMovie> rentedMovies = new List<RentedMovie>();
            cmd = new SqlCommand("SELECT rent.ClientId,rent.MovieId,m.Rate,c.Name,m.Title,rent.StartDate,rent.Status FROM RentedMovies rent INNER JOIN Clients c on rent.ClientId=c.Id INNER JOIN Movies m on rent.MovieId=m.Id", OpenConnection());
            sdr = cmd.ExecuteReader();
            while (sdr.Read())
            {
                rentedMovies.Add(new RentedMovie
                {
                    ClientId=int.Parse(sdr["ClientId"].ToString()),
                    MovieId = int.Parse(sdr["MovieId"].ToString()),
                    ClientName = sdr["Name"].ToString(),
                    MovieName = sdr["Title"].ToString(),
                    RentRate=double.Parse(sdr["Rate"].ToString()),
                    StartDate = DateTime.Parse(sdr["StartDate"].ToString()),
                    Status = bool.Parse(sdr["Status"].ToString())
                });
            }
            return rentedMovies;
        }
        public void RentMovieCopy(RentedMovie rent)
        {
            cmd = new SqlCommand("INSERT INTO RentedMovies Values('" + rent.ClientId + "','" + rent.MovieId + "','"+DateTime.Now+"',1)", OpenConnection());
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("UPDATE Movies SET Copies=Copies-1 WHERE Id='" + rent.MovieId + "'", OpenConnection());
            cmd.ExecuteNonQuery();
        }
        public void ReturnMovieCopy(RentedMovie rent)
        {
            cmd = new SqlCommand("UPDATE RentedMovies SET Status=0 WHERE ClientId='"+rent.ClientId+"' AND MovieId='"+rent.MovieId+"'", OpenConnection());
            cmd.ExecuteNonQuery();
            cmd = new SqlCommand("UPDATE Movies SET Copies=Copies+1 WHERE Id='" + rent.MovieId + "'", OpenConnection());
            cmd.ExecuteNonQuery();
        }
    }
}
