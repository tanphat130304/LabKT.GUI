using DAL.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class StudentService
    {
        private readonly QuanlySVEntities _context;

        public StudentService()
        {
            _context = new QuanlySVEntities(); // Khởi tạo context để truy cập vào cơ sở dữ liệu
        }

        // Lấy tất cả sinh viên
        public List<Sinhvien> GetAll()
        {
            return _context.Sinhviens.ToList(); // Trả về danh sách sinh viên
        }

        // Thêm sinh viên
        public bool Insert(Sinhvien student)
        {
            try
            {
                _context.Sinhviens.Add(student);
                _context.SaveChanges();
                return true; // Trả về true nếu thêm thành công
            }
            catch
            {
                return false; // Trả về false nếu có lỗi xảy ra
            }
        }

        // Cập nhật sinh viên
        public string Update(Sinhvien student)
        {
            var existingStudent = _context.Sinhviens.Find(student.MaSV);
            if (existingStudent == null)
                return "Sinh viên không tồn tại!";

            existingStudent.HotenSV = student.HotenSV;
            existingStudent.MaLop = student.MaLop;
            existingStudent.NgaySinh = student.NgaySinh;
            _context.SaveChanges();
            return "Cập nhật sinh viên thành công!";
        }

        // Xóa sinh viên
        public string Delete(string maSV)
        {
            var student = _context.Sinhviens.Find(maSV);
            if (student == null)
                return "Sinh viên không tồn tại!";

            _context.Sinhviens.Remove(student);
            _context.SaveChanges();
            return "Xóa sinh viên thành công!";
        }
    }
}
