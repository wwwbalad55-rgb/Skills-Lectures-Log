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

namespace CarRentalSystem
{
    public partial class UC_Return : UserControl
    {
        string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";
        private void populate()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // نجيب الحجوزات الحالية حتى نكدر نرجعها
                    string query = "SELECT * FROM RentalTbl";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // تأكد إن اسم الـ DataGridView بالـ Design هو dgvRental
                    dgvRental.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في جلب بيانات الحجز: " + ex.Message);
            }
        }

        public UC_Return()
        {
            InitializeComponent();
            populate(); // لازم تضيف هذا السطر حتى يشتغل الجدول أول ما تفتح الواجهة
        }

        private void UC_Return_Load(object sender, EventArgs e)
        {

        }

        private void btnReturn_Click(object sender, EventArgs e)
        {



            if (dgvRental.SelectedRows.Count == 0)
            {
                MessageBox.Show(" يرجى تحديد الحجز من الجدول ");
                return;
            }

            try
            {
                // سحب البيانات من السطر المحدد بالجدول
                string plate = dgvRental.SelectedRows[0].Cells["CarPlate"].Value.ToString();
                string customer = dgvRental.SelectedRows[0].Cells["CustName"].Value.ToString();

                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();

                    // 1. إضافة عملية الإرجاع لجدول ReturnTbl
                    string queryReturn = "INSERT INTO ReturnTbl (CarPlate, CustName, ReturnDate, Fine) " +
                                         "VALUES (@plate, @cust, @date, @fine)";
                    MySqlCommand cmdRet = new MySqlCommand(queryReturn, con);
                    cmdRet.Parameters.AddWithValue("@plate", plate);
                    cmdRet.Parameters.AddWithValue("@cust", customer);
                    cmdRet.Parameters.AddWithValue("@date", DateTime.Today.Date);
                    cmdRet.Parameters.AddWithValue("@fine", txtFine.Text == "" ? "0" : txtFine.Text);
                    cmdRet.ExecuteNonQuery();

                    // 2. تحديث حالة السيارة في جدول السيارات لترجع Available
                    string queryUpdate = "UPDATE CarsTbl SET Status = 'Available' WHERE Plate = @plate";
                    MySqlCommand cmdUpd = new MySqlCommand(queryUpdate, con);
                    cmdUpd.Parameters.AddWithValue("@plate", plate);
                    cmdUpd.ExecuteNonQuery();

                    // 3. مسح الحجز من جدول الحجوزات (لأن السيارة رجعت)
                    string queryDel = "DELETE FROM RentalTbl WHERE CarPlate = @plate";
                    MySqlCommand cmdDel = new MySqlCommand(queryDel, con);
                    cmdDel.Parameters.AddWithValue("@plate", plate);
                    cmdDel.ExecuteNonQuery();

                    MessageBox.Show("تم إرجاع السيارة بنجاح، ورجعت متاحة للحجز! ");

                    txtFine.Text = ""; // تفريغ صندوق الغرامة
                    populate(); // تحديث الجدول بالواجهة
                }
                // هذا السطر يخلي الجدول يفرش غصبن عليه ويملي كل المساحة
                dgvRental.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("حدث خطأ: " + ex.Message);
            }



        }

        private void dgvRental_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvRental.SelectedRows.Count > 0)
            {
                // 1. جلب تاريخ الإرجاع المتوقع من الجدول
                DateTime returnDate = Convert.ToDateTime(dgvRental.SelectedRows[0].Cells["ReturnDate"].Value);

                // 2. جلب تاريخ اليوم
                DateTime today = DateTime.Today;

                // 3. حساب الفرق بالأيام
                TimeSpan ts = today - returnDate;
                int days = ts.Days;

                // 4. إذا كان هناك تأخير (الأيام أكثر من 0)
                if (days > 0)
                {
                    int finePerDay = 10000; // حدد مبلغ الغرامة لليوم الواحد (مثلاً 10 آلاف)
                    int totalFine = days * finePerDay;
                    txtFine.Text = totalFine.ToString();
                }
                else
                {
                    // إذا رجعها بنفس الوقت أو قبل الموعد
                    txtFine.Text = "0";
                }
            }
        }

        private void dgvRental_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    }
    

