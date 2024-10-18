using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BUS
{
    public class LopService
    {
        private QuanlySVEntities context;

        // Constructor khởi tạo context
        public LopService()
        {
            context = new QuanlySVEntities(); 
        }

        
        public List<Lop> GetAll()
        {
            using (var context = new QuanlySVEntities())
            {
                return context.Lops.AsNoTracking().ToList();
            }
        }

        public Lop GetById(string maLop)
        {
            using (var context = new QuanlySVEntities())
            {
                return context.Lops.SingleOrDefault(l => l.MaLop == maLop);
            }
        }

        public void Insert(Lop lop)
        {
            using (var context = new QuanlySVEntities())
            {
                try
                {
                    context.Lops.Add(lop);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    
                    throw new Exception("Lỗi khi thêm lớp: " + ex.Message);
                }
            }
        }

        public void Update(Lop lop)
        {
            using (var context = new QuanlySVEntities())
            {
                var existingLop = context.Lops.Find(lop.MaLop);
                if (existingLop == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy lớp có mã: " + lop.MaLop);
                }

                existingLop.TenLop = lop.TenLop;
                context.SaveChanges();
            }
        }

        public void Delete(string maLop)
        {
            using (var context = new QuanlySVEntities())
            {
                var lop = context.Lops.SingleOrDefault(l => l.MaLop == maLop);
                if (lop != null)
                {
                    context.Lops.Remove(lop);
                    context.SaveChanges();
                }
            }
        }
    }
}
