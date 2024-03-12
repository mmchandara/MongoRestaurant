using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MongoRestaurant
{
    public partial class AdminForm : Form
    {
        static MongoClient dbClient = new MongoClient("BLANK");
        static IMongoDatabase db = dbClient.GetDatabase("Restaurantdb");
        static IMongoCollection<User> usersCollection = db.GetCollection<User>("Users");
        static IMongoCollection<MenuItem> menuCollection = db.GetCollection<MenuItem>("Menu");

        public AdminForm()
        {
            InitializeComponent();
        }
        private class User
        {
            [BsonId]
            public ObjectId Id { get; set; }
            [BsonElement("Username")]
            public string Username { get; set; }
            [BsonElement("Password")]
            public string Password { get; set; }
            [BsonElement("Role")]
            public string Role { get; set; }
            [BsonElement("AccessLevel")]
            public int AccessLevel { get; set; }
        }
        private void DisplayUsers()
        {
            var usersCollection = db.GetCollection<User>("Users");

            List<User> users = usersCollection.AsQueryable().ToList();
            dataGridView1.DataSource = users;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var usersCollection = db.GetCollection<User>("Users");

                User userDocument = new User
                {
                    Username = txtUser.Text,
                    Password = txtPass.Text,
                    Role = comboRole.Text,
                    AccessLevel = int.Parse(comboAccess.Text)
                };
                usersCollection.InsertOne(userDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            txtPass.Clear();
            comboRole.ResetText();
            comboAccess.ResetText();
            DisplayUsers();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var usersCollection = db.GetCollection<User>("Users");
                var filter = Builders<User>.Filter.Eq("Id", ObjectId.Parse(txtID.Text));
                var update = Builders<User>.Update.Set("Username", txtUser.Text).Set("Password", txtPass.Text).Set("Role", comboRole.Text).Set("AccessLevel", int.Parse(comboAccess.Text));

                usersCollection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtID.Clear();
            txtUser.Clear();
            txtPass.Clear();
            comboRole.ResetText();
            comboAccess.ResetText();
            DisplayUsers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var usersCollection = db.GetCollection<User>("Users");
                var filter = Builders<User>.Filter.Eq("Id", ObjectId.Parse(txtID.Text));

                usersCollection.DeleteOne(filter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtID.Clear();
            DisplayUsers();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            DisplayUsers();
        }

        //MENU CODE
        private class MenuItem
        {
            [BsonId]
            public ObjectId Id { get; set; }
            [BsonElement("Name")]
            public string Name { get; set; }
            [BsonElement("Category")]
            public string Category { get; set; }
            [BsonElement("Description")]
            public string Description { get; set; }
            [BsonElement("Price")]
            public decimal Price { get; set; }
        }
        private void DisplayMenu()
        {
            var menuCollection = db.GetCollection<MenuItem>("Menu");
            List<MenuItem> menuItems = menuCollection.AsQueryable().ToList();
            dataGridView2.DataSource = menuItems;
        }

        private void btnMenuAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var menuCollection = db.GetCollection<MenuItem>("Menu");

                MenuItem menuDocument = new MenuItem
                {
                    Name = txtMenuName.Text,
                    Category = txtMenuCat.Text,
                    Description = richDes.Text,
                    Price = decimal.Parse(txtMenuPrice.Text)
                };
                menuCollection.InsertOne(menuDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtMenuName.Clear();
            txtMenuCat.Clear();
            richDes.Clear();
            txtMenuPrice.Clear();
            DisplayMenu();
        }

        private void btnMenuEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var menuCollection = db.GetCollection<MenuItem>("Menu");
                var filter = Builders<MenuItem>.Filter.Eq("Id", ObjectId.Parse(txtMenuID.Text));
                var update = Builders<MenuItem>.Update.Set("Name", txtMenuName.Text).Set("Catergory", txtMenuCat.Text).Set("Description", richDes.Text).Set("Price", decimal.Parse(txtMenuPrice.Text));

                menuCollection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtMenuID.Clear();
            txtMenuName.Clear();
            txtMenuCat.Clear();
            richDes.Clear();
            txtMenuPrice.Clear();
            DisplayMenu();
        }

        private void btnMenuDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var menuCollection = db.GetCollection<MenuItem>("Menu");
                var filter = Builders<MenuItem>.Filter.Eq("Id", ObjectId.Parse(txtMenuID.Text));

                menuCollection.DeleteOne(filter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtMenuID.Clear();
            DisplayMenu();
        }

        private void btnMenuDisplay_Click(object sender, EventArgs e)
        {
            DisplayMenu();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtUser.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtPass.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            comboRole.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            comboAccess.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMenuID.Text = dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtMenuName.Text = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtMenuCat.Text = dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString();
            richDes.Text = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
            txtMenuPrice.Text = dataGridView2.Rows[e.RowIndex].Cells[4].Value.ToString();
        }
    }
}
