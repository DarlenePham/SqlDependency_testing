using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlDependency_testing
{
    public partial class Form1 : Form
    {
        string connectionString = "data source=.; database=wms_kelantan; integrated security=SSPI";

        public Form1()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                if (cn.State == ConnectionState.Closed)
                    cn.Open();
                SqlCommand cmd = new SqlCommand("select id, name, price from product", cn);
                cmd.Notification = null;
                SqlDependency sqlDependency = new SqlDependency(cmd);
                sqlDependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
                DataTable dt = new DataTable("Product");
                dt.Load(cmd.ExecuteReader());
                dataGridView1.DataSource = dt;
            }
        }

        delegate void UpdateData();
        public void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency sqlDependency = sender as SqlDependency;
            sqlDependency.OnChange -= OnDependencyChange;
            UpdateData updateData = new UpdateData(LoadData);
            this.Invoke(updateData, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlClientPermission sqlClientPermission = new SqlClientPermission(System.Security.Permissions.PermissionState.Unrestricted);
            sqlClientPermission.Demand();
            SqlDependency.Start(connectionString);
            LoadData();
        }

        private void Form22_FormClosing(object sender, FormClosingEventArgs e)
        {
            SqlDependency.Stop(connectionString);
        }
    }
}
