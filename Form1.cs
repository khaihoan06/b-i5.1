
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace bt2
{
    public partial class Form1 : Form
    {
        private readonly string connectionString =
            "Server=localhost;Database=QuanLyDangKyKhoaHoc;Integrated Security=True;TrustServerCertificate=True;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var courses = new List<KeyValuePair<int, string>>()
            {
                new KeyValuePair<int, string>(1, "C# Basic"),
                new KeyValuePair<int, string>(2, ".NET Windows Forms"),
                new KeyValuePair<int, string>(3, "ASP.NET Core"),
                new KeyValuePair<int, string>(4, "Entity Framework")
            };

            comboBox1.DisplayMember = "Value";
            comboBox1.ValueMember = "Key";
            comboBox1.DataSource = courses;
        }

        private void maskedTextBox1_MaskInputRejected(
            object sender, MaskInputRejectedEventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(
            object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ form
            string phone = maskedTextBox1.Text.Trim();
            DateTime birth = dateTimePicker1.Value.Date;

            string gender = radioButton1.Checked
                ? radioButton1.Text
                : radioButton2.Checked
                    ? radioButton2.Text
                    : "Khác";

            if (comboBox1.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn khóa học.");
                return;
            }

            int courseId = Convert.ToInt32(comboBox1.SelectedValue);
            string courseName = comboBox1.Text;
            bool agreed = checkBox1.Checked;

            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Vui lòng nhập số điện thoại.");
                maskedTextBox1.Focus();
                return;
            }

            // Câu lệnh lưu dữ liệu
            string sql = @"
                INSERT INTO dbo.DangKyKhoaHoc
                (
                    SoDienThoai,
                    NgaySinh,
                    GioiTinh,
                    MaKhoaHoc,
                    TenKhoaHoc,
                    DongYDieuKhoan
                )
                VALUES
                (
                    @Phone,
                    @Birth,
                    @Gender,
                    @CourseId,
                    @CourseName,
                    @Agreed
                );";

            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20)
                        .Value = phone;

                    cmd.Parameters.Add("@Birth", SqlDbType.Date)
                        .Value = birth;

                    cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10)
                        .Value = gender;

                    cmd.Parameters.Add("@CourseId", SqlDbType.Int)
                        .Value = courseId;

                    cmd.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100)
                        .Value = courseName;

                    cmd.Parameters.Add("@Agreed", SqlDbType.Bit)
                        .Value = agreed;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    $"Đăng ký và lưu dữ liệu thành công!\n\n" +
                    $"SĐT: {phone}\n" +
                    $"Ngày sinh: {birth:dd/MM/yyyy}\n" +
                    $"Giới tính: {gender}\n" +
                    $"Khóa học: {courseName} (ID={courseId})\n" +
                    $"Đã đồng ý điều khoản: {(agreed ? "Có" : "Không")}",
                    "Thông tin đăng ký",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lưu dữ liệu thất bại!\n\n" + ex.Message,
                    "Lỗi SQL Server",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
