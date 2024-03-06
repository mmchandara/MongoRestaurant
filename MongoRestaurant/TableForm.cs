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
    public partial class TableForm : Form
    {
        static MongoClient dbClient = new MongoClient("mongodb+srv://mitchelchandara:Dragonnov669@cluster0.lfw8wai.mongodb.net/");
        static IMongoDatabase db = dbClient.GetDatabase("Restaurantdb");
        static IMongoCollection<Table> tablesCollection = db.GetCollection<Table>("Tables");
        public TableForm()
        {
            InitializeComponent();
        }
        class Table
        {
            [BsonId]
            public int TableID { get; set; }
            [BsonElement("Status")]
            public string Status { get; set; }
            [BsonElement("ReservationDetails")]
            public string ReservationDetails { get; set; }
        }
        private void DisplayTable()
        {
            var usersCollection = db.GetCollection<BsonDocument>("Tables");
            List<Table> table = tablesCollection.AsQueryable().ToList();
            dataGridView1.DataSource = table;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var tablesCollection = db.GetCollection<Table>("Tables");
                Table tableDocument = new Table
                {
                    TableID = int.Parse(txtTableID.Text),
                    Status = comboStatus.Text,
                    ReservationDetails = txtRes.Text 
                };
                tablesCollection.InsertOne(tableDocument);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtTableID.Clear();
            comboStatus.Items.Clear();
            txtRes.Clear();
            DisplayTable();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var filter = Builders<Table>.Filter.Eq("TableID", txtTableID.Text);
                var update = Builders<Table>.Update.Set("Status", comboStatus.Text).Set("ReservationDetails", txtRes.Text);
                
                tablesCollection.UpdateOne(filter, update);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            txtTableID.Clear();
            comboStatus.Items.Clear();
            txtRes.Clear();
            DisplayTable();
        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            DisplayTable();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtTableID.Text = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
            comboStatus.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtRes.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
        }
    }
}
