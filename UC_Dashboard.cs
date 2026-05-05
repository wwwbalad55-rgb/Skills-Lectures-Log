using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient; // تأكد إن هذي المكتبة موجودة فوك

namespace CarRentalSystem
{
    public partial class UC_Dashboard : UserControl
    {
        // 1. نص الاتصال: خليه هنا بداخل الكلاس بس بره الميثودات
        string ConString = "server=localhost;port=3306;database=carrentaldb;user=root;password=1234;";

        public UC_Dashboard()
        {
            InitializeComponent();
            // 2. تشغيل الأرقام فوراً أول ما يفتح الداشبورد
            GetDashboardStats();
        }

        // 3. ميثود التايمر: هذي اللي تنفذ الكود كل ما يخلص الوقت
        

        // 4. الميثود الرئيسية لجلب البيانات من الـ MySQL
        private void GetDashboardStats()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConString))
                {
                    con.Open();

                    // حساب عدد السيارات
                    MySqlCommand cmd1 = new MySqlCommand("SELECT COUNT(*) FROM CarsTbl", con);
                    lblCarCount.Text = cmd1.ExecuteScalar().ToString();

                    // حساب إجمالي الأرباح (مع معالجة القيم الفارغة)
                    MySqlCommand cmd2 = new MySqlCommand("SELECT SUM(Fine) FROM ReturnTbl", con);
                    object res = cmd2.ExecuteScalar();
                    lblRevenue.Text = (res != DBNull.Value) ? res.ToString() + " IQD" : "0 IQD";

                    // حساب عدد الزبائن
                    MySqlCommand cmd3 = new MySqlCommand("SELECT COUNT(*) FROM CustomerTbl", con);
                    lblCustCount.Text = cmd3.ExecuteScalar().ToString();

                    // حساب الحجوزات النشطة
                    MySqlCommand cmd4 = new MySqlCommand("SELECT COUNT(*) FROM RentalTbl", con);
                    lblRentalCount.Text = cmd4.ExecuteScalar().ToString();
                }
            }
            catch (Exception ex)
            {
                // إذا صار خطأ بالاتصال، تطلع رسالة زغيرة وما يوكف البرنامج
                // MessageBox.Show("خطأ في التحديث: " + ex.Message);
            }
        }
    


private void UC_Dashboard_Load(object sender, EventArgs e)
        {


           
            // التأكد إذا كان الداخلي هو موظف (Staff)
            if (DashboardForm.UserRole == "Staff")
            {
                // إخفاء مربع الأرباح (اللي سويناه قبل شوية)
                panel3.Visible = false;

                // إخفاء مربع عدد الزبائن (البرتقالي)
                panel4.Visible = false;

                // إخفاء مربع الحجوزات النشطة (الأحمر)
                panel5.Visible = false;

                // ملاحظة: تأكد من كتابة الأسامي مثل ما موجودة عندك بالـ Properties
            }

            // استدعاء الأرقام من القاعدة (للحسابات المسموحة)
            GetDashboardStats();
        }
        
        

        private void timer1_Tick(object sender, EventArgs e)
        {
           
            // تحديث الأرقام من القاعدة
            GetDashboardStats();

            // تحديث الوقت والتاريخ الحقيقي
            lblDate.Text = DateTime.Now.ToLongDateString(); // يعرض التاريخ
            lblTime.Text = DateTime.Now.ToLongTimeString(); // يعرض الوقت بالثواني
        }
    }
    }



