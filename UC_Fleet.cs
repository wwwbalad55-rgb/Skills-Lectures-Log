using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// في ملف UC_Fleet.cs
namespace CarRentalSystem
{

    public partial class UC_Fleet : UserControl
    {
        public UC_Fleet()
        {
            InitializeComponent();
            populate(); // استدعيها هنا حتى تترس الجدول بيانات أول ما تفتح الواجهة
        }
        // استبدل 'your_password' بالرمز اللي دخلت بيه للـ Workbench قبل شوية
        string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";
        private void populate()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    string query = "SELECT * FROM CarsTbl";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    
                    dgvCars.DataSource = dt; // dgvCars هو اسم الجدول مالتك بالتصميم

                }
                // هذا السطر يخلي الجدول يفرش غصبن عليه ويملي كل المساحة
                dgvCars.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show("عدنا مشكلة بالربط: " + ex.Message);
            }
        }


        private void clearFields()
        {
            txtModel.Clear();
            txtPlate.Clear();
            txtPrice.Clear();
            cmbStatus.SelectedIndex = -1;
        }

        private void btnAddCar_Click(object sender, EventArgs e)
        {
    
            // 1. نتأكد إن الحقول مو فارغة حتى لا يضرب البرنامج
            if (txtModel.Text == "" || txtPlate.Text == "" || txtPrice.Text == "" || cmbStatus.SelectedIndex == -1)
            {
                MessageBox.Show("يرجى ملئ الحقول");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // 2. جملة الإضافة (INSERT)
                    string query = "INSERT INTO CarsTbl (Model, Plate, Price, Status) VALUES (@model, @plate, @price, @status)";

                    MySqlCommand cmd = new MySqlCommand(query, con);

                    // 3. نربط المتغيرات بالقيم المكتوبة بالشاشة (Parameters) - هاي أأمن طريقة
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@plate", txtPlate.Text);
                    cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.SelectedItem.ToString());

                    cmd.ExecuteNonQuery(); // تنفيذ الأمر

                    MessageBox.Show("تمت إضافة السيارة بنجاح ! ");

                    // 4. تحديث الجدول فوراً حتى تظهر السيارة الجديدة
                    populate();

                    // 5. تنظيف الحقول حتى نضيف سيارة ثانية
                    clearFields();
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("صار عندنا خلل بالإضافة: " + ex.Message);
            }
        }
        

        private void dgvCars_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvCars_AutoSizeColumnsModeChanged(object sender, DataGridViewAutoSizeColumnsModeEventArgs e)
        {

        }

        private void dgvCars_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
            
        }

        private void dgvCars_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // نتأكد إن المستخدم ضغط على سطر بيه بيانات مو الفراغ الجوة
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCars.Rows[e.RowIndex];

                // نرجع البيانات للمربعات حسب ترتيب الأعمدة بالجدول
                txtModel.Text = row.Cells[1].Value.ToString();
                txtPlate.Text = row.Cells[2].Value.ToString();
                txtPrice.Text = row.Cells[3].Value.ToString();
                cmbStatus.Text = row.Cells[4].Value.ToString();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
           
            if (txtPlate.Text == "")
            {
                MessageBox.Show(" اختار سيارة من الجدول أول شي حتى نمسحها");
                return;
            }

            // رسالة تأكيد حتى لا يمسح شي بالخطأ
            DialogResult result = MessageBox.Show("صحيح تريد تمسح هاي السيارة؟", "تأكيد حذف", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(ConString))
                    {
                        con.Open();
                        // نمسح السيارة بناءً على رقم اللوحة (لأنها فريدة)
                        string query = "DELETE FROM CarsTbl WHERE Plate = @plate";
                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@plate", txtPlate.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("تم الحذف بنجاح!  ");

                        populate(); // تحديث الجدول
                        clearFields(); // تنظيف الحقول
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ بالحذف: " + ex.Message);
                }
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
           
            // نتأكد إن رقم اللوحة موجود لأن هو "مفتاح" التعديل عندنا
            if (txtPlate.Text == "")
            {
                MessageBox.Show("، اختار السيارة من الجدول أول شي حتى تتعدل بياناتها");
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // جملة التعديل: نحدث الموديل والسعر والحالة بناءً على رقم اللوحة
                    string query = "UPDATE CarsTbl SET Model=@model, Price=@price, Status=@status WHERE Plate=@plate";

                    MySqlCommand cmd = new MySqlCommand(query, con);

                    // ربط القيم الجديدة من المربعات
                    cmd.Parameters.AddWithValue("@model", txtModel.Text);
                    cmd.Parameters.AddWithValue("@price", txtPrice.Text);
                    cmd.Parameters.AddWithValue("@status", cmbStatus.Text);
                    cmd.Parameters.AddWithValue("@plate", txtPlate.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("تم تحديث بيانات السيارة  ");

                    populate();    // تحديث الجدول حتى تظهر التغييرات
                    clearFields(); // تنظيف الحقول
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("صار خلل بالتحديث: " + ex.Message);
            }
        
    }

        private void txtPrice_TextChanged(object sender, EventArgs e)
        {

        }
    }
    }
    

