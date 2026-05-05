using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
namespace CarRentalSystem
{
    public partial class UC_Customers : UserControl
    {


        // 1. تعريف نص الاتصال أولاً ونخليه static حتى نكدر نستخدمه فوق
        static string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";

        // 2. هسة نمرر نص الاتصال للـ Con
        MySqlConnection Con = new MySqlConnection(ConString);

        int key = 0;
        private void populate()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    string query = "SELECT * FROM CustomerTbl";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCustomer.DataSource = dt; // تأكد إن الاسم dgvCustomer مطابق للتصميم
                }
                // هذا السطر يخلي الجدول يفرش غصبن عليه ويملي كل المساحة
                dgvCustomer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في عرض البيانات: " + ex.Message);
            }
        }

        
        public UC_Customers()
        {
            InitializeComponent();
            populate(); // هذا السطر هو اللي يترس الجدول بيانات أول ما تفتح الواجهة
        }

        private void dgvCustomers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {



            if (e.RowIndex >= 0)
            {
                // نجلب الـ ID من العمود الأول بالسطر اللي ضغطت عليه
                key = Convert.ToInt32(dgvCustomer.Rows[e.RowIndex].Cells[0].Value.ToString());

                // نوزع البيانات على التكست بوكس حتى نعرف شدا نسوي
                txtName.Text = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtPhone.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtAddress.Text = dgvCustomer.Rows[e.RowIndex].Cells[3].Value.ToString();
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {

            if (txtName.Text == "" || txtPhone.Text == "" || txtAddress.Text == "")
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    Con.Open();
                    // إذا الـ ID يزيد وحده بقاعدة البيانات، امسحه من هنا
                    string query = "INSERT INTO customertbl (CustName, CustPhone, CustAddress) VALUES (@name, @phone, @add)";
                    MySqlCommand cmd = new MySqlCommand(query, Con);

                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@add", txtAddress.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Customer Successfully Added!");
                    Con.Close();
                    populate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    Con.Close();
                }
            }
        }



        

        private void dgvCustomers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
            if (e.RowIndex >= 0)
            {
                // السطر الأهم: جلب المعرّف (ID) من العمود الأول
                key = Convert.ToInt32(dgvCustomer.Rows[e.RowIndex].Cells[0].Value.ToString());

                // جلب باقي البيانات للصناديق
                txtName.Text = dgvCustomer.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtPhone.Text = dgvCustomer.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtAddress.Text = dgvCustomer.Rows[e.RowIndex].Cells[3].Value.ToString();
            }
        }
        


        private void button2_Click(object sender, EventArgs e)
        {



            if (key == 0)
            {
                MessageBox.Show("حدد الزبون من الجدول أولاً!");
            }
            else
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    string query = "DELETE FROM CustomerTbl WHERE CustId = @id";
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@id", key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم حذف الزبون بنجاح!");
                    populate(); // لتحديث الجدول فوراً
                    key = 0; // تصفير المفتاح بعد العملية
                }
            }
        }



        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (key == 0 || txtName.Text == "" || txtPhone.Text == "")
            {
                MessageBox.Show("تأكد من اختيار الزبون وملء البيانات!");
            }
            else
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(ConString))
                    {
                        con.Open();
                        string query = "UPDATE CustomerTbl SET CustName=@name, CustPhone=@phone, CustAdd=@add WHERE CustId=@id";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@name", txtName.Text);
                        cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@add", txtAddress.Text);
                        cmd.Parameters.AddWithValue("@id", key);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم تحديث بيانات الزبون بنجاح! ");
                        populate();
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            // لازم يكون هيج بنهاية الكود مالت الزر
            MessageBox.Show("تمت العملية بنجاح");
            populate(); // هاي هي اللي تخلي التغيير يظهر كدامك فوراً
        }

        private void button3_Click(object sender, EventArgs e)
        {



            
            if (key == 0)
            {
                MessageBox.Show("  حدد الزبون من الجدول أولاً");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // التعديل هنا: غيرنا CustAdd إلى CustAddress
                    string query = "UPDATE CustomerTbl SET CustName=@name, CustPhone=@phone, CustAddress=@add WHERE CustId=@id";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@add", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@id", key);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم تحديث بيانات الزبون بنجاح! ");
                    populate(); // تحديث الجدول فوراً
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ أثناء التحديث: " + ex.Message);
            }
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

    

