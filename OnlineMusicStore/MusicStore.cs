using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineMusicStore
{
    public static class MusicStore
    {
        public static void GetMenu()
        {
            Console.WriteLine("Добре дошли в онлайн музикален магазин Орфей");
            Console.WriteLine("1. Добави инструмент");
            Console.WriteLine("2. Регистрирай потребител");
            Console.WriteLine("3. Направи поръчка");
            Console.WriteLine("4. Добави ревю");
            Console.WriteLine("5. Обработи плащане");
            Console.WriteLine("6. Прегледай всички категории");
            Console.WriteLine("7. Прегледай всички инструменти");
            Console.WriteLine("8. Прегледай всички потребители");
            Console.WriteLine("9. Прегледай всички поръчки");
            Console.WriteLine("10. Прегледай артикули от поръчки");
            Console.WriteLine("11. Прегледай ревюта (филтрирани по инструмент)");
            Console.WriteLine("12. Прегледай плащания");
            Console.WriteLine("13. Изход");
            Console.Write("Изберете опция: ");
            int option = int.Parse(Console.ReadLine());
            while (true)
            {
                switch (option)
                {
                    case 1: AddInstrument(); break;
                    case 2: //AddUser(); break;
                    case 3: //AddOrder(); break;
                    case 4: //AddReview(); break;
                    case 5: //ProcessPayment(); break;
                    case 6: //ViewCategories(); break;
                    case 7: //ViewInstruments(); break;
                    case 8: //ViewUsers(); break;
                    case 9: //ViewOrders(); break;
                    case 10: //ViewOrderItems(); break;
                    case 11: //ViewReviews(); break;
                    case 12: //ViewPayments(); break;
                    case 13: Console.WriteLine("Изход от приложението. Благодаря!"); return;
                    default: Console.WriteLine("Невалиден избор на команда. Опитаите отново."); break;
                }
                option = int.Parse(Console.ReadLine());
            }
        }
        static int GetCategoryID(string categoryName)
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {
                string checkQuery = "SELECT CategoryID FROM Category WHERE Name = @Name";
                SqlCommand command = new SqlCommand(checkQuery, connection);
                using (command)
                {
                    command.Parameters.AddWithValue("@Name", categoryName);
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return (int)result;
                    }
                }
                string insertQuery = "INSERT INTO Category (Name) VALUES (@Name);";
                using (SqlCommand cmd = new SqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Name", categoryName);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public static void AddInstrument()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {
                try
                {                   
                    Console.Write("Въведете име на инструмента: ");
                    string name = Console.ReadLine();

                    Console.Write("Въведете цена: ");
                    decimal price = decimal.Parse(Console.ReadLine());

                    Console.Write("Въведете наличност: ");
                    int stock = int.Parse(Console.ReadLine());

                    Console.Write("Въведете категория на инструмента: ");
                    string categoryName = Console.ReadLine();
                    int categoryID = GetCategoryID(categoryName);

                    string addInstrumentQuery = "INSERT INTO Instruments (Name, Price, Stock, CategoryID) VALUES (@Name, @Price, @Stock, @CategoryID)";
                    SqlCommand addInstrumentCommand = new SqlCommand(addInstrumentQuery, connection);
                    using (addInstrumentCommand)
                    {
                        addInstrumentCommand.Parameters.AddWithValue("@Name", name);
                        addInstrumentCommand.Parameters.AddWithValue("@Price", price);
                        addInstrumentCommand.Parameters.AddWithValue("@Stock", stock);
                        addInstrumentCommand.Parameters.AddWithValue("@CategoryID", categoryID);
                        addInstrumentCommand.ExecuteNonQuery();
                    }
                    Console.Write("Инструментът е добавен успешно!");
                }
                catch (Exception ex)
                {
                    Console.Write("Грешка при добавяне на инструмент: " + ex.Message);
                }
            }
        }
        public static void AddUser()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {
                try
                {
                    Console.Write("Въведете името на потребителя: ");
                    string name = Console.ReadLine();

                    Console.Write("Въведете имейл на потребителя: ");
                    string email = Console.ReadLine();

                    string addUserQuery = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)";
                    SqlCommand insertUserCommand = new SqlCommand(addUserQuery, connection);
                    using (insertUserCommand)
                    {
                        insertUserCommand.Parameters.AddWithValue("@Name", name);
                        insertUserCommand.Parameters.AddWithValue("@Email", email);
                        insertUserCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine("Потребителят е добавен успешно!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Грешка при добавяне на потребител: " + ex.Message);
                }
            }
        }
        public static void AddOrder()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {
                try
                {
                    Console.Write("Въведете ID на потребителя:");
                    int userId = int.Parse(Console.ReadLine());

                    string addOrderQuery = "INSERT INTO Orders (UserId, OrderDate) VALUES (@UserId, GETDATE());";
                    SqlCommand insertOrderCommand = new SqlCommand(addOrderQuery, connection);
                    using (insertOrderCommand)
                    {
                        insertOrderCommand.Parameters.AddWithValue("@UserId", userId);
                        int orderId= insertOrderCommand.ExecuteNonQuery();
                        Console.WriteLine($"Поръчката е добавен успешно! ID на поръчката: {orderId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Грешка при добавяне на поръчка" + ex.Message);
                }
            }
        }
        public static void AddReview()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {
                try
                {                    
                    Console.Write("Въведете ID на потребителя: ");
                    int userID = int.Parse(Console.ReadLine());

                    Console.Write("Въведете ID на инструмента: ");
                    int instrumentID = int.Parse(Console.ReadLine());

                    Console.Write("Въведете рейтинг (1-5): ");
                    int rating = int.Parse(Console.ReadLine());

                    Console.Write("Въведете коментар: ");
                    string comment = Console.ReadLine();

                    string insertReviewQuery = "INSERT INTO Reviews (UserId, InstrumentId, Rating, Comment, ReviewDate) VALUES (@UserID, @InstrumentID, @Rating, @Comment, GETDATE())";
                    SqlCommand addReviewCommand = new SqlCommand(insertReviewQuery, connection);
                    using (addReviewCommand)
                    {
                        addReviewCommand.Parameters.AddWithValue("@UserId", userID);
                        addReviewCommand.Parameters.AddWithValue("@InstrumentId", instrumentID);
                        addReviewCommand.Parameters.AddWithValue("@Rating", rating);
                        addReviewCommand.Parameters.AddWithValue("@Comment", comment);
                        addReviewCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine("Ревюто е добавено успешно!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Грешка при добавяне на ревю: " + ex.Message);
                }
            }
        }
        public static void ProcessPayment()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                try
                {
                    connection.Open();
                    Console.Write("Въведете ID на поръчката: ");
                    int orderID = int.Parse(Console.ReadLine());
                    Console.Write("Въведете метод на плащане (напр. Карта, PayPal): ");
                    string method = Console.ReadLine();
                    Console.Write("Въведете сума: ");
                    decimal amount = decimal.Parse(Console.ReadLine());

                    string proccessPaymentQuery = "INSERT INTO Payment (OrderID, PaymentMethod, PaymentDate, Amount)" +
                        " VALUES (@OrderID, @PaymentMethod, GETDATE(), @Amount)";
                    SqlCommand processPaymentCommand = new SqlCommand(proccessPaymentQuery, connection);
                    using (processPaymentCommand)
                    {
                        processPaymentCommand.Parameters.AddWithValue("@OrderID", orderID);
                        processPaymentCommand.Parameters.AddWithValue("@PaymentMethod", method);
                        processPaymentCommand.Parameters.AddWithValue("@Amount", amount);
                        processPaymentCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine("Плащането е обработено успешно!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Грешка при обработка на плащане: " + ex.Message);
                }
            }
        }
    }
}