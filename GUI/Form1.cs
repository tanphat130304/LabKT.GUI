using BUS;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace GUI
{
    public partial class frmSinhvien : Form
    {
        public readonly StudentService studentService = new StudentService();
        public readonly LopService lopService = new LopService(); 
        private readonly List<string> danhSachMaSVDaXoa = new List<string>(); 
        private bool isSaved = false; 

        public frmSinhvien()
        {
            InitializeComponent();
            txtMSSV.MaxLength = 12; // Giới hạn độ dài mã sinh viên
        }

        private void frmSinhvien_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadLop();
            dgvSinhvien.CellClick += dgvSinhvien_CellClick;
        }

        private void LoadLop()
        {
            try
            {
                var classes = lopService.GetAll();
                if (classes != null && classes.Any())
                {
                    cboLop.DataSource = classes;
                    cboLop.DisplayMember = "TenLop"; // Tên lớp hiển thị
                    cboLop.ValueMember = "MaLop"; // Mã lớp
                }
                else
                {
                    MessageBox.Show("Không có lớp nào để hiển thị.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách lớp: {ex.Message}");
            }
        }

        private void LoadData()
        {
            try
            {
                var students = studentService.GetAll();
                dgvSinhvien.DataSource = students.Select(s => new
                {
                    MaSV = s.MaSV,
                    HotenSV = s.HotenSV,
                    NgaySinh = s.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                    TenLop = s.Lop?.TenLop ?? ""
                }).ToList();

                // Đặt tiêu đề cho các cột
                dgvSinhvien.Columns["MaSV"].HeaderText = "Mã SV";
                dgvSinhvien.Columns["HotenSV"].HeaderText = "Họ tên";
                dgvSinhvien.Columns["NgaySinh"].HeaderText = "Ngày sinh";
                dgvSinhvien.Columns["TenLop"].HeaderText = "Lớp";
                dgvSinhvien.RowHeadersVisible = false;
                dgvSinhvien.ReadOnly = true;
                dgvSinhvien.AllowUserToAddRows = false;
                dgvSinhvien.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}");
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra lớp đã được chọn chưa
                if (cboLop.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn lớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy mã lớp từ ComboBox
                string maLop = cboLop.SelectedValue.ToString();

                // Kiểm tra mã sinh viên có bị trùng không
                string maSV = txtMSSV.Text.Trim();
                var existingSV = studentService.GetAll().FirstOrDefault(s => s.MaSV == maSV);
                if (existingSV != null)
                {
                    MessageBox.Show("Mã sinh viên đã tồn tại! Vui lòng nhập mã khác.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng sinh viên mới
                Sinhvien sv = new Sinhvien
                {
                    MaSV = maSV,
                    HotenSV = txtHotenSV.Text.Trim(),
                    NgaySinh = dtNgaysinh.Value,
                    MaLop = maLop 
                };

              
                if (studentService.Insert(sv)) 
                {
                    LoadData(); 
                    ClearForm();
                    MessageBox.Show("Thêm sinh viên thành công!");
                }
                else
                {
                    MessageBox.Show("Thêm sinh viên thất bại.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string maSV = txtMSSV.Text.Trim();
                if (!string.IsNullOrEmpty(maSV))
                {
                    // Thêm mã sinh viên vào danh sách đã xóa
                    danhSachMaSVDaXoa.Add(maSV);
                    string result = studentService.Delete(maSV);
                    MessageBox.Show(result);
                    LoadData();
                    ClearForm();
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                var student = new Sinhvien
                {
                    MaSV = txtMSSV.Text.Trim(),
                    HotenSV = txtHotenSV.Text.Trim(),
                    MaLop = cboLop.SelectedValue.ToString(),
                    NgaySinh = dtNgaysinh.Value
                };

                if (string.IsNullOrEmpty(student.MaSV) || string.IsNullOrEmpty(student.HotenSV))
                {
                    MessageBox.Show("Mã SV và Họ tên không được để trống.");
                    return;
                }

                if (student.MaSV.Length < 1 || student.MaSV.Length > 12)
                {
                    MessageBox.Show("Mã SV phải từ 1 đến 12 ký tự.");
                    return;
                }

                string result = studentService.Update(student);
                MessageBox.Show(result);
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        private void ClearForm()
        {
            txtMSSV.Clear();
            txtHotenSV.Clear();
            cboLop.SelectedIndex = -1;
            dtNgaysinh.Value = DateTime.Now; // Đặt lại ngày sinh về hiện tại
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn thoát không?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Thoát chương trình
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtTim.Text.Trim();
                var students = studentService.GetAll();
                var filteredStudents = students.Where(s => s.MaSV.Contains(searchTerm) || s.HotenSV.Contains(searchTerm))
                                               .Select(s => new
                                               {
                                                   MaSV = s.MaSV,
                                                   HotenSV = s.HotenSV,
                                                   NgaySinh = s.NgaySinh?.ToString("dd/MM/yyyy") ?? "",
                                                   TenLop = s.Lop?.TenLop ?? ""
                                               }).ToList();

                dgvSinhvien.DataSource = filteredStudents;

                if (filteredStudents.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}");
            }
        }

        private void btnKhong_Click(object sender, EventArgs e)
        {
            if (isSaved)
            {
                MessageBox.Show("Dữ liệu đã được lưu. Không cần phải hủy.");
            }
            else
            {
                ClearForm();
                LoadData(); // Tải lại dữ liệu
            }
        }

        private void dgvSinhvien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhvien.Rows[e.RowIndex];
                txtMSSV.Text = row.Cells["MaSV"].Value.ToString();
                txtHotenSV.Text = row.Cells["HotenSV"].Value.ToString();

                if (!string.IsNullOrEmpty(row.Cells["NgaySinh"].Value.ToString()))
                {
                    dtNgaysinh.Value = DateTime.ParseExact(row.Cells["NgaySinh"].Value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }

                var student = studentService.GetAll().FirstOrDefault(s => s.MaSV == row.Cells["MaSV"].Value.ToString());
                if (student != null)
                {
                    cboLop.SelectedValue = student.MaLop; // Chọn lớp của sinh viên
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý các sinh viên bị xóa
                foreach (string maSV in danhSachMaSVDaXoa)
                {
                   
                    var existingStudent = studentService.GetAll().FirstOrDefault(s => s.MaSV == maSV);
                    if (existingStudent != null)
                    {
                        studentService.Delete(maSV);
                    }
                }
                isSaved = true; // Đánh dấu là đã lưu
                MessageBox.Show("Lưu thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
    }
}
