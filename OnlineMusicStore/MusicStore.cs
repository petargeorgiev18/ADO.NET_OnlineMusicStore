using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
                    case 2: AddUser(); break;
                    case 3: AddOrder(); break;
                    case 4: AddReview(); break;
                    case 5: ProcessPayment(); break;
                    case 6: ViewCategories(); break;
                    case 7: ViewInstruments(); break;
                    case 8: ViewUsers(); break;
                    case 9: ViewOrders(); break;
                    case 10: ViewOrderInstruments(); break;
                    case 11: ViewReviews(); break;
                    case 12: ViewPayments(); break;
                    case 13: Console.WriteLine("Изход от приложението. Благодаря!"); 
                        return;
                    default: Console.WriteLine("Невалиден избор на команда. Опитаите отново."); 
                        break;
                }
                option = int.Parse(Console.ReadLine());
            }
        }
        public static int GetCategoryID(string categoryName)
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {
                string checkQuery = "SELECT CategoryID FROM Categories WHERE Name = @Name";
                SqlCommand command = new SqlCommand(checkQuery, connection);
                using (command)
                {
                    command.Parameters.AddWithValue("@Name", categoryName);
                    var result = command.ExecuteScalar();

                    int res = Convert.ToInt32(result);
                    if (res != 0)
                    {
                        return res;
                    }
                }
                string insertQuery = "INSERT INTO Categories (Name) OUTPUT Inserted.CategoryId VALUES (@Name);";
                SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                using (insertCommand)
                {
                    insertCommand.Parameters.AddWithValue("@Name", categoryName);
                    var newid  = insertCommand.ExecuteScalar();
                    return Convert.ToInt32(newid);
                }
            }
        }
        public static void AddInstrument()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
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
        }
        public static void AddUser()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
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
        }
        public static void AddOrder()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
            {

                Console.Write("Въведете ID на потребителя: ");
                int userId = int.Parse(Console.ReadLine());

                Console.Write("Въведете ID на инструмента: ");
                int instrumentId = int.Parse(Console.ReadLine());

                string addOrderQuery = @"
                INSERT INTO Orders (UserId, OrderDate) 
                OUTPUT Inserted.OrderId
                VALUES (@UserId, GETDATE());";
                SqlCommand insertOrderCommand = new SqlCommand(addOrderQuery, connection);
                using (insertOrderCommand)
                {
                    insertOrderCommand.Parameters.AddWithValue("@UserId", userId);
                    var orderId = insertOrderCommand.ExecuteScalar();
                    int orderID = Convert.ToInt32(orderId);
                    Console.WriteLine($"Поръчката е добавена успешно! ID на поръчката: {orderID}");
                    string addOrderItemQuery = @"
                    INSERT INTO OrderInstruments ([OrderId], [InstrumentId]) 
                    VALUES (@OrderId, @InstrumentId);";
                    SqlCommand insertOrderItemCommand = new SqlCommand(addOrderItemQuery, connection);
                    using (insertOrderItemCommand)
                    {
                        insertOrderItemCommand.Parameters.AddWithValue("@OrderId", orderID);
                        insertOrderItemCommand.Parameters.AddWithValue("@InstrumentId", instrumentId);
                        insertOrderItemCommand.ExecuteNonQuery();
                    }
                    Console.WriteLine("Артикулът е добавен към поръчката.");
                }
                Console.WriteLine("Поръчката е завършена.");
            }
        }
        public static void AddReview()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            connection.Open();
            using (connection)
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
        }
        public static void ProcessPayment()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                Console.Write("Въведете ID на поръчката: ");
                int orderID = int.Parse(Console.ReadLine());
                Console.Write("Въведете метод на плащане (напр. Карта, PayPal): ");
                string method = Console.ReadLine();
                Console.Write("Въведете сума: ");
                decimal amount = decimal.Parse(Console.ReadLine());

                string proccessPaymentQuery = "INSERT INTO Payments (OrderID, PaymentMethod, PaymentDate, Amount)" +
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
        }
        public static void ViewCategories()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                string viewCategoriesQuery = "SELECT * FROM Categories";
                SqlCommand viewCategoriesCommand = new SqlCommand(viewCategoriesQuery, connection);
                using (SqlDataReader reader = viewCategoriesCommand.ExecuteReader())
                {
                    Console.WriteLine("Списък с Категории:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"CategoryID: {reader["CategoryID"]}, Име: {reader["Name"]}");
                    }
                }
            }
        }
        public static void ViewInstruments()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                string viewInstrumentsQuery = @"SELECT 
                                     i.InstrumentID, i.Name, i.Price, i.Stock, c.Name AS CategoryName 
                                     FROM Instruments i
                                     JOIN Categories c ON i.CategoryID = c.CategoryID";
                SqlCommand viewInstrumentsCommand = new SqlCommand(viewInstrumentsQuery, connection);
                using (SqlDataReader reader = viewInstrumentsCommand.ExecuteReader())
                {
                    Console.WriteLine("Списък с Инструменти:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["InstrumentID"]}," +
                            $" Име: {reader["Name"]}, Цена: {reader["Price"]}," +
                            $" Категория: {reader["CategoryName"]}," +
                            $" Наличност: {reader["Stock"]}");
                    }
                }
            }
        }
        public static void ViewUsers()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                string query = "SELECT * FROM Users";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Списък с Потребители:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID: {reader["UserID"]}, Име: {reader["Name"]}, Имейл: {reader["Email"]}");
                    }
                }
            }
        }
        public static void ViewOrders()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                string query = "SELECT * FROM Orders";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Списък с Поръчки:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Поръчка ID: {reader["OrderID"]}," +
                            $" Потребител ID: {reader["UserID"]}, Дата: {reader["OrderDate"]}");
                    }
                }
            }
        }
        public static void ViewOrderInstruments()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                string query = "SELECT * FROM OrderInstruments";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Списък с Артикули от Поръчки:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Поръчка ID: {reader["OrderID"]}," +
                            $" Инструмент ID: {reader["InstrumentID"]}, Количество: {reader["Quantity"]}");
                    }
                }
            }
        }
        public static void ViewReviews()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                Console.Write("Въведете име на инструмента за филтриране на ревюта: ");
                string name = Console.ReadLine();
                string query = @"SELECT * FROM Reviews r 
                                  JOIN Instruments i ON r.InstrumentId = i.InstrumentId 
                                  WHERE i.Name = @InstrumentName";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@InstrumentName", name);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Ревюта:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ReviewID: {reader["ReviewID"]}, Рейтинг: {reader["Rating"]}," +
                            $" Коментар: {reader["Comment"]}, Дата: {reader["ReviewDate"]}");
                    }
                }
            }
        }
        public static void ViewPayments()
        {
            SqlConnection connection = new SqlConnection(Connection.connectionString);
            using (connection)
            {
                connection.Open();
                string query = "SELECT * FROM Payments";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("Плащания:");
                    while (reader.Read())
                    {
                        Console.WriteLine($"ID на плащане: {reader["PaymentID"]}," +
                            $" ID на поръчка: {reader["OrderID"]}," +
                            $" Метод: {reader["PaymentMethod"]}, Сума: {reader["Amount"]}," +
                            $" Дата: {reader["PaymentDate"]}");
                    }
                }
            }
        }
    }
}
