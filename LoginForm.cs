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
    public partial class LoginForm : Form
    {
        bool drag = false;
        Point start_point = new Point(0, 0);
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            // التأكد من إدخال البيانات

            // 1. التأكد إن المستخدم اختار رتبة وما ترك الحقول فارغة
            if (cmbRole.SelectedIndex == -1 || txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("يرجى ملئ كل الحقول ");
                return;
            }

            try
            {
                string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // نعدل الاستعلام حتى يشمل الرتبة (URole)
                    string query = "SELECT COUNT(*) FROM UsersTbl WHERE UName=@name AND UPass=@pass AND URole=@role";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@name", txtUsername.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                    cmd.Parameters.AddWithValue("@role", cmbRole.SelectedItem.ToString()); // ياخذ القيمة المختارة من الكومبو بوكس

                    int result = Convert.ToInt32(cmd.ExecuteScalar());

                    if (result > 0) // إذا نجح تسجيل الدخول
                    {
                        // 1. نجيب الرتبة من الكومبو بوكس
                        string role = cmbRole.SelectedItem.ToString();

                        // 2. نفتح الداشبورد ونرسل الرتبة وياه
                        DashboardForm main = new DashboardForm(role);
                        main.Show();
                        this.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("الرجاء المحاولة في وقت لاحق " + ex.Message);
            }
        }



        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void chkShow_CheckedChanged(object sender, EventArgs e)
        {

            // هاي العلامة (!) معناها "اعكس الحالة"
            txtPassword.UseSystemPasswordChar = !chkShow.Checked;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;
        }

        private void lblExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lblExit_MouseDown(object sender, MouseEventArgs e)
        {


        }

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
           
            drag = true;
            start_point = new Point(e.X, e.Y);
        }

        private void LoginForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {
                Point p = PointToScreen(e.Location);
                this.Location = new Point(p.X - start_point.X, p.Y - start_point.Y);
            }
        }

        private void LoginForm_MouseUp(object sender, MouseEventArgs e)
        {
            drag = false;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {

        }
    }
    }

    

