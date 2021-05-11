using ConsoleTables;
using DVDStore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVDStore
{
    class Program
    {
      public List<Movie> Movies=new List<Movie>();
      public  List<Client> Clients=new List<Client>();
      public  List<RentedMovie> RentedMovies=new List<RentedMovie>();
      public  DbContext db=new DbContext();
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Start();
        }
        void Start()
        {
            Movies = new List<Movie>();
            Clients = new List<Client>();
            RentedMovies = new List<RentedMovie>();
            Movies = db.GetMoviesCopy();
            Clients = db.GetClientList();
            RentedMovies = db.GetRentedList();
            MainMenu();

        }

        private void MainMenu()
        {
            int choice;
            do
            {
                Console.Clear();
                var strArray = new string[] { "1- Add Client", "2- List Clients",
                                          "3- List Copies","4- Add New Copy",
                                          "5- Rent Copy","6- Return Copy",
                                          "7- Client History","8- List Overdue Books","9- Statistics","10- Exit" };

                Console.WriteLine("-----------WELCOME TO DVD STORE MANAGEMENT SYSTEEM-----------");
                Console.WriteLine("\n");
                strArray.ToList().ForEach(Console.WriteLine);
                Console.WriteLine("--------------------------------------------------------------");
                Console.Write("\nSelect your choice: ");
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.Write("\nInvalid Choice, Please enter valid choice: ");
                }
                switch (choice)
                {
                    case 1:
                        AddClient();
                        break;
                    case 2:
                        PrintClientList();
                        break;
                    case 3:
                        PrintCopiesAvailable();
                        break;
                    case 4:
                        AddMovie();
                        break;
                    case 5:
                        RentCopy();
                        break;
                    case 6:
                        ReturnCopy();
                        break;
                    case 7:
                        PrintHistory();
                        break;
                    case 8:
                        PrintOverDueRent();
                        break;
                    case 9:
                        PrintStats();
                        break;

                }
            } while (choice != 10);
            
          
        }

        private void PrintStats()
        {
            DateTime fromDate, toDate;
            Console.Write("\nEnter From Date e.g '2020-12-01': ");
            while (!DateTime.TryParse(Console.ReadLine(), out fromDate))
            {
                Console.Write("\nInvalid Date Enter date according to Formate Given: ");

            }
            Console.Write("\nEnter To Date e.g '2021-01-01': ");
            while (!DateTime.TryParse(Console.ReadLine(), out toDate))
            {
                Console.Write("\nInvalid Date Enter date according to Formate Given: ");

            }
            var list = RentedMovies.Where(x => x.StartDate >= fromDate && x.StartDate <= toDate).ToList();
            Console.WriteLine();
            Console.WriteLine("{0}{1}", "Total Rentals== " + list.Count,"\nTotal Price of rented movies== "+list.Sum(x=>x.RentRate));
            Console.ReadKey();
        }

        private void PrintOverDueRent()
        {
           var list= RentedMovies.Where(x => (DateTime.Now- x.StartDate).TotalDays >= 14 && x.Status == true).ToList();
            ConsoleTable.From<RentedMovie>(list).Write();
            Console.ReadKey();
        }

        //Get Client Details and save to db
        private void AddClient()
        {
            Client client=new Client();
            Console.Write("Enter Client Name: ");
            client.Name = Console.ReadLine();
            while(client.Name.Trim().Length<=0)
            {
                Console.Write("\nInvalid Client Name, Enter valid Name");
                client.Name = Console.ReadLine();
            }
            Console.Write("\nEnter Client Address");
            client.Address = Console.ReadLine();
            db.AddClient(client);
            Clients = db.GetClientList();
            Console.WriteLine("\nTransaction Success!");
            Console.ReadKey();
        }
        //Get Movie Copy Details and save to db
        private void AddMovie()
        {
            int copies;
            double rate;
            Movie movie = new Movie();
            Console.Write("Enter Copy Title: ");
            movie.Title = Console.ReadLine();
            while (movie.Title.Trim().Length <= 0)
            {
                Console.Write("\nInvalid Copy Name, Enter valid Name: ");
                movie.Title = Console.ReadLine();
            }
            Console.Write("\nEnter Number of Copies Available: ");
            while (!int.TryParse(Console.ReadLine(),out copies))
            {
                Console.Write("\nInvalid Copies, Enter valid Copies count: ");
                
            }
            Console.Write("\nEnter Copy Rent Rate: ");
            while (!double.TryParse(Console.ReadLine(), out rate))
            {
                Console.Write("\nInvalid Rent Rate, Enter valid Rate: ");

            }
            movie.RentRate = rate;
            movie.CopiesAvailable = copies;
            db.AddMovieCopy(movie);
            Movies = db.GetMoviesCopy();
            Console.WriteLine("\nTransaction Success!");
            Console.ReadKey();

        }
        //PRINT Copies Available Details on screen
         void PrintCopiesAvailable()
        {
            ConsoleTable.From<Movie>(Movies).Write();
            Console.WriteLine("Press any key to return to main menu");
            Console.ReadKey();
        }
        //Print Client details available in db
        void PrintClientList()
        {
            ConsoleTable.From<Client>(Clients).Write();

            Console.WriteLine("Press any key to return to main menu");
            Console.ReadKey();
        }
        //Rent A copy to clietn
        void RentCopy()
        {
            int clientId, movieId;
            RentedMovie movie = new RentedMovie();
            Console.Write("\nEnter Client ID: ");
            while (!int.TryParse(Console.ReadLine(), out clientId))
            {
                Console.Write("\nInvalid Client ID, Enter valid Client ID: ");

            }
            if(Clients.Any(x=>x.ClientId==clientId))
            {
                Console.Write("\nEnter Copy ID: ");
                while (!int.TryParse(Console.ReadLine(), out movieId))
                {
                    Console.Write("\nInvalid Copy ID, Enter valid Copy ID: ");

                }
                if(Movies.Any(x=>x.MovieId==movieId&&x.CopiesAvailable>=1))
                {
                   if(!RentedMovies.Any(x=>x.MovieId==movieId&&x.ClientId==clientId&&x.Status==true))
                    {
                        movie.ClientId = clientId;
                        movie.MovieId = movieId;
                        db.RentMovieCopy(movie);
                        RentedMovies = db.GetRentedList();
                        Console.WriteLine("\nTransaction Success!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Customer has already rented the same copy can'y rent same copy again, Turning Back to Main Menu");
                        Console.ReadKey();
                        MainMenu();
                    }

                }
                else
                {
                    Console.WriteLine("Copy ID Not Found or it can be out of stock, Turning Back to Main Menu");
                    Console.ReadKey();
                    MainMenu();
                }
            }
            else
            {
                Console.WriteLine("Client ID Not Found, Turning Back to Main Menu");
                Console.ReadKey();
                MainMenu();
            }
        }
        //PRINT CLient active and historic rentals
        void PrintHistory()
        {
            int clientId;
            Console.Write("\nEnter Client ID: ");
            while (!int.TryParse(Console.ReadLine(), out clientId))
            {
                Console.Write("\nInvalid Client ID, Enter valid Client ID: ");

            }
            if (Clients.Any(x => x.ClientId == clientId))
            {
                Console.WriteLine("-------------ACTIVE RENTAL-----------");
                //Console.WriteLine("CLIENT\t\t|COPY\t\t\t|Rental Date");
                ConsoleTable.From<RentedMovie>(RentedMovies.Where(x => x.ClientId == clientId && x.Status == true).ToList()).Write();

                Console.WriteLine("\n-------------HISTORIC RENTAL-----------");
               
                    ConsoleTable.From<RentedMovie>(RentedMovies.Where(x => x.ClientId == clientId&&x.Status==false).ToList()).Write();
                    
                Console.WriteLine("Press any key to return to main menu");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Client Details Not Found");
                Console.ReadKey();
                MainMenu();
            }
               

        }
        //Return Copy
        void ReturnCopy()
        {
            int clientId, movieId;
            RentedMovie movie = new RentedMovie();
            Console.Write("\nEnter Client ID: ");
            while (!int.TryParse(Console.ReadLine(), out clientId))
            {
                Console.Write("\nInvalid Client ID, Enter valid Client ID: ");

            }
            if (Clients.Any(x => x.ClientId == clientId))
            {
                Console.Write("\nEnter Copy ID: ");
                while (!int.TryParse(Console.ReadLine(), out movieId))
                {
                    Console.Write("\nInvalid Copy ID, Enter valid Copy ID: ");

                }
                if (Movies.Any(x => x.MovieId == movieId))
                {
                    if(RentedMovies.Any(x=>x.ClientId==clientId&&x.MovieId==movieId&&x.Status==true))
                    {
                        movie.ClientId = clientId;
                        movie.MovieId = movieId;
                        db.ReturnMovieCopy(movie);
                        RentedMovies = db.GetRentedList();
                        Console.WriteLine("\nTransaction Success!");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Rental Details Not Found");
                        Console.ReadKey();
                        MainMenu();
                    }
                }
                else
                {
                    Console.WriteLine("Copy ID Not Found, Turning Back to Main Menu");
                    Console.ReadKey();
                    MainMenu();
                }
            }
            else
            {
                Console.WriteLine("Client ID Not Found, Turning Back to Main Menu");
                Console.ReadKey();
                MainMenu();
            }
        }
    }
}
