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
    public partial class UC_Booking : UserControl
    {

        string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";
        // ميثود لجلب السيارات المتاحة فقط (Available)
        private void fillCars()
        {
            using (MySqlConnection con = new MySqlConnection(ConString))
            {
                con.Open();
                cmbCarPlate.Items.Clear(); // حتى لا تتكرر الأسماء كل ما تفتح الواجهة

                // نطلب الموديل واللوحة سوية من جدول السيارات
                string query = "SELECT Model, Plate FROM CarsTbl WHERE Status = 'Available'";
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    // ندمج الموديل واللوحة حتى تطلع بالـ ComboBox بشكل مرتب
                    string carInfo = rdr["Model"].ToString() + " - " + rdr["Plate"].ToString();
                    cmbCarPlate.Items.Add(carInfo);
                }
            }
        }

        // ميثود لجلب أسماء الزبائن
        private void fillCustomers()
        {
            using (MySqlConnection con = new MySqlConnection(ConString))
            {
                con.Open();
                string query = "SELECT CustName FROM CustomerTbl";
                MySqlCommand cmd = new MySqlCommand(query, con);
                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    cmbCustomer.Items.Add(rdr["CustName"].ToString());
                }
            }
        }
        public UC_Booking()
        {
            InitializeComponent();

            // نصيح الميثودات حتى تنفذ شغلها أول ما تفتح الواجهة
            fillCars();
            fillCustomers();
            populate();      // إذا عندك جدول (DataGridView) للحجوزات ضيفها هماتين
        }
        private void populate()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // نجلب البيانات من جدول الحجوزات اللي سويته بالـ Workbench
                    string query = "SELECT * FROM RentalTbl";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // تأكد إن اسم الجدول بصفحة التصميم هو dgvRentals
                    dgvRentals.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في عرض الحجوزات: " + ex.Message);
            }
            // هذا السطر يخلي الجدول يفرش غصبن عليه ويملي كل المساحة
            dgvRentals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void cmbCarPlate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
                   
            // 1. فحص هل البيانات واصلة من الواجهة لو لا
            if (cmbCarPlate.SelectedItem == null || cmbCustomer.SelectedItem == null)
            {
                MessageBox.Show("يرجلى اختيار سيارة من الجدول");
                return;
            }

            // 2. محاولة الاتصال والخزن مع إظهار تفاصيل الخطأ إن وجد
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();

                    // إضافة الحجز
                    string query = "INSERT INTO RentalTbl (CarPlate, CustName, RentDate, ReturnDate, RentFees) " +
                                   "VALUES (@plate, @cust, @rdate, @retdate, @fees)";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@plate", cmbCarPlate.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@cust", cmbCustomer.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@rdate", dtpRentDate.Value.Date);
                    cmd.Parameters.AddWithValue("@retdate", dtpReturnDate.Value.Date);
                    cmd.Parameters.AddWithValue("@fees", txtFees.Text);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        // تحديث حالة السيارة في جدول السيارات
                        string updateQuery = "UPDATE CarsTbl SET Status = 'Rented' WHERE Plate = @plate";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);
                        updateCmd.Parameters.AddWithValue("@plate", cmbCarPlate.SelectedItem.ToString());
                        updateCmd.ExecuteNonQuery();

                        MessageBox.Show(" تم حجز السيارة ");

                        // تحديث الجدول المعروض أمامك
                        populate();
                    }
                    else
                    {
                        MessageBox.Show("لم يتم الاضافة في قاعدة البيانات");
                    }
                }
            }
            catch (MySqlException ex)
            {
                // إذا المشكلة بالداتا بيس (اسم عمود غلط، باسورد غلط) راح تطلع هنا
                MessageBox.Show("خطأ في قاعدة البيانات: " + ex.Message);
            }
            catch (Exception ex)
            {
                // لأي خطأ برمجي آخر
                MessageBox.Show("حدث خطأ عام: " + ex.Message);
            }
            // هذا السطر يخلي الجدول يفرش غصبن عليه ويملي كل المساحة
            dgvRentals.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void UC_Booking_Load(object sender, EventArgs e)
        {

        }

        private void dgvRentals_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    }

