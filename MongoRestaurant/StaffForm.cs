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
    public partial class StaffForm : Form
    {
        static MongoClient dbClient = new MongoClient("BLANK");
        static IMongoDatabase db = dbClient.GetDatabase("Restaurantdb");
        static IMongoCollection<Order> ordersCollection = db.GetCollection<Order>("Orders");
        static IMongoCollection<BsonDocument> tablesCollection = db.GetCollection<BsonDocument>("Tables");
        public StaffForm()
        {
            InitializeComponent();
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
                    TotalAmount= decimal.Parse(txtAmount.Text)
                };
                ordersCollection.InsertOne(newOrder);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtUser.Clear();
            txtOrderName.Clear();
            txtAmount.Clear();
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
            txtOrderID.Clear();
            txtUser.Clear();
            txtAmount.Clear();
            DisplayOrders();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var deleteFilter = Builders<Order>.Filter.Eq("ID", ObjectId.Parse(txtOrderID.Text));
                ordersCollection.DeleteOne(deleteFilter);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        private void PrintBill(ObjectId orderID)
        {
            try
            {
                var ordersCollection = db.GetCollection<Order>("Orders");
                var orderFilter = Builders<Order>.Filter.Eq("ID", orderID);
                var orderDocument = ordersCollection.Find(orderFilter).FirstOrDefault();

                if (orderDocument != null)
                {
                    int userID = orderDocument.UserID;
                    string itemList = orderDocument.ItemList;
                    decimal totalAmount = orderDocument.TotalAmount;
                    decimal? discounts = orderDocument.Discounts;

                    string discountText = discounts.HasValue ? $"Discounts: {discounts:C}" : "Discounts: N/A";
                    string billDetails = $"OrderID: {orderID} UserID: {userID} Item List: {itemList}  Total Amount: {totalAmount:C} {discountText}";

                    MessageBox.Show(billDetails, "Bill Details");
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
        }
        private void btnBill_Click(object sender, EventArgs e)
        {
            if (ObjectId.TryParse(txtOrderID.Text, out ObjectId orderIDToPrint))
            {
                PrintBill(orderIDToPrint);
            }
            else
            {
                MessageBox.Show("Please enter a valid OrderID to print the bill.");
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
            txtDiscount.Clear();
            txtAmount.Clear();
            txtDiscount.Clear();;
            txtOrderID.Clear();
            DisplayOrders();
        }

        private void btnTables_Click(object sender, EventArgs e)
        {
            TableForm tableForm = new TableForm();
            tableForm.Show();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
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
    }
}
