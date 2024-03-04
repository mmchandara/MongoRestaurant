using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace MongoRestaurant
{
    public partial class MainForm : Form
    {
        static MongoClient dbClient = new MongoClient("mongodb+srv://mitchelchandara:Dragonnov669@cluster0.lfw8wai.mongodb.net/");
        static IMongoDatabase db = dbClient.GetDatabase("Restaurantdb");
        static IMongoCollection<BsonDocument> usersCollection = db.GetCollection<BsonDocument>("Users");
        static IMongoCollection<BsonDocument> menuCollection = db.GetCollection<BsonDocument>("Menu");
        static IMongoCollection<BsonDocument> ordersCollection = db.GetCollection<BsonDocument>("Orders");
        static IMongoCollection<BsonDocument> tablesCollection = db.GetCollection<BsonDocument>("Tables");

        public MainForm()
        {
            InitializeComponent();
            var usersData = new List<BsonDocument>
            {
                new BsonDocument { {"Username", "Johndoe545" }, { "Password", "johndoeiscool" }, { "Role", "Admin" }, { "AccessLevel", 3 } },
                new BsonDocument { { "Username", "Mary01Jane" }, { "Password", "Mmary@134" }, { "Role", "Staff" }, { "AccessLevel", 1 } },
                new BsonDocument { { "Username", "Jane12" }, { "Password", "janjan" }, { "Role", "Manager" }, { "AccessLevel", 2 } }
            };
            usersCollection.InsertMany(usersData);

            var menuData = new List<BsonDocument>
            {
                new BsonDocument { { "Name", "Fried Rice" }, { "Category", "Entrée" }, { "Description", "Plate of Fried Rice" }, { "Price", 12.99 } },
                new BsonDocument { { "Name", "California Roll" }, { "Category", "Sushi" }, { "Description", "Sushi Roll with imitation crab, avocado, and cucumber" }, { "Price", 13.99 } },
                new BsonDocument { { "Name", "Crab Puffs" }, { "Category", "Appetizer" }, { "Description", "Deep fried crab ragoons" }, { "Price", 8.99 } }
            };
            menuCollection.InsertMany(menuData);

            var orderData = new List<BsonDocument>
            {
                new BsonDocument { { "UserID", 1 }, { "ItemList", "Item 1" }, { "TotalAmount", 15.25 }, { "Discounts", 0 } },
                new BsonDocument { { "UserID", 2 }, { "ItemList", "Item 2" }, { "TotalAmount", 11.34 }, { "Discounts", 5 } },
                new BsonDocument { { "UserID", 3 }, { "ItemList", "Item 3" }, { "TotalAmount", 16.25 }, { "Discounts", 15 } }
            };
            var tableData = new List<BsonDocument>
            {
                new BsonDocument { { "Status", "Available" }, { "ReservationDetails", "No Reservations" } },
                new BsonDocument { { "Status", "Occupied" }, { "ReservationDetails", "No Reservations" } },
                new BsonDocument { { "Status", "Reserved" }, { "ReservationDetails", "Reservations at 6pm" } }
            };
            tablesCollection.InsertMany(tableData);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("Username", txtUser.Text) & Builders<BsonDocument>.Filter.Eq("Password", txtPassword.Text);
                var user = usersCollection.Find(filter).FirstOrDefault();

                if (user != null)
                {
                    string role = user["Role"].AsString;

                    if (role == "Admin")
                    {
                        AdminForm adminForm = new AdminForm();
                        adminForm.Show();
                    }
                    else if (role == "Manager")
                    {
                        ManagerForm managerForm = new ManagerForm();
                        managerForm.Show();
                    }
                    else
                    {
                        StaffForm staffForm = new StaffForm();
                        staffForm.Show();
                    }
                }
                else
                {
                    MessageBox.Show("Invalid username or password");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
