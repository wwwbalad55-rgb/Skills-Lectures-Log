using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
namespace CarRentalSystem
{
    public partial class DashboardForm : Form
    {
        bool drag = false;
        Point start_point = new Point(0, 0);
        // 1. التعريفات العامة تكون هنا (برا الأقواس)
        string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";
        public static string UserRole = "";

        // 2. المُشيد الافتراضي (لازم يبقى موجود للـ Designer)
        public DashboardForm()
        {
            InitializeComponent();
        }

        // 3. المُشيد اللي يستلم الرتبة من اللوكن
        public DashboardForm(string role)
        {
            InitializeComponent();
            UserRole = role; // هنا خزننا القيمة اللي جتي
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            addUserControl(new UC_Dashboard());

            // هسة الفلترة راح تشتغل لوز
            if (UserRole == "Staff")
            {
                btnCustomers.Visible = false;
                // btnManageUsers.Visible = false; // تأكد من الاسم بالتصميم كما اتفقنا
            }
        }
        // ... باقي الكود مالتك


        
        
        private void GetDashboardStats()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();

                    // 1. حساب عدد السيارات
                    MySqlCommand cmd1 = new MySqlCommand("SELECT COUNT(*) FROM CarsTbl", con);
                    lblCarCount.Text = cmd1.ExecuteScalar().ToString();

                    // 2. حساب إجمالي الأرباح
                    MySqlCommand cmd2 = new MySqlCommand("SELECT SUM(Fine) FROM ReturnTbl", con);
                    object res = cmd2.ExecuteScalar();
                    lblRevenue.Text = (res != DBNull.Value) ? res.ToString() + " IQD" : "0 IQD";

                    // 3. حساب عدد الزبائن
                    MySqlCommand cmd3 = new MySqlCommand("SELECT COUNT(*) FROM CustomerTbl", con);
                    lblCustCount.Text = cmd3.ExecuteScalar().ToString();

                    // 4. حساب الحجوزات النشطة
                    MySqlCommand cmd4 = new MySqlCommand("SELECT COUNT(*) FROM RentalTbl", con);
                    lblRentCount.Text = cmd4.ExecuteScalar().ToString();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في جلب البيانات: " + ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            addUserControl(new UC_Return());
        }

        private void DashbordForm_Load(object sender, EventArgs e)
        {
           
            // 1. هذا السطر وحده كافي ووافي لأن هو ينظف ويضيف ويحدث
            addUserControl(new UC_Dashboard());

            // 2. هذن الأسطر اللي جوه "عطلهن" (حط ببدأيتهن //) لأن يمسحن الشغل
            // pnlMain.Controls.Clear();  <-- حط // هنا
            // GetDashboardStats();       <-- وحط // هنا لأن هي أصلاً موجودة بداخل الـ UC
        }
        
        


        private void addUserControl(UserControl userControl)
        {

            userControl.Dock = DockStyle.Fill;
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(userControl);
            userControl.BringToFront();
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {

            UC_Customers uc = new UC_Customers();
            addUserControl(uc); // هاي الميثود اللي استعملناها ويه الـ Fleet
        }





        private void btnFleet_Click(object sender, EventArgs e)
        {
            // الآن pnlMain تشير فقط للمربع (Panel) الموجود بالواجهة
            pnlMain.Controls.Clear();

            // الآن UC_Fleet هو الاسم الصحيح للكلاس
            UC_Fleet fleetPage = new UC_Fleet();

            fleetPage.Dock = DockStyle.Fill;
            pnlMain.Controls.Add(fleetPage);
        }

        private void btnBooking_Click(object sender, EventArgs e)
        {

            // استدعاء واجهة الحجز اللي صممتها
            addUserControl(new UC_Booking());
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            lblTime.Text = DateTime.Now.ToString("hh:mm:ss tt");
            lblDate.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void lblRentCount_Click(object sender, EventArgs e)
        {

        }
        private void GetRecentJobs()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();
                    // نجيب آخر 5 عمليات حجز من الجدول
                    string query = "SELECT * FROM RentalTbl ORDER BY RentId DESC LIMIT 5";
                    MySqlDataAdapter sda = new MySqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dataGridView1.DataSource = dt; // تأكد من الاسم هنا
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


        }

        private void pnlMain_Paint(object sender, PaintEventArgs e)
        {
            
            // هذا الكود يعبي المربعات الملونة بالأرقام
            GetDashboardStats();

            // هذا الكود اللي ضفناه هسة يعبي الجدول بآخر العمليات
            GetRecentJobs();
        }

        private void lblCustCount_Click(object sender, EventArgs e)
        {

        }
    }
    }


    
    
    

