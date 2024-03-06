using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MongoRestaurant
{
    public partial class ManagerForm : Form
    {
        static MongoClient dbClient = new MongoClient("mongodb+srv://mitchelchandara:Dragonnov669@cluster0.lfw8wai.mongodb.net/");
        static IMongoDatabase db = dbClient.GetDatabase("Restaurantdb");
        static IMongoCollection<Order> ordersCollection = db.GetCollection<Order>("Orders");
        static IMongoCollection<User> usersCollection = db.GetCollection<User>("Users");
        static IMongoCollection<BsonDocument> tablesCollection = db.GetCollection<BsonDocument>("Tables");
        public ManagerForm()
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
        private class Order
        {
            [BsonId]
            public ObjectId ID { get; set; }
            [BsonElement("UserID")]
            public int UserID { get; set; }
            [BsonElement("ItemList")]
            public string ItemList { get; set; }
            [BsonElement("TotalAmount")]
            public decimal TotalAmount { get; set; }
            [BsonElement("Discounts")]
            public decimal? Discounts { get; set; }
        }
        private void DisplayOrders()
        {
            var ordersCollection = db.GetCollection<Order>("Orders");
            List<Order> orders = ordersCollection.AsQueryable().ToList();
            dataGridView1.DataSource = orders;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var ordersCollection = db.GetCollection<Order>("Orders");
                Order newOrder = new Order
                {
                    UserID = int.Parse(txtUser.Text),
                    ItemList = txtOrderName.Text,
                    TotalAmount = decimal.Parse(txtAmount.Text)
                };
                ordersCollection.InsertOne(newOrder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            txtOrderName.Clear();
            txtDiscount.Clear();
            txtAmount.Clear();
            txtOrderID.Clear();
            DisplayOrders();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var filter = Builders<Order>.Filter.Eq("ID", ObjectId.Parse(txtOrderID.Text));
                var update = Builders<Order>.Update.Set("UserID", int.Parse(txtUser.Text)).Set("ItemList", txtOrderName.Text).Set("TotalAmount", decimal.Parse(txtAmount.Text));

                ordersCollection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            txtOrderName.Clear();
            txtDiscount.Clear();
            txtAmount.Clear();
            txtOrderID.Clear();
            DisplayOrders();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var deleteFilter = Builders<Order>.Filter.Eq("ID", ObjectId.Parse(txtOrderID.Text));
                ordersCollection.DeleteOne(deleteFilter);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            txtOrderName.Clear();
            txtDiscount.Clear();
            txtAmount.Clear();
            txtOrderID.Clear();
            DisplayOrders();
        }
        private decimal CalculateTotalAmount(ObjectId orderID)
        {
            decimal totalAmount = 0;

            try
            {
                var ordersCollection = db.GetCollection<Order>("Orders");
                var orderFilter = Builders<Order>.Filter.Eq("ID", orderID);
                var order = ordersCollection.Find(orderFilter).FirstOrDefault();

                if (order != null)
                {
                    totalAmount = order.TotalAmount;
                }
                else
                {
                    MessageBox.Show($"Order with OrderID {orderID} not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return totalAmount;
        }
        private decimal ApplyDiscount(decimal totalAmount, decimal discountPercentage)
        {
            decimal discountAmount = (totalAmount * discountPercentage) / 100;
            return totalAmount - discountAmount;
        }
        private void UpdateTotalAmountInDatabase(ObjectId orderId, decimal discountedAmount)
        {
            try
            {
                var ordersCollection = db.GetCollection<BsonDocument>("Orders");
                var filter = Builders<BsonDocument>.Filter.Eq("ID", orderId);
                var update = Builders<BsonDocument>.Update
                    .Set("TotalAmount", discountedAmount)
                    .Set("Discounts", decimal.Parse(txtDiscount.Text));

                var result = ordersCollection.UpdateOne(filter, update);

                if (result.ModifiedCount > 0)
                {
                    MessageBox.Show($"Discount applied successfully to OrderID {orderId}. New total amount: {discountedAmount:C}");
                }
                else
                {
                    MessageBox.Show($"Order with OrderID {orderId} not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                if (decimal.TryParse(txtDiscount.Text, out decimal discountPercentage))
                {
                    ObjectId orderId = ObjectId.Parse(txtOrderID.Text);

                    decimal totalAmount = CalculateTotalAmount(orderId);
                    decimal discountedAmount = ApplyDiscount(totalAmount, discountPercentage);

                    UpdateTotalAmountInDatabase(orderId, discountedAmount);
                }
                else
                {
                    MessageBox.Show("Invalid discount percentage. Please enter a valid decimal value.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            txtOrderName.Clear();
            txtDiscount.Clear();
            txtAmount.Clear();
            txtOrderID.Clear();
            DisplayOrders();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtOrderID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            txtUser.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtOrderName.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
            txtAmount.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
            object discountValue = dataGridView1.Rows[e.RowIndex].Cells[4].Value;
            txtDiscount.Text = discountValue != null ? discountValue.ToString() : string.Empty;
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtRoleID.Text = dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString();
            comboRole.Text = dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString();
        }

        private void btnTable_Click(object sender, EventArgs e)
        {
            TableForm tableForm = new TableForm();
            tableForm.Show();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            DisplayOrders();
        }

        //USER CODE
        private void DisplayUsers()
        {
            var usersCollection = db.GetCollection<User>("Users");

            List<User> users = usersCollection.AsQueryable().ToList();
            dataGridView2.DataSource = users;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                var usersCollection = db.GetCollection<User>("Users");

                User userDocument = new User
                {
                    Username = txtRoleID.Text,
                    Role = comboRole.Text
                };
                usersCollection.InsertOne(userDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            comboRole.ResetText();
            DisplayUsers();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DisplayUsers();
        }
    }
}
